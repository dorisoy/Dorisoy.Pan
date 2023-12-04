import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { SharedData } from '@core/domain-classes/shared-data';
import { ToastrService } from 'ngx-toastr';
import { BaseComponent } from 'src/app/base.component';
import { HomeService } from 'src/app/home/home.service';

@Component({
  selector: 'app-rename-file-folder',
  templateUrl: './rename-file-folder.component.html',
  styleUrls: ['./rename-file-folder.component.scss']
})
export class RenameFileFolderComponent extends BaseComponent implements OnInit {
  folderFileForm: FormGroup;
  isLoading = false;
  constructor(private fb: FormBuilder,
    @Inject(MAT_DIALOG_DATA) public data: SharedData,
    public dialogRef: MatDialogRef<RenameFileFolderComponent>,
    private homeService: HomeService,
    private toastrService: ToastrService) {
    super();
  }

  ngOnInit(): void {
    let name = this.data.type == 'file' ? this.data.document.name : this.data.folder.name;
    this.createFolderForm(name)
  }

  createFolderForm(name: string) {
    this.folderFileForm = this.fb.group({
      name: [name, [Validators.required]],
    })
  }

  onCancel() {
    this.dialogRef.close();
  }

  save() {
    if (this.folderFileForm.invalid) {
      this.folderFileForm.markAllAsTouched();
      return;
    }
    if (this.data.type == 'file') {
      this.renameFile();
    } else {
      this.renameFolder();
    }
  }

  renameFolder() {
    this.isLoading = true;
    const name = this.folderFileForm.get('name').value;
    this.sub$.sink = this.homeService.renameFolder(this.data.folder.physicalFolderId, name)
      .subscribe(() => {
        this.isLoading = false;
        this.data.folder.name = name;
        this.toastrService.success('重命名成功')
        this.dialogRef.close();
      }, () => this.isLoading = false);
  }

  renameFile() {
    this.isLoading = true;
    const name = this.folderFileForm.get('name').value;
    this.sub$.sink = this.homeService.renameDocument(this.data.document.id, name)
      .subscribe(() => {
        this.isLoading = false;
        this.data.document.name = name;
        this.toastrService.success('重命名成功')
        this.dialogRef.close();
      }, () => this.isLoading = false);
  }

}
