import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ThemePalette } from '@angular/material/core';
import { ActivatedRoute, Router } from '@angular/router';
import { User } from '@core/domain-classes/user';
import { ToastrService } from 'ngx-toastr';
import { BaseComponent } from 'src/app/base.component';
import { UserService } from '../user.service';


@Component({
  selector: 'app-manage-user',
  templateUrl: './manage-user.component.html',
  styleUrls: ['./manage-user.component.scss']
})
export class ManageUserComponent extends BaseComponent implements OnInit {
  user: User;
  userForm: FormGroup;
  isEditMode = false;
  isLoading = false;
  color: ThemePalette = 'accent';
  constructor(private fb: FormBuilder,
    private router: Router,
    private activeRoute: ActivatedRoute,
    private userService: UserService,
    private toastrService: ToastrService) {
    super();
  }

  ngOnInit(): void {
    this.createUserForm();
    this.getIsAdminChange();
    this.sub$.sink = this.activeRoute.data.subscribe(
      (data: { user: User }) => {
        if (data.user) {
          this.isEditMode = true;
          this.userForm.patchValue(data.user);
          this.user = data.user;
        } else {
          this.userForm.get('password').setValidators([Validators.required, Validators.minLength(6)]);
          this.userForm.get('confirmPassword').setValidators([Validators.required]);
        }
      });
  }

  getIsAdminChange() {
    this.sub$.sink = this.userForm.get('isAdmin').valueChanges
      .subscribe((c: boolean) => {
        this.patchClaims(c);
      });
  }
  patchClaims(flag: boolean) {
    this.userForm.patchValue({
      userClaims: {
        isFolderCreate: flag,
        isFileUpload: flag,
        isDeleteFileFolder: flag,
        isSharedFileFolder: flag,
        isSendEmail: flag,
        isRenameFile: flag,
        isDownloadFile: flag,
        isCopyFile: flag,
        isCopyFolder: flag,
        isMoveFile: flag,
        isSharedLink: flag
      }
    });
  }

  createUserForm() {
    this.userForm = this.fb.group({
      id: [''],
      firstName: ['', [Validators.required]],
      lastName: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: [''],
      password: [''],
      confirmPassword: [''],
      address: [''],
      isActive: [true],
      isAdmin: [false],
      userClaims: this.fb.group({
        isFolderCreate: [false],
        isFileUpload: [false],
        isDeleteFileFolder: [false],
        isSharedFileFolder: [false],
        isSendEmail: [false],
        isRenameFile: [false],
        isDownloadFile: [false],
        isCopyFile: [false],
        isCopyFolder: [false],
        isMoveFile: [false],
        isSharedLink: [false]
      })
    }, {
      validator: this.checkPasswords
    });
  }

  checkPasswords(group: FormGroup) {
    let pass = group.get('password').value;
    let confirmPass = group.get('confirmPassword').value;
    return pass === confirmPass ? null : { notSame: true }
  }

  saveUser() {
    if (this.userForm.valid) {
      this.isLoading = true;
      const user = this.createBuildObject();
      if (this.isEditMode) {
        this.sub$.sink = this.userService.updateUser(user).subscribe(() => {
          this.isLoading = false;
          this.toastrService.success('用户更新成功');
          this.router.navigate(['/admin']);
        }, () => this.isLoading = false);
      } else {
        this.sub$.sink = this.userService.addUser(user).subscribe(() => {
          this.isLoading = false;
          this.toastrService.success('用户创建成功');
          this.router.navigate(['/admin']);
        }, () => this.isLoading = false);
      }
    } else {
      this.userForm.markAllAsTouched();
    }
  }

  createBuildObject(): User {
    const userId = this.userForm.get('id').value;
    const user: User = {
      id: userId,
      firstName: this.userForm.get('firstName').value,
      lastName: this.userForm.get('lastName').value,
      email: this.userForm.get('email').value,
      phoneNumber: this.userForm.get('phoneNumber').value,
      password: this.userForm.get('password').value,
      userName: this.userForm.get('email').value,
      isActive: this.userForm.get('isActive').value,
      isAdmin: this.userForm.get('isAdmin').value,
      address: this.userForm.get('address').value,
      userClaims: {
        isFolderCreate: this.userForm.get('userClaims.isFolderCreate').value,
        isFileUpload: this.userForm.get('userClaims.isFileUpload').value,
        isDeleteFileFolder:  this.userForm.get('userClaims.isDeleteFileFolder').value,
        isSharedFileFolder:  this.userForm.get('userClaims.isSharedFileFolder').value,
        isSendEmail:  this.userForm.get('userClaims.isSendEmail').value,
        isRenameFile:  this.userForm.get('userClaims.isRenameFile').value,
        isDownloadFile:  this.userForm.get('userClaims.isDownloadFile').value,
        isCopyFile:  this.userForm.get('userClaims.isCopyFile').value,
        isCopyFolder:  this.userForm.get('userClaims.isCopyFolder').value,
        isMoveFile:  this.userForm.get('userClaims.isMoveFile').value,
        isSharedLink:  this.userForm.get('userClaims.isSharedLink').value
      }
    }
    return user;
  }
}
