import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { User } from '@core/domain-classes/user';
import { SecurityService } from '@core/security/security.service';
import { environment } from '@environments/environment';
import { ToastrService } from 'ngx-toastr';
import { BaseComponent } from 'src/app/base.component';
import { ChangePasswordComponent } from '../change-password/change-password.component';
import { UserService } from '../user.service';

@Component({
  selector: 'app-my-profile',
  templateUrl: './my-profile.component.html',
  styleUrls: ['./my-profile.component.scss']
})
export class MyProfileComponent extends BaseComponent implements OnInit {
  userForm: FormGroup;
  user: User;
  fileSelected: File;
  imgURL: any;
  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private toastrService: ToastrService,
    private dialog: MatDialog,
    private securityService: SecurityService) {
    super();
  }

  ngOnInit(): void {
    this.createUserForm();
    this.sub$.sink = this.userService.getUserProfile().subscribe((user: User) => {
      this.user = user;
      if (this.user) {
        if (this.user.profilePhoto) {
          this.imgURL = environment.apiUrl + this.user.profilePhoto;
        }
        this.userForm.patchValue(this.user);
      }
    });

  }

  fileEvent($event) {
    this.fileSelected = $event.target.files[0];
    if (!this.fileSelected) {
      return;
    }
    const mimeType = this.fileSelected.type;
    if (mimeType.match(/image\/*/) == null) {
      return;
    }
    const reader = new FileReader();
    reader.readAsDataURL(this.fileSelected);
    // tslint:disable-next-line: variable-name
    reader.onload = (_event) => {
      const formData = new FormData();
      formData.append(this.fileSelected.name, this.fileSelected);
      this.userService.updateProfilePhoto(formData).subscribe((user: User) => {
        this.toastrService.success('照片更新成功');
        this.imgURL = reader.result;
        this.securityService.updateUserProfile(user);
        $event.target.value = '';
      });
    }
  }

  removeImage() {
    const formData = new FormData();
    this.userService.updateProfilePhoto(formData).subscribe((user: User) => {
      this.toastrService.success('照片移除成功');
      this.imgURL = `${environment.apiUrl}${user.profilePhoto}`;
      this.securityService.updateUserProfile(user);
    });
  }

  createUserForm() {
    this.userForm = this.fb.group({
      id: [''],
      firstName: ['', [Validators.required]],
      lastName: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: [''],
      address: ['']
    });
  }

  updateProfile() {
    if (this.userForm.valid) {
      const user = this.createBuildObject();
      this.sub$.sink = this.userService.updateUserProfile(user)
        .subscribe((user: User) => {
          this.toastrService.success('个人信息更新成功');
          this.securityService.updateUserProfile(user);
        });
    } else {
      this.toastrService.error('请输入正确的数据')
    }
  }

  createBuildObject(): User {
    const user: User = {
      id: this.userForm.get('id').value,
      firstName: this.userForm.get('firstName').value,
      lastName: this.userForm.get('lastName').value,
      email: this.userForm.get('email').value,
      phoneNumber: this.userForm.get('phoneNumber').value,
      userName: this.userForm.get('email').value,
      address: this.userForm.get('address').value
    }
    return user;
  }

  changePassword(): void {
    this.dialog.open(ChangePasswordComponent, {
      width: '350px',
      data: Object.assign({}, this.user)
    });
  }
}
