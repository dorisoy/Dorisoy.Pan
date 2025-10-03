import {
  Directive,
  HostListener,
  Input,
  Output,
  ElementRef,
  EventEmitter,
  Renderer2,
} from '@angular/core';
import { Documents } from '@core/domain-classes/document';
import { Folder } from '@core/domain-classes/folder';
import { UserNotification } from '@core/domain-classes/notification';
import { environment } from '@environments/environment';
import {
  RecentActivity,
  RecentActivityType,
} from '@core/domain-classes/recent-activity';
import { CommonError } from '@core/error-handler/common-error';
import { CommonService } from '@core/services/common.service';
import { TreeViewService } from '@core/services/tree-view.service';
import { FileUploadProcessComponent } from '@shared/file-upload-process/file-upload-process.component';
import { OverlayPanel } from '@shared/overlay-panel/overlay-panel.service';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { ObservableService } from '@core/services/observable.service';
import { SubSink } from 'SubSink';
import { HttpEventType } from '@angular/common/http';
import { SecurityService } from '@core/security/security.service';
import { ComputeMD5, ChunkFile } from '@core/utils/file-helper';
import { Chunk } from '@core/core.types';

@Directive({
  selector: '[appDragDrop]',
})
export class DragDropDirective {
  totalFileUploaded: number = 0;
  sub$: SubSink;
  reading = 0;
  pathArray: Array<string> = [];
  pathFilesEntries: Array<any> = [];
  constructor(
    private el: ElementRef,
    private renderer: Renderer2,
    private overlay: OverlayPanel,
    private observableService: ObservableService,
    private commonService: CommonService,
    private toastrService: ToastrService,
    private treviewService: TreeViewService,
    private securityService: SecurityService
  ) {
    this.sub$ = new SubSink();
  }
  @Input() appDragDrop: Folder;
  @Input() rootFolder: Folder;
  fileOver: boolean = false;
  @HostListener('dragover', ['$event']) onDragOver(evt) {
    this.renderer.addClass(this.el.nativeElement, 'mat-elevation-z8');
    // Dragover listener @HostListener('dragover', ['$event']) onDragover (evt) {
    evt.preventDefault();
    evt.stopPropagation();
  }

  @HostListener('dragleave', ['$event']) public onDragLeave(evt) {
    // Dragleave listener @HostListener('dragleave', ['$event']) public onDragLeave (evt) {
    evt.preventDefault();
    evt.stopPropagation();
    this.renderer.removeClass(this.el.nativeElement, 'mat-elevation-z8');
  }

  @HostListener('drop', ['$event']) onDrop(evt) {
    // Drop listener @HostListener('drop', ['$event']) public ondrop(evt) {
    evt.preventDefault();
    evt.stopPropagation();
    this.fileOver = false;
    this.pathFilesEntries = [];
    this.pathArray = [];
    // let files: File[] = evt.target.files;
    // const paths = [...files].map(f => f['webkitRelativePath']);
    const items = evt.dataTransfer.items;
    const total = evt.dataTransfer.items.length;
    this.reading = 0;
    for (let i = 0; i < items.length; i++) {
      const item = items[i];
      if (item.kind === 'file') {
        const entry = item.webkitGetAsEntry();
        if (entry.isFile) {
          if (!this.securityService.hasClaim('IsFileUpload')) {
            this.toastrService.error('You do not have right to upload files.');
            return;
          }
          this.parseFileEntry(entry).then((c: any) => {
            this.onFileDropped(c, i, total, true);
          });
        } else if (entry.isDirectory) {
          if (!this.securityService.hasClaim('IsFileUpload')) {
            this.toastrService.error(
              'You do not have right to upload Folders.'
            );
            return;
          }
          this.parseDirectoryEntryList(entry);
        }
      }
    }
  }

  parseDirectoryEntryList(entry) {
    this.parseDirectoryEntry(entry).then((c: Array<any>) => {
      this.reading--;
      let filePaths: Array<any> = [...c].filter((f) => f.isFile);
      let directoryPaths: Array<any> = [...c].filter((f) => f.isDirectory);
      this.pathFilesEntries.push(...filePaths);

      let paths: Array<string> = filePaths.map((f) => f['fullPath']);
      paths = paths.map((path) => {
        return path.substr(1, path.length - 1);
      });
      this.pathArray.push(...paths);
      if (directoryPaths.length > 0) {
        directoryPaths.forEach((d) => {
          this.parseDirectoryEntryList(d);
        });
      }
      if (this.reading == 0) {
        this.sub$.sink = this.createFolders(this.pathArray).subscribe(
          (folders: Folder[]) => {
            this.pathArray = [];
            if (folders.length > 0) {
              const uniueIds = this.getUniqueIds(folders[0], folders);
              folders.forEach((folder) => {
                if (this.rootFolder.id == folder.parentId) {
                  this.treviewService.setRefreshTreeView(folder);
                } else {
                  if (uniueIds.find((c) => c == folder.id)) {
                    this.treviewService.setRefreshTreeView(folder);
                  }
                }
              });
              // if (folders.length > 0) {
              //   this.sendNotification();
              // }
            }
            this.pathFilesEntries.forEach((entry: any, index: number) => {
              this.parseFileEntry(entry).then((c: any) => {
                this.onFileDropped(
                  c,
                  index,
                  this.pathFilesEntries.length,
                  false
                );
              });
            });
          }
        );
      }
    });
  }
  getUniqueIds(folder: Folder, folders: Folder[]) {
    const ids = folders.filter((c) => c.parentId != folder.parentId);
    if (ids.length > 0) {
      return ids.map((c) => c.id);
    } else {
      return [folder.id];
    }
  }

  parseFileEntry(fileEntry) {
    return new Promise((resolve, reject) => {
      fileEntry.file(
        (file) => {
          file['fileName'] = fileEntry.fullPath.substr(
            1,
            fileEntry.fullPath.length - 1
          );
          resolve(file);
        },
        (err) => {
          reject(err);
        }
      );
    });
  }

  parseDirectoryEntry(directoryEntry) {
    this.reading++;
    const directoryReader = directoryEntry.createReader();
    return new Promise((resolve, reject) => {
      directoryReader.readEntries(
        (entries) => {
          resolve(entries);
        },
        (err) => {
          this.reading--;
          reject(err);
        }
      );
    });
  }
  onDoucmentUploadEvent(document: Documents) {
    if (document.thumbnailPath) {
      document.thumbnailPath = `${environment.apiUrl}${document.thumbnailPath}`;
    }
    let existingFolder = this.appDragDrop.documents.find(
      (c) => c.id == document.id
    );
    if (existingFolder) {
      this.appDragDrop.documents = this.appDragDrop.documents.map((doc) => {
        return doc.id == document.id ? document : doc;
      });
    } else {
      this.appDragDrop.documents.push(document);
    }
  }

  async uploadFile(file: File, fileCount: number) {
    this.observableService.initializeDocumentUploadProcess(file.name);
    let md5 = await ComputeMD5(file, (info) => {
      this.observableService.upadteDocumentUploadProgress(
        file.name,
        info.percent,
        false,
        true
      );
    });

    let chunk = (info: Chunk): Promise<boolean> => {
      return new Promise((resolve) => {
        let errorFunc = () => {
          this.observableService.upadteDocumentUploadProgress(
            file.name,
            100,
            true
          );
          if (info.current == info.total) {
            this.totalFileUploaded = this.totalFileUploaded + 1;
            if (this.totalFileUploaded == fileCount) {
              this.sendNotification();
            }
          }
        };
        const formData = new FormData();
        formData.append(file.name, info.file);
        this.sub$.sink = this.commonService
          .uploadFolderDocument(
            formData,
            this.appDragDrop.physicalFolderId,
            info.current,
            info.total,
            md5,
            info.size
          )
          .subscribe(
            (event) => {
              if (event.type === HttpEventType.UploadProgress) {
                let progress = Number(
                  (
                    (100 * (info.current - 1 + event.loaded / event.total)) /
                    info.total
                  ).toFixed(2)
                );
                if (progress >= 100) {
                  //数据包已接收完成，等待后端处理完成
                  progress = 99.5;
                }
                console.log(file.name + progress + '%');
                this.observableService.upadteDocumentUploadProgress(
                  file.name,
                  progress
                );
              } else if (event.type === HttpEventType.Response) {
                if (event.body == null) {
                  resolve(true);
                  return;
                }
                const returnDocument = event.body as Documents;
                if (returnDocument && returnDocument.id != undefined) {
                  this.addRecentActivity(null, returnDocument);
                  this.onDoucmentUploadEvent(returnDocument);
                  this.observableService.upadteDocumentUploadProgress(
                    file.name,
                    100
                  );
                  if (info.current == info.total) {
                    this.totalFileUploaded = this.totalFileUploaded + 1;
                    if (this.totalFileUploaded == fileCount) {
                      this.sendNotification();
                    }
                  }
                  resolve(false);
                } else {
                  errorFunc();
                  this.toastrService.error(
                    `上传错误: ${JSON.stringify(event.body).substring(0, 20)}`
                  );
                  resolve(false);
                }
              }
            },
            (error) => {
              errorFunc();
              resolve(false);
            }
          );
      });
    };

    ChunkFile(file, chunk);
  }

  async onFileDropped(
    file: any,
    currentIndex: number,
    totalLength: number,
    onlyFile: boolean
  ) {
    this.renderer.removeClass(this.el.nativeElement, 'mat-elevation-z8');
    if (!this.observableService.progressBarOverlay) {
      this.observableService.progressBarOverlay = this.overlay.open(
        FileUploadProcessComponent,
        {
          origin: 'global',
          hasBackdrop: false,
          position: { right: '10px', bottom: '10px' },
          mobilePosition: { left: 0, bottom: 0 },
        }
      );
    }

    try {
      this.uploadFile(file, totalLength);
    } catch (error) {
      this.toastrService.error(error);
    }
  }

  sendNotification() {
    const notification: UserNotification = {
      folderId: this.appDragDrop.physicalFolderId,
    };
    this.sub$.sink = this.commonService
      .sendNotification(notification)
      .subscribe((c) => {});
  }

  addRecentActivity(folder: Folder, documents: Documents) {
    const recentActivity: RecentActivity = {
      folderId: folder ? folder.id : null,
      documentId: documents ? documents.id : null,
      action: RecentActivityType.CREATED,
    };
    this.sub$.sink = this.commonService
      .addRecentActivity(recentActivity)
      .subscribe((c) => {});
  }

  createFolders(paths): Observable<Folder[] | CommonError> {
    return this.commonService.createChildFoders(
      paths,
      this.appDragDrop.id,
      this.appDragDrop.physicalFolderId
    );
  }
}
