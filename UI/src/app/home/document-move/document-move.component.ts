import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { CopyDocument } from '@core/domain-classes/copy-document';
import { Documents } from '@core/domain-classes/document';
import { Folder } from '@core/domain-classes/folder';
import { FolderPath } from '@core/domain-classes/folder-path';
import { MoveFolderRoot } from '@core/domain-classes/move-folder-root';
import { UserNotification } from '@core/domain-classes/notification';
import { ClonerService } from '@core/services/clone.service';
import { CommonService } from '@core/services/common.service';
import { ToastrService } from 'ngx-toastr';
import { BaseComponent } from 'src/app/base.component';
import { HomeService } from '../home.service';

@Component({
  selector: 'app-document-move',
  templateUrl: './document-move.component.html',
  styleUrls: ['./document-move.component.scss']
})
export class DocumentMoveComponent extends BaseComponent implements OnInit {

  selectedDocument: Documents;
  selectedFolder: Folder;
  folderPaths: FolderPath[] = [];
  isLoading = false;
  constructor(
    public dialogRef: MatDialogRef<DocumentMoveComponent>,
    @Inject(MAT_DIALOG_DATA) public data: MoveFolderRoot,
    private cloneService: ClonerService,
    private homeService: HomeService,
    private commonService: CommonService,
    private toastr: ToastrService) {
    super();
  }

  ngOnInit(): void {
    this.selectedFolder = this.cloneService.deepClone<Folder>(this.data.root);
    this.getChildFolders(this.data.root.id);
    this.getPath(this.data.root.id);
  }

  onFolderClick(parent: Folder) {
    this.selectedFolder = this.cloneService.deepClone<Folder>(parent);
    this.getChildFolders(parent.id);
    this.getPath(parent.id);
  }

  getChildFolders(parentId: string) {
    this.sub$.sink = this.homeService.getFolders(parentId)
      .subscribe((childs: Folder[]) => {
        this.selectedFolder.children = this.cloneService.deepClone<Folder[]>(childs);
      });
  }
  onCancel() {
    this.dialogRef.close({ 'flag': false });
  }

  onCopied() {
    if (this.data.sourceParentId === this.selectedFolder.physicalFolderId) {
      this.toastr.error('Distination and Source folder would not same.');
      return;
    }

    this.isLoading = true;
    var copyDocumement: CopyDocument = {
      documentId: this.data.sourceId,
      destinationFolderId: this.selectedFolder.id
    }
    this.sub$.sink = this.homeService.moveDocument(copyDocumement)
      .subscribe((c: boolean) => {
        this.isLoading = false;
        this.sendNotification();
        this.dialogRef.close({ 'flag': true });
      }, () => this.isLoading = false);
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

  getPath(id) {
    this.sub$.sink = this.homeService.getFolderParentsById(id)
      .subscribe((path: FolderPath[]) => {
        this.folderPaths = path;
      });
  }

  onPathClick(folder: Folder) {
    this.getChildFolders(folder.id);
  }

}
