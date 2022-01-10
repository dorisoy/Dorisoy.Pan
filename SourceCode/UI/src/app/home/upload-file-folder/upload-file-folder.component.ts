import { HttpEventType } from '@angular/common/http';
import { ChangeDetectorRef, Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { Documents } from '@core/domain-classes/document';
import { Folder } from '@core/domain-classes/folder';
import { FileUploadProcessComponent } from '@shared/file-upload-process/file-upload-process.component';
import { OverlayPanelRef } from '@shared/overlay-panel/overlay-panel-ref';
import { OverlayPanel } from '@shared/overlay-panel/overlay-panel.service';
import { UserNotification } from '@core/domain-classes/notification';
import { CommonService } from '@core/services/common.service';
import { ToastrService } from 'ngx-toastr';
import { BaseComponent } from 'src/app/base.component';
import { HomeService } from '../home.service';
import { ObservableService } from '../../core/services/observable.service';
import { RecentActivity, RecentActivityType } from '@core/domain-classes/recent-activity';
import { TreeViewService } from '@core/services/tree-view.service';

@Component({
  selector: 'app-upload-file-folder',
  templateUrl: './upload-file-folder.component.html',
  styleUrls: ['./upload-file-folder.component.scss']
})
export class UploadFileFolderComponent extends BaseComponent implements OnInit, OnDestroy {
  totalFileUploaded = 0;
  @Input() selectedFolder: Folder;
  @Output() uploadDocumentEvent: EventEmitter<Documents> = new EventEmitter<Documents>();
  @Output() uploadFolderEvent: EventEmitter<Folder> = new EventEmitter<Folder>();
  isLoading: boolean = false;

  constructor(private homeService: HomeService,
    private toastrService: ToastrService,
    private observableService: ObservableService,
    private overlay: OverlayPanel,
    private cd: ChangeDetectorRef,
    private commonService: CommonService,
    private treviewService: TreeViewService) {
    super();
  }

  ngOnInit(): void {
  }

  fileEvent($event) {
    let files: File[] = $event.target.files;
    if (files.length == 0) {
      return;
    }
    this.totalFileUploaded = 0;
    if (!this.observableService.progressBarOverlay) {
      this.observableService.progressBarOverlay = this.overlay.open(FileUploadProcessComponent, {
        origin: 'global',
        hasBackdrop: false,
        position: { right: '10px', bottom: '10px' },
        mobilePosition: { left: 0, bottom: 0 }
      });
    }
    for (let index = 0; index < files.length; index++) {
      try {
        const reader = new FileReader();
        const file = files[index];
        reader.readAsDataURL(file);
        reader.onload = (_event) => {
          const formData = new FormData();
          formData.append(file.name, file);
          this.observableService.initializeDocumentUploadProcess(file.name);
          this.sub$.sink = this.commonService.uploadFolderDocument(formData, this.selectedFolder.physicalFolderId)
            .subscribe(event => {
              if (event.type === HttpEventType.UploadProgress) {
                const progress = Math.round(100 * event.loaded / event.total);
                this.cd.markForCheck();
                this.observableService.upadteDocumentUploadProgress(file.name, progress);
              }
              else if (event.type === HttpEventType.Response) {
                const returnDocument = event.body as Documents;
                this.addRecentActivity(null, returnDocument);
                this.uploadDocumentEvent.emit(returnDocument);
                this.observableService.upadteDocumentUploadProgress(file.name, 100);
                this.totalFileUploaded = this.totalFileUploaded + 1;
                if (this.totalFileUploaded == files.length) {
                  this.sendNotification();
                  $event.target.value = '';
                }
              }
            }, (error) => {
              this.observableService.upadteDocumentUploadProgress(file.name, 100, true);
              this.totalFileUploaded = this.totalFileUploaded + 1;
              if (this.totalFileUploaded == files.length) {
                this.sendNotification();
                $event.target.value = '';
              }
            });
        }
      }
      catch (error) {
        this.toastrService.error(error);
      }
    }
  }

  folderEvent($event) {
    let files: File[] = $event.target.files;
    const paths = [...files].map(f => f['webkitRelativePath']);
    this.sub$.sink = this.commonService.createChildFoders(paths, this.selectedFolder.id, this.selectedFolder.physicalFolderId)
      .subscribe((childs: Folder[]) => {
        this.treviewService.setRefreshTreeView(childs[0]);
        this.addRecentActivity(childs[0], null);
        this.uploadFolderEvent.emit(childs[0]);
        this.totalFileUploaded = 0;
        if (!this.observableService.progressBarOverlay) {
          this.observableService.progressBarOverlay = this.overlay.open(FileUploadProcessComponent, {
            origin: 'global',
            hasBackdrop: false,
            position: { right: '10px', bottom: '10px' },
            mobilePosition: { left: 0, bottom: 0 }
          });
        }
        for (let index = 0; index < files.length; index++) {
          const reader = new FileReader();
          const file = files[index];
          reader.readAsDataURL(file);
          reader.onload = (_event) => {
            const formData = new FormData();
            formData.append(file.name, file);
            this.observableService.initializeDocumentUploadProcess(file.name);
            this.sub$.sink = this.commonService.uploadFolderDocument(formData, this.selectedFolder.physicalFolderId)
              .subscribe(event => {
                if (event.type === HttpEventType.UploadProgress) {
                  const progress = Math.round(100 * event.loaded / event.total);
                  this.cd.markForCheck();
                  this.observableService.upadteDocumentUploadProgress(file.name, progress);
                }
                else if (event.type === HttpEventType.Response) {
                  this.observableService.upadteDocumentUploadProgress(file.name, 100);
                  this.totalFileUploaded = this.totalFileUploaded + 1;
                  if (this.totalFileUploaded == files.length) {
                    this.sendNotification();
                    $event.target.value = '';
                  }
                }
              }, (error) => {
                this.observableService.upadteDocumentUploadProgress(file.name, 100, true);
                this.totalFileUploaded = this.totalFileUploaded + 1;
                if (this.totalFileUploaded == files.length) {
                  this.sendNotification();
                  $event.target.value = '';
                }
              });
          }
        }
      }, () => {
        this.toastrService.error('Error while uploading Folder.')
      });

  }

  sendNotification() {
    if (this.selectedFolder.isShared) {
      const notification: UserNotification = {
        folderId: this.selectedFolder.physicalFolderId
      };
      this.sub$.sink = this.commonService.sendNotification(notification)
        .subscribe(c => {
        });
    }
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

  ngOnDestroy(): void {
    if (this.observableService.progressBarOverlay) {
      this.observableService.resetDocumentUploadProcess();
      this.observableService.progressBarOverlay.close();
      this.observableService.progressBarOverlay = null;
    }
  }
}
