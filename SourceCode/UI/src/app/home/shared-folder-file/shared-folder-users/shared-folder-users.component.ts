import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { CommonDialogService } from '@core/common-dialog/common-dialog.service';
import { UserNotification } from '@core/domain-classes/notification';
import { RemoveFolderUser } from '@core/domain-classes/remove-folder-user';
import { SharedData } from '@core/domain-classes/shared-data';
import { UserAuth } from '@core/domain-classes/user-auth';
import { UserInfo } from '@core/domain-classes/user-info';
import { SecurityService } from '@core/security/security.service';
import { CommonService } from '@core/services/common.service';
import { environment } from '@environments/environment';
import { ToastrService } from 'ngx-toastr';
import { BaseComponent } from 'src/app/base.component';
import { HomeService } from '../../home.service';

@Component({
  selector: 'app-shared-folder-users',
  templateUrl: './shared-folder-users.component.html',
  styleUrls: ['./shared-folder-users.component.scss']
})
export class SharedFolderUsersComponent extends BaseComponent implements OnInit {
  currentLogginUser: UserAuth;
  isLoading = false;
  constructor(
    public dialogRef: MatDialogRef<SharedFolderUsersComponent>,
    @Inject(MAT_DIALOG_DATA) public data: SharedData,
    private homeService: HomeService,
    private toastrService: ToastrService,
    private commonService: CommonService,
    private commonDialogService: CommonDialogService,
    private securityService: SecurityService
  ) {
    super();

  }

  ngOnInit(): void {
    this.currentLogginUser = this.securityService.getUserDetail();
  }

  onFolderUserRemoved(user: UserInfo) {
    this.commonDialogService.deleteConformationDialog("Are you sure you want to remove this user")
      .subscribe((isTrue) => {
        if (isTrue) {
          const folder: RemoveFolderUser = {
            userId: user.id,
            folderId: this.data.folder.id,
            physicalFolderId: this.data.folder.physicalFolderId
          };
          const userNotification: UserNotification = {
            folderId: this.data.parentPhysicalFolderId,
            users: [user.id]
          };
          this.isLoading = true;
          this.sub$.sink = this.homeService.removePhysicalUsers(folder)
            .subscribe(c => {
              this.isLoading = false;
              this.toastrService.success('权限移除成功');
              this.commonService.sendNotification(userNotification).subscribe();
              this.data.folder.users = this.data.folder.users.filter(u => u.id != user.id);
              this.data.folder.isShared = this.data.folder.users.length > 1;
              if (this.currentLogginUser.id == folder.userId) {
                this.dialogRef.close(true);
              }
            }, () => this.isLoading = false);
        }
      });
  }

  onDocumentUserRemoved(user: UserInfo) {
    this.commonDialogService.deleteConformationDialog("你确定要移除用户吗")
      .subscribe((isTrue) => {
        if (isTrue) {
          const userNotification: UserNotification = {
            folderId: this.data.parentPhysicalFolderId,
            users: [user.id]
          };
          this.sub$.sink = this.homeService.removeDocumentUsers(this.data.document.id, user.id)
            .subscribe(c => {
              this.toastrService.success('权限移除成功');
              this.commonService.sendNotification(userNotification).subscribe();
              this.data.document.users = this.data.document.users.filter(u => u.id != user.id);
              if (this.currentLogginUser.id == user.id) {
                this.dialogRef.close(true);
              }
            });
        }
      });
  }

  onCancel() {
    this.dialogRef.close();
  }
}
