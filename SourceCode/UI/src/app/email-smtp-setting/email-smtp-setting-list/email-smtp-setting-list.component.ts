import { Component, OnInit } from '@angular/core';
import { CommonDialogService } from '@core/common-dialog/common-dialog.service';
import { EmailSMTPSetting } from '@core/domain-classes/email-smtp-setting';
import { ToastrService } from 'ngx-toastr';
import { BaseComponent } from 'src/app/base.component';
import { EmailSmtpSettingService } from '../email-smtp-setting.service';

@Component({
  selector: 'app-email-smtp-setting-list',
  templateUrl: './email-smtp-setting-list.component.html',
  styleUrls: ['./email-smtp-setting-list.component.scss']
})
export class EmailSmtpSettingListComponent extends BaseComponent implements OnInit {
  emailSMTPSettings: EmailSMTPSetting[] = [];
  displayedColumns: string[] = ['action', 'userName', 'host', 'port', 'isDefault'];

  constructor(private emailSmtpSettingService: EmailSmtpSettingService,
    private commonDialogService: CommonDialogService,
    private toastrService: ToastrService) {
    super();
  }

  ngOnInit(): void {
    this.getEmailSMTPSettings();
  }

  getEmailSMTPSettings() {
    this.sub$.sink = this.emailSmtpSettingService.getEmailSMTPSettings().subscribe((settings: EmailSMTPSetting[]) => {
      this.emailSMTPSettings = settings;
    })
  }

  deleteEmailSMTPSetting(setting: EmailSMTPSetting) {
    this.sub$.sink = this.commonDialogService
      .deleteConformationDialog(`你确定要删除 ${setting.host}吗？`)
      .subscribe((isTrue: boolean) => {
        if (isTrue) {
          this.sub$.sink = this.emailSmtpSettingService.deleteEmailSMTPSetting(setting.id).subscribe(() => {
            this.toastrService.success('已成功删除电子邮件SMTP配置');
            this.getEmailSMTPSettings();
          });
        }
      });
  }
}
