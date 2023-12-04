import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Folder } from '@core/domain-classes/folder';
import { RecentActivity, RecentActivityType } from '@core/domain-classes/recent-activity';
import { CommonService } from '@core/services/common.service';
import { ToastrService } from 'ngx-toastr';
import { BaseComponent } from 'src/app/base.component';
import { HomeService } from '../home.service';

@Component({
  selector: 'app-add-folder',
  templateUrl: './add-folder.component.html',
  styleUrls: ['./add-folder.component.scss']
})
export class AddFolderComponent extends BaseComponent implements OnInit {
  folderForm: FormGroup;
  isLoading = false;
  constructor(
    private fb: FormBuilder,
    @Inject(MAT_DIALOG_DATA) public data: Folder,
    public dialogRef: MatDialogRef<AddFolderComponent>,
    private homeService: HomeService,
    private toastrService: ToastrService,
    private commonService: CommonService) {
    super();
  }

  ngOnInit(): void {
    this.createFolderForm();
  }

  createFolderForm() {
    this.folderForm = this.fb.group({
      name: ['', [Validators.required]],
      virtualParentId: [this.data.id == 'Root' ? null : this.data.id],
      physicalFolderId: [this.data.physicalFolderId]
    })
  }

  closeDialog() {
    this.dialogRef.close();
  }

  addFolder() {
    if (this.folderForm.invalid) {
      this.folderForm.markAllAsTouched();
      return;
    }
    this.isLoading = true;
    this.sub$.sink = this.homeService.createFolder(this.folderForm.value)
      .subscribe((folder: Folder) => {
        this.isLoading = false;
        this.addRecentActivity(folder);
        if (folder?.isRestore) {
          this.toastrService.success('文件夹位于跟踪文件中，已成功还原')
        } else {
          this.toastrService.success('文件夹保存成功')
        }
        this.dialogRef.close(folder)
      }, () => this.isLoading = false)
  }
  addRecentActivity(folder: Folder) {
    const recentActivity: RecentActivity = {
      folderId: folder.id,
      documentId: null,
      action: RecentActivityType.CREATED
    };
    this.sub$.sink = this.commonService.addRecentActivity(recentActivity)
      .subscribe(c => {
      });
  }
}
