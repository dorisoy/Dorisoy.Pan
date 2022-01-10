import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Documents } from '@core/domain-classes/document';
import { DocumentComment } from '@core/domain-classes/document-comment';
import { ToastrService } from 'ngx-toastr';
import { BaseComponent } from 'src/app/base.component';
import { HomeService } from '../home.service';

@Component({
  selector: 'app-document-comment',
  templateUrl: './document-comment.component.html',
  styleUrls: ['./document-comment.component.scss']
})
export class DocumentCommentComponent extends BaseComponent implements OnInit {
  commentForm: FormGroup;

  constructor(private fb: FormBuilder,
    @Inject(MAT_DIALOG_DATA) public data: Documents,
    public dialogRef: MatDialogRef<DocumentCommentComponent>,
    private homeService: HomeService,
    private toastrService: ToastrService) {
    super();
  }

  ngOnInit(): void {
    this.createFolderForm();
  }

  createFolderForm() {
    this.commentForm = this.fb.group({
      comment: ['', [Validators.required]],
      documentId: [this.data.id],
    })
  }

  closeDialog() {
    this.dialogRef.close();
  }

  addComment() {
    if (this.commentForm.invalid) {
      this.toastrService.error('Please enter the message.');
      return;
    }
    this.sub$.sink = this.homeService.addComment(this.commentForm.value)
      .subscribe((comment: DocumentComment) => {
        this.commentForm.controls.comment.setValue('');
        this.data.documentComments.push(comment)
      })
  }
}
