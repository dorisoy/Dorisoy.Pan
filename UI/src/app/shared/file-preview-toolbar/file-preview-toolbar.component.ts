import { HttpEventType, HttpResponse } from '@angular/common/http';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { CommonDialogService } from '@core/common-dialog/common-dialog.service';
import { Documents } from '@core/domain-classes/document';
import { DocumentComment } from '@core/domain-classes/document-comment';
import { DocumentMoveDelete } from '@core/domain-classes/document-move-delete';
import { DocumentShareableLink } from '@core/domain-classes/document-shareable-link';
import { DocumentVersion } from '@core/domain-classes/document-version';
import { DocumentsPreview } from '@core/domain-classes/documents-preview';
import { Folder } from '@core/domain-classes/folder';
import { HierarchyShared } from '@core/domain-classes/hierarchy-shared';
import { MoveFolderRoot } from '@core/domain-classes/move-folder-root';
import { RecentActivity, RecentActivityType } from '@core/domain-classes/recent-activity';
import { SendFileFolderData } from '@core/domain-classes/send-file-folder';
import { SharedData } from '@core/domain-classes/shared-data';
import { ClonerService } from '@core/services/clone.service';
import { CommonService } from '@core/services/common.service';
import { DocumentVersionHistoryComponent } from '@shared/document-version-history/document-version-history.component';
import { OverlayPanelRef } from '@shared/overlay-panel/overlay-panel-ref';
import { RenameFileFolderComponent } from '@shared/rename-file-folder/rename-file-folder.component';
import { SendMailComponent } from '@shared/send-mail/send-mail.component';
import { ToastrService } from 'ngx-toastr';
import { BaseComponent } from 'src/app/base.component';
import { DocumentCommentComponent } from 'src/app/home/document-comment/document-comment.component';
import { DocumentCopyComponent } from 'src/app/home/document-copy/document-copy.component';
import { DocumentMoveComponent } from 'src/app/home/document-move/document-move.component';
import { DocumentSharedLinkComponent } from 'src/app/home/document-shared-link/document-shared-link.component';
import { HomeService } from 'src/app/home/home.service';
import { PreventSharedFolderComponent } from 'src/app/home/shared-folder-file/prevent-shared-folder/prevent-shared-folder.component';
import { SharedFolderFileComponent } from 'src/app/home/shared-folder-file/shared-folder-file.component';

@Component({
  selector: 'app-file-preview-toolbar',
  templateUrl: './file-preview-toolbar.component.html',
  styleUrls: ['./file-preview-toolbar.component.scss']
})
export class FilePreviewToolbarComponent extends BaseComponent implements OnInit {
  @Input() documentsPreview: DocumentsPreview;
  @Output() onNexPreivewClick = new EventEmitter();
  @Output() onMoveDeleteItem = new EventEmitter<DocumentMoveDelete>();
  currentIndex = 1;
  currentDocument: Documents;
  selectedFolder: Folder;
  rootFolder: Folder;
  constructor(
    private overlayRef: OverlayPanelRef,
    private homeService: HomeService,
    private toastrService: ToastrService,
    private clonerService: ClonerService,
    public dialog: MatDialog,
    public commonDialogService: CommonDialogService,
    private commonService: CommonService) {
    super();
  }

  ngOnInit(): void {
    this.currentDocument = this.documentsPreview.document;
    this.selectedFolder = this.documentsPreview.selectFolder;
    this.rootFolder = this.documentsPreview.rootFolder;
    if (this.documentsPreview.otherDocuments) {
      this.currentIndex = this.documentsPreview.otherDocuments.indexOf(this.documentsPreview.otherDocuments.find(c => c.id === this.documentsPreview.document.id)) + 1;
    }
  }

  closeToolbar() {
    this.overlayRef.close();
  }

  onNextClick() {
    if (this.documentsPreview.otherDocuments.length > this.currentIndex) {
      this.currentIndex = this.currentIndex + 1;
      const doc = this.clonerService.deepClone<Documents>(this.documentsPreview.otherDocuments[this.currentIndex - 1]);
      this.currentDocument = doc;
      this.onNexPreivewClick.emit(doc);
    }
  }

  onPreviousClick() {
    if (this.currentIndex > 1) {
      this.currentIndex = this.currentIndex - 1;
      const doc = this.clonerService.deepClone<Documents>(this.documentsPreview.otherDocuments[this.currentIndex - 1]);
      this.currentDocument = doc;
      this.onNexPreivewClick.emit(doc);
    }
  }

  sendEmail() {
    const emailData: SendFileFolderData = {
      type: 'file',
      document: this.currentDocument
    }
    this.dialog.open(SendMailComponent, {
      panelClass: ['full-width-dialog', 'min-width-dialog'],
      data: emailData,
    });
  }

  onShared(type: string) {
    const sharedDocument: SharedData = {
      type: type,
      document: this.currentDocument,
      folder: null,
      parentPhysicalFolderId: this.rootFolder.physicalFolderId,
    };
    this.dialog.open(SharedFolderFileComponent, {
      width: '500px',
      data: sharedDocument,
    });
  }

  renameDocument() {
    const sharedDocument: SharedData = {
      type: 'file',
      document: this.currentDocument,
      parentPhysicalFolderId: this.rootFolder.physicalFolderId,
    };
    this.dialog.open(RenameFileFolderComponent, {
      width: '300px',
      data: sharedDocument,
    });
  }

  deleteDocument() {
    this.sub$.sink = this.commonDialogService
      .deleteConformationDialog(
        `你确定要删除 ${this.currentDocument.name}`
      )
      .subscribe((isTrue: boolean) => {
        if (isTrue) {
          this.sub$.sink = this.homeService
            .deleteDocument(this.currentDocument.id)
            .subscribe(() => {
              this.toastrService.success('文档删除成功');
              this.overlayRef.emitValue({ id: this.currentDocument.id });
              this.overlayRef.close();
            });
        }
      });
  }

  onVersionHistoryClick(): void {
    let documentInfo = this.clonerService.deepClone<Documents>(this.currentDocument);
    this.sub$.sink = this.homeService
      .getDocumentVersion(this.currentDocument.id)
      .subscribe((documentVersions: DocumentVersion[]) => {
        documentInfo.documentVersions = documentVersions;
        this.dialog.open(DocumentVersionHistoryComponent, {
          panelClass: 'full-width-dialog',
          data: Object.assign({}, documentInfo),
        });
      });
  }

  onCommentClick() {
    let documentInfo = this.clonerService.deepClone<Documents>(this.currentDocument);
    this.sub$.sink = this.homeService
      .getDocumentComments(this.currentDocument.id)
      .subscribe((comments: DocumentComment[]) => {
        documentInfo.documentComments = comments;
        this.dialog.open(DocumentCommentComponent, {
          panelClass: ['full-width-dialog', 'min-width-dialog'],
          data: Object.assign({}, documentInfo),
        });
      });
  }

  onMoved() {
    this.checkSharedFolder(this.selectedFolder, this.currentDocument).then((folder: Folder) => {
      const moveFolderRoot: MoveFolderRoot = {
        sourceId: this.currentDocument.id,
        root: this.rootFolder,
        sourceName: this.currentDocument.name,
        sourceParentId: this.currentDocument.physicalFolderId
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
            this.toastrService.success('文件移除成功');
            this.onMoveDeleteItem.emit({ documentId: this.currentDocument.id, operation: 'move' });
            this.overlayRef.emitValue({ id: this.currentDocument.id });
            this.overlayRef.close();
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


  onCopied() {
    const moveFolderRoot: MoveFolderRoot = {
      sourceId: this.currentDocument.id,
      root: this.rootFolder,
      sourceName: this.currentDocument.name,
      sourceParentId: this.currentDocument.physicalFolderId
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
        }
      });
  }

  downloadDocument() {
    this.sub$.sink = this.homeService.downloadDocument(this.currentDocument).subscribe(
      (event) => {
        if (event.type === HttpEventType.Response) {
          this.downloadFile(event, this.currentDocument);
        }
      },
      (error) => {
        this.toastrService.error('error while downloading document');
      }
    );
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

  toggleDocumentStarred() {
    this.sub$.sink = this.homeService
      .toggleDocumentStarred(this.currentDocument.id)
      .subscribe((d) => {
        if (this.currentDocument.isStarred) {
          this.toastrService.success(`Unstarred ${this.currentDocument.name}.`);
        } else {
          this.toastrService.success(`Starred ${this.currentDocument.name}.`);
        }
        this.currentDocument.isStarred = !this.currentDocument.isStarred;
      });
  }

  onCreateShareableLink() {
    this.sub$.sink = this.homeService
      .getDocumentShareableLink(this.currentDocument.id)
      .subscribe((link: DocumentShareableLink) => {
        this.dialog.open(DocumentSharedLinkComponent, {
          width: '500px',
          data: { document: this.currentDocument, link: link },
        });
      });
  }



  addRecentActivity() {
    const recentActivity: RecentActivity = {
      folderId: null,
      documentId: this.currentDocument.id,
      action: RecentActivityType.VIEWED,
    };
    this.sub$.sink = this.commonService
      .addRecentActivity(recentActivity)
      .subscribe((c) => { });
  }

}
