import { HttpEventType, HttpResponse } from '@angular/common/http';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { CommonDialogService } from '@core/common-dialog/common-dialog.service';
import { Documents } from '@core/domain-classes/document';
import { DocumentComment } from '@core/domain-classes/document-comment';
import { DocumentShareableLink } from '@core/domain-classes/document-shareable-link';
import { DocumentVersion } from '@core/domain-classes/document-version';
import { Folder } from '@core/domain-classes/folder';
import { MoveFolderRoot } from '@core/domain-classes/move-folder-root';
import {
  RecentActivity,
  RecentActivityType,
} from '@core/domain-classes/recent-activity';
import { SharedData } from '@core/domain-classes/shared-data';
import { User } from '@core/domain-classes/user';
import { ClonerService } from '@core/services/clone.service';
import { CommonService } from '@core/services/common.service';
import { DocumentVersionHistoryComponent } from '@shared/document-version-history/document-version-history.component';
import { OverlayPanel } from '@shared/overlay-panel/overlay-panel.service';
import { ToastrService } from 'ngx-toastr';
import { BaseComponent } from 'src/app/base.component';
import { DocumentCommentComponent } from 'src/app/home/document-comment/document-comment.component';
import { DocumentCopyComponent } from 'src/app/home/document-copy/document-copy.component';
import { DocumentMoveComponent } from 'src/app/home/document-move/document-move.component';
import { DocumentSharedLinkComponent } from 'src/app/home/document-shared-link/document-shared-link.component';
import { HomeService } from 'src/app/home/home.service';
import { ObservableService } from '@core/services/observable.service';
import { SharedFolderFileComponent } from 'src/app/home/shared-folder-file/shared-folder-file.component';
import { RenameFileFolderComponent } from '@shared/rename-file-folder/rename-file-folder.component';
import { HierarchyShared } from '@core/domain-classes/hierarchy-shared';
import { PreventSharedFolderComponent } from 'src/app/home/shared-folder-file/prevent-shared-folder/prevent-shared-folder.component';
import { SendMailComponent } from '@shared/send-mail/send-mail.component';
import { SendFileFolderData } from '@core/domain-classes/send-file-folder';
import { BasePreviewComponent } from '@shared/base-preview/base-preview.component';
import { DocumentsPreview } from '@core/domain-classes/documents-preview';
import { MatCheckboxChange } from '@angular/material/checkbox';

@Component({
  selector: 'app-document-base',
  templateUrl: './document-base.component.html',
  styleUrls: ['./document-base.component.scss'],
})
export class DocumentBaseComponent extends BaseComponent implements OnInit {
  @Input() documents: Documents[] = [];
  @Output() deleteDocumentEvent: EventEmitter<string> =
    new EventEmitter<string>();
  @Input() selectFolder: Folder;
  @Input() isCopy: Folder;
  @Input() isMove: Folder;
  rootFolder: Folder;
  @Output() copyDocumentEvent: EventEmitter<string> =
    new EventEmitter<string>();
  isCheck: boolean = false;
  constructor(
    public overlay: OverlayPanel,
    public commonService: CommonService,
    public homeService: HomeService,
    public toastrService: ToastrService,
    public dialog: MatDialog,
    public commonDialogService: CommonDialogService,
    public clonerService: ClonerService,
    public observableService: ObservableService
  ) {
    super();
  }

  ngOnInit(): void {
    this.rootFolderSubscription();
  }

  rootFolderSubscription() {
    this.sub$.sink = this.observableService.rootFolder$.subscribe(folder => {
      this.rootFolder = folder;
    });
  }

  getTooltip(users: User[]) {
    return users.map((c) => `${c.firstName} ${c.lastName}`).join(', ');
  }

  sendEmail(document: Documents) {
    const emailData: SendFileFolderData = {
      type: 'file',
      document: document
    }
    this.dialog.open(SendMailComponent, {
      panelClass: ['full-width-dialog', 'min-width-dialog'],
      data: emailData,
    });
  }

  onShared(folDoc: any, type: string) {
    const sharedDocument: SharedData = {
      type: type,
      document: type === 'folder' ? null : folDoc,
      folder: type === 'folder' ? folDoc : null,
      parentPhysicalFolderId: this.rootFolder.physicalFolderId,
    };
    this.dialog.open(SharedFolderFileComponent, {
      width: '500px',
      data: sharedDocument,
    });
  }

  renameDocument(document: Documents) {
    const sharedDocument: SharedData = {
      type: 'file',
      document: document,
      parentPhysicalFolderId: this.rootFolder.physicalFolderId,
    };
    this.dialog.open(RenameFileFolderComponent, {
      width: '300px',
      data: sharedDocument,
    });
  }
  checkAllSubscription() {
    this.sub$.sink = this.observableService.mainCheckBox$.subscribe(c => {
      this.isCheck = c;
    });
  }

  deleteDocument(document: Documents) {
    this.sub$.sink = this.commonDialogService
      .deleteConformationDialog(
        `你确定要删除 ${document.name}`
      )
      .subscribe((isTrue: boolean) => {
        if (isTrue) {
          this.sub$.sink = this.homeService
            .deleteDocument(document.id)
            .subscribe(() => {
              this.toastrService.success('文档删除成功');
              this.deleteDocumentEvent.emit(document.id);
            });
        }
      });
  }

  onVersionHistoryClick(document: Documents): void {
    let documentInfo = this.clonerService.deepClone<Documents>(document);
    this.sub$.sink = this.homeService
      .getDocumentVersion(document.id)
      .subscribe((documentVersions: DocumentVersion[]) => {
        documentInfo.documentVersions = documentVersions;
        this.dialog.open(DocumentVersionHistoryComponent, {
          panelClass: 'full-width-dialog',
          data: Object.assign({}, documentInfo),
        });
      });
  }

  onCommentClick(document: Documents) {
    let documentInfo = this.clonerService.deepClone<Documents>(document);
    this.sub$.sink = this.homeService
      .getDocumentComments(document.id)
      .subscribe((comments: DocumentComment[]) => {
        documentInfo.documentComments = comments;
        this.dialog.open(DocumentCommentComponent, {
          panelClass: ['full-width-dialog', 'min-width-dialog'],
          data: Object.assign({}, documentInfo),
        });
      });
  }

  onMoved(document: Documents) {
    this.checkSharedFolder(this.selectFolder, document).then((folder: Folder) => {
      const moveFolderRoot: MoveFolderRoot = {
        sourceId: document.id,
        root: this.rootFolder,
        sourceName: document.name,
        sourceParentId: document.physicalFolderId
      };
      const dialogRef = this.dialog.open(DocumentMoveComponent, {
        panelClass: 'custom-modalbox',
        minHeight: '510px',
        data: moveFolderRoot,
      });
      this.sub$.sink = dialogRef
        .afterClosed()
        .subscribe((result: { [key: string]: boolean }) => {
          if (result['flag']) {
            this.toastrService.success('文件移动成功');
            this.selectFolder.documents = this.selectFolder.documents.filter(c => c.id != document.id);
          }
        });
    });
  }

  checkSharedFolder(folder: Folder, document: Documents) {
    return new Promise((resolve, reject) => {
      if (folder.isShared) {
        const hierarchyShared: HierarchyShared = {
          id: folder.id,
          name: document.name,
          isParentShared: false,
          isChildShared: false,
          isFolderShared: true,
          operation: 'Move',
          isFolder: false
        };
        this.dialog.open(PreventSharedFolderComponent, {
          panelClass: 'custom-modalbox-450',
          minHeight: '100px',
          data: hierarchyShared,
        });
        reject();
      } else {
        // this.isLoading = true;
        this.sub$.sink = this.commonService
          .isParentShared(folder.id)
          .subscribe((c: HierarchyShared) => {
            // this.isLoading = false;
            c.name = folder.name;
            c.isFolderShared = false;
            c.isFolder = false;
            c.operation = "Move";
            if (c.isChildShared || c.isParentShared) {
              this.dialog.open(PreventSharedFolderComponent, {
                panelClass: 'custom-modalbox-450',
                minHeight: '100px',
                data: c,
              });
              reject();
            } else {
              resolve(folder);
            }
          }, (err) => {
            reject();
            // this.isLoading = false;
          });
      }
    })
  }


  onCopied(document: Documents) {
    const moveFolderRoot: MoveFolderRoot = {
      sourceId: document.id,
      root: this.rootFolder,
      sourceName: document.name,
      sourceParentId: document.physicalFolderId
    };
    const dialogRef = this.dialog.open(DocumentCopyComponent, {
      panelClass: 'custom-modalbox',
      minHeight: '510px',
      data: moveFolderRoot,
    });
    this.sub$.sink = dialogRef
      .afterClosed()
      .subscribe((result: { [key: string]: boolean }) => {
        if (result['flag']) {
          this.toastrService.success('文件拷贝成功');
          this.copyDocumentEvent.emit(document.physicalFolderId)
        }
      });
  }

  downloadDocument(document: Documents) {
    this.sub$.sink = this.homeService.downloadDocument(document).subscribe(
      (event) => {
        if (event.type === HttpEventType.Response) {
          this.downloadFile(event, document);
        }
      },
      (error) => {
        this.toastrService.error('error while downloading document');
      }
    );
  }

  addOrRemoveDocumentId(documentId: string, value: MatCheckboxChange) {
    this.observableService.setDocumentOrFolderId({
      documentId: documentId,
      folderId: '',
    });
    if (!value.checked) {
      this.observableService.mainCheckBox$.next(value.checked);
    }
  }

  private downloadFile(data: HttpResponse<Blob>, doc: Documents) {
    const downloadedFile = new Blob([data.body], { type: data.body.type });
    const a = document.createElement('a');
    a.setAttribute('style', 'display:none;');
    document.body.appendChild(a);
    a.download = doc.name;
    a.href = URL.createObjectURL(downloadedFile);
    a.target = '_blank';
    a.click();
    document.body.removeChild(a);
  }

  toggleDocumentStarred(document: Documents) {
    this.sub$.sink = this.homeService
      .toggleDocumentStarred(document.id)
      .subscribe((d) => {
        if (document.isStarred) {
          this.toastrService.success(`Unstarred ${document.name}.`);
        } else {
          this.toastrService.success(`Starred ${document.name}.`);
        }
        document.isStarred = !document.isStarred;
      });
  }

  onCreateShareableLink(document: Documents) {
    this.sub$.sink = this.homeService
      .getDocumentShareableLink(document.id)
      .subscribe((link: DocumentShareableLink) => {
        this.dialog.open(DocumentSharedLinkComponent, {
          width: '500px',
          data: { document, link },
        });
      });
  }

  onDocumentView(document: Documents, otherDocuments?: Documents[], isViewOlnly = false) {
    if (!isViewOlnly) {
      this.addRecentActivity(document);
    }
    const documentsPreview: DocumentsPreview = {
      document: { ...document },
      otherDocuments: otherDocuments ? [...otherDocuments] : [],
      selectFolder: { ...this.selectFolder },
      rootFolder: { ...this.rootFolder }
    };

    const overlayContent = this.overlay.open(BasePreviewComponent, {
      position: 'center',
      origin: 'global',
      panelClass: ['file-preview-overlay-container', 'light-black-background'],
      data: documentsPreview,
    });
    this.sub$.sink = overlayContent.valueChanged().subscribe((c: any) => {
      if (c['id']) {
        this.documents = this.clonerService.deepClone<Documents[]>(this.documents.filter(d => d.id != c['id']));
      }
    })
  }

  addRecentActivity(documents: Documents) {
    const recentActivity: RecentActivity = {
      folderId: null,
      documentId: documents.id,
      action: RecentActivityType.VIEWED,
    };
    this.sub$.sink = this.commonService
      .addRecentActivity(recentActivity)
      .subscribe((c) => { });
  }
}
