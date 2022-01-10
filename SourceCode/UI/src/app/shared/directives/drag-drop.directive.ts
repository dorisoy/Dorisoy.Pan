import { Directive, HostListener, Input, ElementRef, Renderer2 } from '@angular/core';
import { Documents } from '@core/domain-classes/document';
import { Folder } from '@core/domain-classes/folder';
import { UserNotification } from '@core/domain-classes/notification';
import { RecentActivity, RecentActivityType } from '@core/domain-classes/recent-activity';
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

@Directive({
  selector: '[appDragDrop]'
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
    private securityService: SecurityService) {
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
          if(!this.securityService.hasClaim('IsFileUpload')){
            this.toastrService.error('You do not have right to upload files.');
            return;
          }
          this.parseFileEntry(entry).then((c: any) => {
            this.onFileDropped(c, i, total, true);
          });
        } else if (entry.isDirectory) {
          if(!this.securityService.hasClaim('IsFileUpload')){
            this.toastrService.error('You do not have right to upload Folders.');
            return;
          }
          this.parseDirectoryEntryList(entry);
        }
      }
    }
  }

  parseDirectoryEntryList(entry) {
    this.parseDirectoryEntry(entry)
      .then((c: Array<any>) => {
        this.reading--;
        let filePaths: Array<any> = [...c].filter(f => f.isFile);
        let directoryPaths: Array<any> = [...c].filter(f => f.isDirectory);
        this.pathFilesEntries.push(...filePaths);

        let paths: Array<string> = filePaths.map(f => f['fullPath']);
        paths = paths.map(path => {
          return path.substr(1, path.length - 1);
        });
        this.pathArray.push(...paths);
        if (directoryPaths.length > 0) {
          directoryPaths.forEach(d => {
            this.parseDirectoryEntryList(d);
          });
        }
        if (this.reading == 0) {
          this.sub$.sink = this.createFolders(this.pathArray)
            .subscribe((folders: Folder[]) => {
              this.pathArray = [];
              if (folders.length > 0) {
                const uniueIds = this.getUniqueIds(folders[0], folders);
                folders.forEach(folder => {
                  if (this.rootFolder.id == folder.parentId) {
                    this.treviewService.setRefreshTreeView(folder);
                  } else {
                    if (uniueIds.find(c => c == folder.id)) {
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
                  this.onFileDropped(c, index, this.pathFilesEntries.length, false);
                });
              });
            });
        }
      });
  }
  getUniqueIds(folder: Folder, folders: Folder[]) {
    const ids = folders.filter(c => c.parentId != folder.parentId);
    if (ids.length > 0) {
      return ids.map(c => c.id);
    } else {
      return [folder.id];
    }
  }

  parseFileEntry(fileEntry) {
    return new Promise((resolve, reject) => {
      fileEntry.file(
        file => {
          file['fileName'] = fileEntry.fullPath.substr(1, fileEntry.fullPath.length - 1);
          resolve(file);
        },
        err => {
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
        entries => {
          resolve(entries);
        },
        err => {
          this.reading--;
          reject(err);
        }
      );
    });
  }

  onFileDropped(file: any, currentIndex: number, totalLength: number, onlyFile: boolean) {
    this.renderer.removeClass(this.el.nativeElement, 'mat-elevation-z8');
    if (!this.observableService.progressBarOverlay) {
      this.observableService.progressBarOverlay = this.overlay.open(FileUploadProcessComponent, {
        origin: 'global',
        hasBackdrop: false,
        position: { right: '10px', bottom: '10px' },
        mobilePosition: { left: 0, bottom: 0 }
      });
    }
    try {
      const reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onload = (_event) => {
        const formData = new FormData();
        formData.append(file.name, file);
        formData.append("fullPath", file.fileName);
        this.observableService.initializeDocumentUploadProcess(file.name);
        this.sub$.sink = this.commonService.uploadFolderDocument(formData, this.appDragDrop.physicalFolderId)
          .subscribe(event => {
            if (event.type === HttpEventType.UploadProgress) {
              const progress = Math.round(100 * event.loaded / event.total);
              this.observableService.upadteDocumentUploadProgress(file.name, progress);
            }
            else if (event.type === HttpEventType.Response) {
              const returnDocument = event.body as Documents;
              this.addRecentActivity(null, returnDocument);
              this.observableService.upadteDocumentUploadProgress(file.name, 100);
              this.totalFileUploaded = this.totalFileUploaded + 1;
              if (onlyFile || (currentIndex == totalLength - 1)) {
                this.sendNotification();
              }
            }
          }, (error) => {
            this.observableService.upadteDocumentUploadProgress(file.name, 100, true);
            this.totalFileUploaded = this.totalFileUploaded + 1;
            this.sendNotification();
          });
      }
    }
    catch (error) {
      this.toastrService.error(error);
    }
  }

  sendNotification() {
    const notification: UserNotification = {
      folderId: this.appDragDrop.physicalFolderId
    };
    this.sub$.sink = this.commonService.sendNotification(notification)
      .subscribe(c => {
      });
  }


  addRecentActivity(folder: Folder, documents: Documents) {
    const recentActivity: RecentActivity = {
      folderId: folder ? folder.id : null,
      documentId: documents ? documents.id : null,
      action: RecentActivityType.CREATED
    };
    this.sub$.sink = this.commonService.addRecentActivity(recentActivity)
      .subscribe(c => {
      });
  }

  createFolders(paths): Observable<Folder[] | CommonError> {
    return this.commonService.createChildFoders(paths, this.appDragDrop.id, this.appDragDrop.physicalFolderId)
  }


}
