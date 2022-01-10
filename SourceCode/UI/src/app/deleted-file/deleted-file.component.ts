import { Component, OnInit } from '@angular/core';
import { CommonDialogService } from '@core/common-dialog/common-dialog.service';
import { Documents } from '@core/domain-classes/document';
import { Folder } from '@core/domain-classes/folder';
import { TreeViewService } from '@core/services/tree-view.service';
import { environment } from '@environments/environment';
import { ToastrService } from 'ngx-toastr';
import { BaseComponent } from '../base.component';
import { DeletedFileService } from './deleted-file.service';

@Component({
  selector: 'app-deleted-file',
  templateUrl: './deleted-file.component.html',
  styleUrls: ['./deleted-file.component.scss']
})
export class DeletedFileComponent extends BaseComponent implements OnInit {
  folders: Folder[] = [];
  documents: Documents[] = [];
  isDocumentLoading = false;
  isFolderLoading = false;
  constructor(private deletedFileService: DeletedFileService,
    private commonDialogService: CommonDialogService,
    private toastrService: ToastrService
  ) {
    super();
  }

  ngOnInit(): void {
    this.getDeletedFolders();
    this.getDeletedDocuments();
  }

  getDeletedFolders() {
    this.isFolderLoading = true;
    this.sub$.sink = this.deletedFileService.getDeletedFolders().subscribe((folders: Folder[]) => {
      this.isFolderLoading = false;
      this.folders = folders;
    }, () => this.isFolderLoading = false)
  }

  getDeletedDocuments() {
    this.isFolderLoading = true;
    this.sub$.sink = this.deletedFileService.getDeletedDocuments().subscribe((documents: Documents[]) => {
      this.isFolderLoading = false;
      documents.forEach(document => {
        if (document.thumbnailPath) {
          document.thumbnailPath = `${environment.apiUrl}${document.thumbnailPath}`
        }
      });
      this.documents = documents;
    }, () => this.isFolderLoading = false)
  }

  restoreDeletedFolder(id: string, name: string) {
    this.sub$.sink = this.commonDialogService
      .deleteConformationDialog(`您确定要还原吗 ${name}`)
      .subscribe((isTrue: boolean) => {
        if (isTrue) {
          this.isFolderLoading = true;
          this.sub$.sink = this.deletedFileService.restoreDeletedFolder(id).subscribe(() => {
            this.isFolderLoading = false;
            this.folders = this.folders.filter(c => c.id != id);
            this.toastrService.success('文件夹还原成功')
          }, () => this.isFolderLoading = false);
        }
      });
  }

  deleteFolder(id: string, name: string) {
    this.sub$.sink = this.commonDialogService
      .deleteConformationDialog(`你确定要永久删除 ${name} 吗？`)
      .subscribe((isTrue: boolean) => {
        if (isTrue) {
          this.isFolderLoading = true;
          this.sub$.sink = this.deletedFileService.deleteFolder(id).subscribe(() => {
            this.isFolderLoading = false;
            this.folders = this.folders.filter(c => c.id != id);
            this.toastrService.success('文件夹永久删除成功')
          }, () => this.isFolderLoading = false);
        }
      })
  }

  restoreDeletedDocument(id: string, name: string) {
    this.sub$.sink = this.commonDialogService
      .deleteConformationDialog(`您确定要还原${name}吗？`)
      .subscribe((isTrue: boolean) => {
        if (isTrue) {
          this.isDocumentLoading = true;
          this.sub$.sink = this.deletedFileService.restoreDeletedDocument(id).subscribe(() => {
            this.isDocumentLoading = false;
            this.documents = this.documents.filter(c => c.id != id);
            this.toastrService.success('文件还原成功.')
          }, () => this.isDocumentLoading = false);
        }
      });
  }

  deleteDocument(id: string, name: string) {
    this.sub$.sink = this.commonDialogService
      .deleteConformationDialog(`你确定要永久删除 ${name}吗？`)
      .subscribe((isTrue: boolean) => {
        if (isTrue) {
          this.isDocumentLoading = true;
          this.sub$.sink = this.deletedFileService.deleteDocument(id).subscribe(() => {
            this.isDocumentLoading = false;
            this.documents = this.documents.filter(c => c.id != id);
            this.toastrService.success('文件永久删除成功')
          }, () => this.isDocumentLoading = false);
        }
      })
  }
  removedAll() {
    this.sub$.sink = this.commonDialogService
      .deleteConformationDialog(`你确定要永久删除文件夹和文件吗？`)
      .subscribe((isTrue: boolean) => {
        if (isTrue) {
          this.isFolderLoading = true;
          this.sub$.sink = this.deletedFileService.deleteFoldersAndDocuments()
            .subscribe((flag: boolean) => {
              this.isFolderLoading = false;
              if (flag) {
                this.getDeletedFolders();
                this.getDeletedDocuments();
              }
            },
              () => this.isFolderLoading = false)
        }
      });
  }
}
