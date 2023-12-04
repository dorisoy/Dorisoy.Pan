import { Component, Inject, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import {
  MatDialog,
  MatDialogRef,
  MAT_DIALOG_DATA,
} from '@angular/material/dialog';
import { UserNotification } from '@core/domain-classes/notification';
import { SharedData } from '@core/domain-classes/shared-data';
import { SharedFileUser } from '@core/domain-classes/shared-file-user';
import { User } from '@core/domain-classes/user';
import { UserResource } from '@core/domain-classes/user-resource';
import { CommonError } from '@core/error-handler/common-error';
import { CommonService } from '@core/services/common.service';
import { environment } from '@environments/environment';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { debounceTime, finalize, switchMap, tap } from 'rxjs/operators';
import { BaseComponent } from 'src/app/base.component';
import { HomeService } from '../home.service';
import { SharedFolderUsersComponent } from './shared-folder-users/shared-folder-users.component';

@Component({
  selector: 'app-shared-folder-file',
  templateUrl: './shared-folder-file.component.html',
  styleUrls: ['./shared-folder-file.component.scss'],
})
export class SharedFolderFileComponent extends BaseComponent implements OnInit {
  sharedForm: FormGroup;
  users$: Observable<User[]> | CommonError;
  isLoading: boolean = false;
  get usersArray(): FormArray {
    return <FormArray>this.sharedForm.get('usersArray');
  }

  constructor(
    public dialogRef: MatDialogRef<SharedFolderFileComponent>,
    @Inject(MAT_DIALOG_DATA) public data: SharedData,
    private fb: FormBuilder,
    private commonService: CommonService,
    private homeService: HomeService,
    private dialog: MatDialog,
    private toastrService: ToastrService
  ) {
    super();
  }

  ngOnInit(): void {
    this.createSharedGroup();
    this.getUsers();
  }

  createSharedGroup() {
    this.sharedForm = this.fb.group({
      id: [
        this.data.type === 'folder'
          ? this.data.folder.id
          : this.data.document.id,
      ],
      type: [this.data.type],
      userName: [''],
      usersArray: this.fb.array([]),
    });
  }

  getUsers() {
    this.users$ = this.sharedForm.get('userName').valueChanges.pipe(
      debounceTime(500),
      tap(() => (this.isLoading = true)),
      switchMap((value) => {
        const userResource = new UserResource();
        userResource.skip = 0;
        userResource.pageSize = 10;
        userResource.first_name = value ? value : '';
        userResource.physicalFolderId =
          this.data.type === 'folder'
            ? this.data.folder.physicalFolderId
            : this.data.document.physicalFolderId;
        userResource.documentId =
          this.data.type != 'folder' ? this.data.document.id : '';
        userResource.type = this.data.type;
        return this.commonService.getUsers(userResource).pipe(
          tap(() => {
            this.isLoading = false;
          })
        );
      }),
      finalize(() => {
        this.isLoading = false;
      })
    );
  }

  selectUser(user: User) {
    const isExisting = (this.usersArray.value as User[]).find(
      (c) => c.id == user.id
    );
    if (!isExisting) {
      this.usersArray.push(this.createUserForm(user));
    }
    this.sharedForm.get('userName').setValue(null);
  }

  createUserForm(user: User): FormGroup {
    return this.fb.group({
      id: [user.id],
      firstName: [user.firstName],
      lastName: [user.lastName],
      email: [user.email],
      profilePhoto: [`${environment.apiUrl}${user.profilePhoto}`]
    });
  }

  removedUser(index: number) {
    this.usersArray.removeAt(index);
  }

  onSharedDocument() {
    this.isLoading = true;
    const users: User[] = this.usersArray.value;
    const buildObj = this.buildObject();
    if (this.sharedForm.valid && users.length > 0) {
      const userNotification: UserNotification = {
        folderId: this.data.parentPhysicalFolderId,
        users: users.map((c) => c.id),
      };
      if (this.data.type === 'folder') {
        this.sub$.sink = this.homeService
          .AddFolderUserPermission(buildObj)
          .subscribe((c) => {
            this.data.folder.isShared = true;
            this.data.folder.users = this.data.folder.users.concat(users);
            this.dialogRef.close();
            this.toastrService.success('文档共享成功');
            this.commonService.sendNotification(userNotification).subscribe();
            this.isLoading = false;
          }, () => this.isLoading = false);
      } else {
        this.sub$.sink = this.homeService
          .AddDocumentUserPermission(buildObj)
          .subscribe((c) => {
            this.isLoading = false;
            this.data.document.users = this.data.document.users.concat(users);
            this.toastrService.success('文档共享成功');
            this.commonService.sendNotification(userNotification).subscribe();
            this.dialogRef.close();
          }, () => this.isLoading = false);
      }
    }
  }
  onCancel() {
    this.dialogRef.close();
  }

  buildObject() {
    const users = this.usersArray.value;
    const sharedFileUser: SharedFileUser = {
      Id:
        this.data.type === 'folder'
          ? this.data.folder.id
          : this.data.document.id,
      users: users.map((c) => c.id),
    };
    return sharedFileUser;
  }

  onFolderUsers() {
    const dialogRef = this.dialog.open(SharedFolderUsersComponent, {
      panelClass: 'custom-modalbox-450',
      data: this.data,
    });
    this.sub$.sink = dialogRef.afterClosed().subscribe((isCurrentUser: boolean) => {
      if (isCurrentUser) {
        this.dialogRef.close();
      }
    });
  }

  onDocumentUsers() {
    const dialogRef = this.dialog.open(SharedFolderUsersComponent, {
      panelClass: 'custom-modalbox-450',
      data: this.data,
    });
    this.sub$.sink = dialogRef.afterClosed().subscribe((isCurrentUser: boolean) => {
      if (isCurrentUser) {
        this.dialogRef.close();
      }
    });
  }
}
