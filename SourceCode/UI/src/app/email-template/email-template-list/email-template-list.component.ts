import { Component, OnInit } from '@angular/core';
import { CommonDialogService } from '@core/common-dialog/common-dialog.service';
import { EmailTemplate } from '@core/domain-classes/email-template';
import { CommonError } from '@core/error-handler/common-error';
import { ToastrService } from 'ngx-toastr';
import { BaseComponent } from 'src/app/base.component';
import { EmailTemplateService } from '../email-template.service';

@Component({
  selector: 'app-email-template-list',
  templateUrl: './email-template-list.component.html',
  styleUrls: ['./email-template-list.component.scss']
})
export class EmailTemplateListComponent extends BaseComponent implements OnInit {

  emailTemplates: EmailTemplate[] = [];
  displayedColumns: string[] = ['action', 'name', 'subject'];
  constructor(
    private emailTemplateService: EmailTemplateService,
    private toastrService: ToastrService,
    private commonDialogService: CommonDialogService
  ) {
    super();
  }

  ngOnInit(): void {
    this.getEmailTemplates();
  }

  delteEmailTemplate(emailTemplate: EmailTemplate) {
    this.sub$.sink = this.commonDialogService
      .deleteConformationDialog(`你确定要删除:: ${emailTemplate.name}`)
      .subscribe((flag: boolean) => {
        if (flag) {
          this.sub$.sink = this.emailTemplateService.deleteEmailTemplate(emailTemplate)
            .subscribe(() => {
              this.toastrService.success('邮件模板删除成功');
              this.getEmailTemplates();
            });
        }
      });
  }

  getEmailTemplates(): void {
    this.sub$.sink = this.emailTemplateService.getEmailTemplates()
      .subscribe((data: EmailTemplate[]) => {
        this.emailTemplates = data;
      }, (err: CommonError) => {
        err.messages.forEach(msg => {
          this.toastrService.error(msg)
        });
      });
  }

}
