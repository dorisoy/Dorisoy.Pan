import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { DocumentShareableLink } from '@core/domain-classes/document-shareable-link';
import { OVERLAY_PANEL_DATA } from '@shared/overlay-panel/overlay-panel-data';
import { OverlayPanelRef } from '@shared/overlay-panel/overlay-panel-ref';
import { BaseComponent } from 'src/app/base.component';
import { HomeService } from 'src/app/home/home.service';

@Component({
  selector: 'app-document-link-preview-password',
  templateUrl: './document-link-preview-password.component.html',
  styleUrls: ['./document-link-preview-password.component.css']
})
export class DocumentLinkPreviewPasswordComponent extends BaseComponent implements OnInit {
  documentLinkForm: FormGroup;
  isPasswordInvalid = false;
  constructor(
    private dialogRef: MatDialogRef<DocumentLinkPreviewPasswordComponent>,
    @Inject(MAT_DIALOG_DATA) public data: DocumentShareableLink,
    private homeService: HomeService,
    private fb: FormBuilder) {
    super();
  }

  ngOnInit(): void {
    this.createDocumentLinkForm();
  }

  createDocumentLinkForm() {
    this.documentLinkForm = this.fb.group({
      password: ['', [Validators.required]],
    });
  }

  checkPassword() {
    if (this.documentLinkForm.valid) {
      const password = this.documentLinkForm.get('password').value;
      this.sub$.sink = this.homeService.checkLinkPassword(this.data.id, password)
        .subscribe((isTrue: boolean) => {
          if (isTrue) {
            this.dialogRef.close(true);
            this.isPasswordInvalid = false;
          } else {
            this.isPasswordInvalid = true;
          }
        })
    } else {
      this.documentLinkForm.markAllAsTouched();
    }
  }
}
