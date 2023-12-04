import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { User } from '@core/domain-classes/user';
import { ToastrService } from 'ngx-toastr';
import { BaseComponent } from 'src/app/base.component';
import { UserService } from '../user.service';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss']
})
export class ResetPasswordComponent extends BaseComponent implements OnInit {
  resetPasswordForm: FormGroup;
  constructor(
    private userService: UserService,
    private fb: FormBuilder,
    public dialogRef: MatDialogRef<ResetPasswordComponent>,
    @Inject(MAT_DIALOG_DATA) public data: User,
    private toastrService: ToastrService) {
    super();
  }

  ngOnInit(): void {
    this.createResetPasswordForm();
    this.resetPasswordForm.get('email').setValue(this.data.email);
  }

  createResetPasswordForm() {
    this.resetPasswordForm = this.fb.group({
      email: [],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]],
    }, {
      validator: this.checkPasswords
    });
  }

  checkPasswords(group: FormGroup) {
    let pass = group.get('password').value;
    let confirmPass = group.get('confirmPassword').value;
    return pass === confirmPass ? null : { notSame: true }
  }

  resetPassword() {
    if (this.resetPasswordForm.valid) {
      this.sub$.sink = this.userService.resetPassword(this.createBuildObject()).subscribe(d => {
        this.toastrService.success('密码重置成功')
        this.dialogRef.close();
      })
    }
  }

  createBuildObject(): User {
    return {
      email: '',
      password: this.resetPasswordForm.get('password').value,
      userName: this.resetPasswordForm.get('email').value,
    }
  }

  onNoClick(): void {
    this.dialogRef.close();
  }
}
