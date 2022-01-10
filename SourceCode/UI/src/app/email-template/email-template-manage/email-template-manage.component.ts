import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { EmailTemplate } from '@core/domain-classes/email-template';
import { Editor } from 'ngx-editor';
import { ToastrService } from 'ngx-toastr';
import { BaseComponent } from 'src/app/base.component';
import { EmailTemplateService } from '../email-template.service';

@Component({
  selector: 'app-email-template-manage',
  templateUrl: './email-template-manage.component.html',
  styleUrls: ['./email-template-manage.component.scss']
})
export class EmailTemplateManageComponent extends BaseComponent implements OnInit {

  emailTemplateForm: FormGroup;
  emailTemplate: EmailTemplate;
  editor: Editor;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private emailTemplateService: EmailTemplateService,
    private router: Router,
    private toastrService: ToastrService
  ) {
    super();
  }

  ngOnInit(): void {
    this.editor = new Editor();
    this.createEmailTemplateForm();
    this.getEmailResolverData();
  }

  getEmailResolverData() {
    this.sub$.sink = this.route.data.subscribe(
      (data: { emailTemplate: EmailTemplate }) => {
        if (data.emailTemplate) {
          this.emailTemplate = data.emailTemplate;
          this.patchEmailTemplateData();
        }
      });
  }

  addUpdateEmailTemplate() {
    if (this.emailTemplateForm.valid) {
      if (this.emailTemplate) {
        this.sub$.sink = this.emailTemplateService
          .updateEmailTemplate(this.createBuildObject())
          .subscribe(c => {
            this.toastrService.success('邮件模板更新成功');
            this.router.navigate(['/admin/email-template']);
          });
      } else {
        this.sub$.sink = this.emailTemplateService
          .addEmailTemplate(this.createBuildObject())
          .subscribe(c => {
            this.toastrService.success('邮件模板保存成功')
            this.router.navigate(['/admin/email-template']);
          })
      }
    } else {
      for (let inner in this.emailTemplateForm.controls) {
        this.emailTemplateForm.get(inner).markAsDirty();
        this.emailTemplateForm.get(inner).updateValueAndValidity();
      }
    }
  }

  createBuildObject(): EmailTemplate {
    const emailTemplate: EmailTemplate = {
      id: this.emailTemplate ? this.emailTemplate.id : null,
      name: this.emailTemplateForm.get('name').value,
      subject: this.emailTemplateForm.get('subject').value,
      body: this.emailTemplateForm.get('body').value
    }
    return emailTemplate;
  }

  createEmailTemplateForm() {
    this.emailTemplateForm = this.fb.group({
      name: ['', [Validators.required]],
      subject: ['', [Validators.required]],
      body: ['', [Validators.required]]
    })
  }

  patchEmailTemplateData() {
    this.emailTemplateForm.patchValue({
      name: this.emailTemplate.name,
      subject: this.emailTemplate.subject,
      body: this.emailTemplate.body
    })
  }

}
