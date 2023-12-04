import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ActivatedRoute } from '@angular/router';
import { Folder } from '@core/domain-classes/folder';
import { FolderPath } from '@core/domain-classes/folder-path';
import { MoveFolder } from '@core/domain-classes/move-folder';
import { MoveFolderRoot } from '@core/domain-classes/move-folder-root';
import { UserNotification } from '@core/domain-classes/notification';
import { ClonerService } from '@core/services/clone.service';
import { CommonService } from '@core/services/common.service';
import { ToastrService } from 'ngx-toastr';
import { BaseComponent } from 'src/app/base.component';
import { HomeService } from '../home.service';

@Component({
  selector: 'app-move-folder',
  templateUrl: './move-folder.component.html',
  styleUrls: ['./move-folder.component.scss']
})
export class MoveFolderComponent extends BaseComponent implements OnInit {
  selectedFolder: Folder;
  folderPaths: Folder[] = [];
  isLoading = false;
  constructor(
    public dialogRef: MatDialogRef<MoveFolderComponent>,
    @Inject(MAT_DIALOG_DATA) public data: MoveFolderRoot,
    private activeRoute: ActivatedRoute,
    private cloneService: ClonerService,
    private homeService: HomeService,
    private toastr: ToastrService,
    private commonService: CommonService
  ) {
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

  onMoved() {
    if (this.data.sourceId === this.selectedFolder.id) {
      this.toastr.error('Distination and Source folder would not same.');
      return;
    }
    if (this.data.sourceParentId === this.selectedFolder.id) {
      this.toastr.error('Distination and Source folder would not same.');
      return;
    }
    this.isLoading = true;
    var moveFoler: MoveFolder = {
      sourceId: this.data.sourceId,
      distinationParentId: this.selectedFolder.id
    }
    this.sub$.sink = this.homeService.moveFolder(moveFoler)
      .subscribe((c: boolean) => {
        this.isLoading = false;
        this.dialogRef.close({ 'flag': true, sourceId: this.data.sourceId, distinationParentId: this.selectedFolder.id });
        this.sendNotification();
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
      .subscribe((path: Folder[]) => {
        this.folderPaths = path;
      });
  }



}
