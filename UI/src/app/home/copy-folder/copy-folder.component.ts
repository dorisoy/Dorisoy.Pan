import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Folder } from '@core/domain-classes/folder';
import { FolderPath } from '@core/domain-classes/folder-path';
import { MoveFolder } from '@core/domain-classes/move-folder';
import { MoveFolderRoot } from '@core/domain-classes/move-folder-root';
import { UserNotification } from '@core/domain-classes/notification';
import { ClonerService } from '@core/services/clone.service';
import { CommonService } from '@core/services/common.service';
import { TreeViewService } from '@core/services/tree-view.service';
import { ToastrService } from 'ngx-toastr';
import { BaseComponent } from 'src/app/base.component';
import { HomeService } from '../home.service';

@Component({
  selector: 'app-copy-folder',
  templateUrl: './copy-folder.component.html',
  styleUrls: ['./copy-folder.component.scss']
})
export class CopyFolderComponent extends BaseComponent implements OnInit {
  selectedFolder: Folder;
  folderPaths: FolderPath[] = [];
  isLoading = false;
  constructor(
    public dialogRef: MatDialogRef<CopyFolderComponent>,
    @Inject(MAT_DIALOG_DATA) public data: MoveFolderRoot,
    private cloneService: ClonerService,
    private homeService: HomeService,
    private commonService: CommonService,
    private treeViewService: TreeViewService,
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
    this.sub$.sink = this.homeService.getFoldersForCopyAndMove(parentId, this.data.sourceId)
      .subscribe((childs: Folder[]) => {
        this.selectedFolder.children = this.cloneService.deepClone<Folder[]>(childs);
      });
  }

  onCancel() {
    this.dialogRef.close({ 'flag': false });
  }

  onCopied() {
    if (this.data.sourceParentId === this.selectedFolder.id) {
      this.toastr.error('Distination and Source folder would not same.');
      return;
    }
    this.isLoading = true;
    var moveFoler: MoveFolder = {
      sourceId: this.data.sourceId,
      distinationParentId: this.selectedFolder.id
    }
    this.sub$.sink = this.homeService.copyFolder(moveFoler)
      .subscribe((c: Folder) => {
        this.isLoading = false;
        this.treeViewService.setRefreshTreeView(c);
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
        .subscribe(c => { });
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
