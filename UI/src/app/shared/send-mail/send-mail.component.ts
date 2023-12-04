import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Documents } from '@core/domain-classes/document';
import { SendFileFolderData } from '@core/domain-classes/send-file-folder';
import { CommonService } from '@core/services/common.service';
import { Editor } from 'ngx-editor';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-send-mail',
  templateUrl: './send-mail.component.html',
  styleUrls: ['./send-mail.component.scss']
})
export class SendMailComponent implements OnInit {
  emailForm: FormGroup;
  isLoading = false;
  fileName = '';
  editor: Editor;
  constructor(private fb: FormBuilder,
    public dialogRef: MatDialogRef<SendMailComponent>,
    @Inject(MAT_DIALOG_DATA) public data: SendFileFolderData,
    private commonService: CommonService,
    private toastrService: ToastrService) { }

  ngOnInit(): void {
    this.editor = new Editor();
    this.fileName = this.data.type == 'file' ? this.data.document.name : this.data.folder.name;
    this.createEmailForm();
  }

  createEmailForm() {
    this.emailForm = this.fb.group({
      id: this.data.type == 'file' ? this.data.document.id : this.data.folder.physicalFolderId,
      toAddress: ['', [Validators.required]],
      cCAddress: [''],
      subject: ['', [Validators.required]],
      body: ['', [Validators.required]]
    });
  }

  sendEmail() {
    if (!this.emailForm.valid) {
      this.emailForm.markAllAsTouched();
      return;
    }
    this.isLoading = true;
    const emailObj = this.emailForm.value;
    if (this.data.type == 'file') {
      this.commonService.sendDocumentEmail(emailObj)
        .subscribe(() => {
          this.toastrService.success('邮件发送成功');
          this.isLoading = false;
          this.closeDialog();
        }, (err) => {
          this.isLoading = false;
          this.toastrService.error(`发送电子邮件时出错: ${err.message}`);
        });
    } else {
      this.commonService.sendFolderEmail(emailObj)
        .subscribe(() => {
          this.toastrService.success('邮件发送成功');
          this.isLoading = false;
          this.closeDialog();
        }, (err) => {
          this.isLoading = false;
          this.toastrService.error(`发送电子邮件时出错: ${err.message}`);
        });
    }
  }

  closeDialog() {
    this.dialogRef.close();
  }

}
