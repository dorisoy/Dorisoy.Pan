import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { EmailSMTPSetting } from '@core/domain-classes/email-smtp-setting';
import { ToastrService } from 'ngx-toastr';
import { BaseComponent } from 'src/app/base.component';
import { EmailSmtpSettingService } from '../email-smtp-setting.service';

@Component({
  selector: 'app-manage-email-smtp-setting',
  templateUrl: './manage-email-smtp-setting.component.html',
  styleUrls: ['./manage-email-smtp-setting.component.scss']
})
export class ManageEmailSmtpSettingComponent extends BaseComponent implements OnInit {
  isEditMode: boolean = false;
  smtpSettingForm: FormGroup;
  constructor(
    private router: Router,
    private fb: FormBuilder,
    private activeRoute: ActivatedRoute,
    private emailSmtpSettingService: EmailSmtpSettingService,
    private toastrService: ToastrService
  ) {
    super();
  }

  ngOnInit(): void {
    this.createEmailSMTPForm();
    this.sub$.sink = this.activeRoute.data.subscribe(
      (data: { smtpSetting: EmailSMTPSetting }) => {
        if (data.smtpSetting) {
          this.isEditMode = true;
          this.smtpSettingForm.patchValue(data.smtpSetting);
        }
      });
  }

  createEmailSMTPForm() {
    this.smtpSettingForm = this.fb.group({
      id: [''],
      host: ['', [Validators.required]],
      userName: ['', [Validators.required]],
      password: ['', [Validators.required]],
      isEnableSSL: [false],
      port: ['', [Validators.required]],
      isDefault: [false],
    });
  }

  saveEmailSMTPSetting() {
    if (this.smtpSettingForm.valid) {
      const emailSMTPSetting = this.createBuildObject();
      if (this.isEditMode) {
        this.sub$.sink = this.emailSmtpSettingService.updateEmailSMTPSetting(emailSMTPSetting).subscribe(() => {
          this.toastrService.success('SMTP更新成功');
          this.router.navigate(['/admin/email-smtp']);
        });
      } else {
        this.sub$.sink = this.emailSmtpSettingService.addEmailSMTPSetting(emailSMTPSetting).subscribe(() => {
          this.toastrService.success('SMTP创建成功');
          this.router.navigate(['/admin/email-smtp']);
        });
      }
    } else {
      this.smtpSettingForm.markAllAsTouched();
    }
  }

  createBuildObject(): EmailSMTPSetting {
    const id = this.smtpSettingForm.get('id').value;
    const user: EmailSMTPSetting = {
      id: id,
      host: this.smtpSettingForm.get('host').value,
      userName: this.smtpSettingForm.get('userName').value,
      password: this.smtpSettingForm.get('password').value,
      isEnableSSL: this.smtpSettingForm.get('isEnableSSL').value,
      port: this.smtpSettingForm.get('port').value,
      isDefault: this.smtpSettingForm.get('isDefault').value
    }
    return user;
  }
}
