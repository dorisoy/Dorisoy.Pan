import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { EmailParameter } from '@core/domain-classes/email-parameter';
import { EmailTemplate } from '@core/domain-classes/email-template';
import { FileInfo } from '@core/domain-classes/file-info';
import { Editor } from 'ngx-editor';
import { ToastrService } from 'ngx-toastr';
import { BaseComponent } from '../base.component';
import { EmailTemplateService } from '../email-template/email-template.service';
import { EmailSendService } from './email-send.service';

@Component({
  selector: 'app-email-send',
  templateUrl: './email-send.component.html',
  styleUrls: ['./email-send.component.scss']
})
export class EmailSendComponent extends BaseComponent implements OnInit {
  emailTamplates: EmailTemplate[] = [];
  selectedEmailTamplate: EmailTemplate;
  emailForm: FormGroup;
  editor: Editor;
  isLoading = false;
  files: any = [];
  fileData: FileInfo[] = [];
  extension: string = '';
  fileType: string = '';
  constructor(
    private fb: FormBuilder,
    private emailTemplateService: EmailTemplateService,
    private toastrService: ToastrService,
    private emailSendService: EmailSendService,
    ) {
    super();
  }

  ngOnInit(): void {
    this.createEmailForm();
    this.editor = new Editor();
    this.getEmailTamplate();
  }

  onTempateChange() {
    this.parameters.clear();
    this.emailForm.patchValue(this.selectedEmailTamplate);
    const regex = /\##(.*?)\##/gi;
    const parameters: Array<string> = this.selectedEmailTamplate.body.match(regex);
    [...new Set(parameters)].forEach(parameter => {
      this.parameters.push(this.newParameter(parameter));
    });
  }

  newParameter(parameter): FormGroup {
    return this.fb.group({
      parameter: [parameter, [Validators.required]],
      value: ['', [Validators.required]]
    })
  }

  get parameters(): FormArray {
    return <FormArray>this.emailForm.get('parameters');
  }

  setParameterValue() {
    const paramters: EmailParameter[] = this.parameters.value;
    let emailBody = this.selectedEmailTamplate.body;
    if (paramters) {
      paramters.forEach(paramter => {
        if (paramter.value) {
          emailBody = emailBody.split(paramter.parameter).join(paramter.value);
        }
      });
      this.emailForm.get('body').setValue(emailBody);
    }
  }

  getEmailTamplate() {
    this.sub$.sink = this.emailTemplateService.getEmailTemplates().subscribe((emailTamplats: EmailTemplate[]) => {
      this.emailTamplates = emailTamplats;
    })
  }

  createEmailForm() {
    this.emailForm = this.fb.group({
      id: [''],
      toAddress: ['', [Validators.required]],
      cCAddress: [''],
      subject: ['', [Validators.required]],
      body: ['', [Validators.required]],
      parameters: this.fb.array([])
    });
  }

  fileBrowseHandler(files: any) {
    for (let file of files) {
      this.files.push(file);
    }
    this.getFileInfo();
  }

  getFileInfo() {
    this.fileData = [];
    for (let i = 0; i < this.files.length; i++) {
      const reader = new FileReader();
      this.extension = this.files[i].name.split('.').pop().toLowerCase();
      this.fileType = this.files[i].type;
      reader.onload = (ev: ProgressEvent<FileReader>) => {
        const fileInfo = new FileInfo();
        fileInfo.src = ev.target.result.toString();
        fileInfo.extension = this.extension;
        fileInfo.name = this.files[i].name;
        fileInfo.fileType = this.fileType;
        this.fileData.push(fileInfo);
      }
      reader.readAsDataURL(this.files[i]);
    };
  }

  sendEmail() {
    if (!this.emailForm.valid) {
      this.emailForm.markAllAsTouched();
      return;
    }
    this.isLoading = true;
    const emailObj = this.emailForm.value;
    emailObj.attechments = this.fileData;
    this.emailSendService.sendEmail(emailObj)
      .subscribe(() => {
        this.toastrService.success('邮件发送成功');
        this.isLoading = false;
        this.clearForm();
      }, () => {
        this.isLoading = false;
      });
  }

  clearForm() {
    this.parameters.clear();
    this.files = [];
    this.selectedEmailTamplate = {
      name: '',
      id: '',
      body: '',
      subject: '',
    };
    this.emailForm.patchValue({
      id: [''],
      toAddress: [''],
      cCAddress: [''],
      subject: ['']
    });
    this.emailForm.get('body').setValue("");

  }

  formatBytes(bytes: number) {
    const sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB']
    if (bytes === 0) return 'n/a'
    const value = Math.floor(Math.log(bytes) / Math.log(1024));
    const i = parseInt(value.toString(), 10)
    if (i === 0) return `${bytes} ${sizes[i]})`
    return `${(bytes / (1024 ** i)).toFixed(1)} ${sizes[i]}`
  }
  onDeleteFile(index: number) {
    this.files.splice(index, 1);
  }

  onFileDropped($event) {
    for (let file of $event) {
      this.files.push(file);
    }
    this.getFileInfo();

  }
}
