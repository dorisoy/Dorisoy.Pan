import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { CommonDialogService } from '@core/common-dialog/common-dialog.service';
import { Documents } from '@core/domain-classes/document';
import { DocumentShareableLink } from '@core/domain-classes/document-shareable-link';
import { ToastrService } from 'ngx-toastr';
import { BaseComponent } from 'src/app/base.component';
import { HomeService } from '../home.service';

@Component({
  selector: 'app-document-shared-link',
  templateUrl: './document-shared-link.component.html',
  styleUrls: ['./document-shared-link.component.css']
})
export class DocumentSharedLinkComponent extends BaseComponent implements OnInit {
  documentLinkForm: FormGroup;
  isEditMode = false;
  isResetLink = false;
  minDate = new Date();
  isLoading = false;
  baseUrl = `${window.location.protocol}//${window.location.host}/preview/`;
  constructor(public dialogRef: MatDialogRef<DocumentSharedLinkComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { document: Documents, link: DocumentShareableLink },
    private fb: FormBuilder,
    private homeService: HomeService,
    private toastrService: ToastrService,
    private commonDialogService: CommonDialogService) {
    super();
  }

  ngOnInit(): void {
    this.createDocumentLinkForm();
    if (this.data.link) {
      if (this.data.link.id) {
        this.isEditMode = true;
        this.pathValue();
      }
    }
  }

  pathValue() {
    this.documentLinkForm.patchValue(this.data.link);
    this.documentLinkForm.get('linkCode').setValue(`${this.baseUrl}${this.data.link.linkCode}`);
    if (this.data.link.linkExpiryTime) {
      this.documentLinkForm.get('isLinkExpiryTime').setValue(true);
    }

    if (this.data.link.password) {
      this.documentLinkForm.get('isPassword').setValue(true);
    }

  }

  createDocumentLinkForm() {
    this.documentLinkForm = this.fb.group({
      id: [''],
      isLinkExpiryTime: new FormControl(false),
      linkExpiryTime: [''],
      isPassword: new FormControl(false),
      password: [''],
      linkCode: [''],
      isAllowDownload: new FormControl(false),
    }, {
      validator: this.checkData
    });
  }

  checkData(group: FormGroup) {
    let isLinkExpiryTime = group.get('isLinkExpiryTime').value;
    let linkExpiryTime = group.get('linkExpiryTime').value;
    let isPassword = group.get('isPassword').value;
    let password = group.get('password').value;
    const data = {};
    if (isLinkExpiryTime && !linkExpiryTime) {
      data['linkExpiryTimeValidator'] = true;
    }
    if (isPassword && !password) {
      data['passwordValidator'] = true;
    }
    return data;
  }

  openLinkSettings() {
    this.isResetLink = !this.isResetLink;
  }

  createLink() {
    if (!this.documentLinkForm.valid) {
      this.documentLinkForm.markAllAsTouched();
      return;
    }
    const link = this.createBuildObject();
    this.isLoading = true;
    this.sub$.sink = this.homeService.createDocumentShareableLink(link).subscribe((data: DocumentShareableLink) => {
      this.toastrService.success('链接生成成功');
      this.data.link = data;
      this.isEditMode = true;
      this.isResetLink = false;
      this.isLoading = false;
      this.pathValue();
    }, () => this.isLoading = false);
  }

  deleteDocumentLink() {
    this.sub$.sink = this.commonDialogService
      .deleteConformationDialog('你确定要删除 the link?')
      .subscribe((isTrue: boolean) => {
        if (isTrue) {
          this.isLoading = true;
          this.sub$.sink = this.homeService.deleteDocumentShareableLInk(this.data.link.id)
            .subscribe(() => {
              this.isLoading = false;
              this.toastrService.success('文档链接删除成功');
              this.dialogRef.close();
            }, () => this.isLoading = false);
        }
      })
  }

  closeDialog() {
    this.dialogRef.close();
  }

  createBuildObject(): DocumentShareableLink {
    const id: string = this.documentLinkForm.get('id').value;
    let linkCode: string = this.documentLinkForm.get('linkCode').value;
    if (linkCode) {
      linkCode = linkCode.replace(this.baseUrl, '');
    }
    const link: DocumentShareableLink = {
      id: id,
      documentId: this.data.document.id,
      isAllowDownload: this.documentLinkForm.get('isAllowDownload').value,
      password: this.documentLinkForm.get('isPassword').value ? this.documentLinkForm.get('password').value : '',
      linkExpiryTime: this.documentLinkForm.get('isLinkExpiryTime').value ? this.documentLinkForm.get('linkExpiryTime').value : '',
      linkCode: linkCode
    }
    return link;
  }
}
