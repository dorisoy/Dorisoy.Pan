import { HttpEventType, HttpResponse } from '@angular/common/http';
import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { CommonDialogService } from '@core/common-dialog/common-dialog.service';
import { Documents } from '@core/domain-classes/document';
import { DocumentVersion } from '@core/domain-classes/document-version';
import { DocumentsPreview } from '@core/domain-classes/documents-preview';
import { BasePreviewComponent } from '@shared/base-preview/base-preview.component';
import { OverlayPanel } from '@shared/overlay-panel/overlay-panel.service';
import { ToastrService } from 'ngx-toastr';
import { BaseComponent } from 'src/app/base.component';
import { HomeService } from 'src/app/home/home.service';

@Component({
  selector: 'app-document-version-history',
  templateUrl: './document-version-history.component.html',
  styleUrls: ['./document-version-history.component.scss']
})
export class DocumentVersionHistoryComponent extends BaseComponent implements OnInit {
  documentVersions: DocumentVersion[] = [];
  constructor(@Inject(MAT_DIALOG_DATA) public data: Documents,
    private overlay: OverlayPanel,
    public dialogRef: MatDialogRef<DocumentVersionHistoryComponent>,
    private homeService: HomeService,
    private toastrService: ToastrService,
    private commandDialogService: CommonDialogService
  ) {
    super();
  }

  ngOnInit(): void {
  }

  closeDialog() {
    this.dialogRef.close();
  }

  onDocumentView(version: DocumentVersion) {
    this.data.id = version.id;
    this.data.isVersion = true;
    const documentsPreview: DocumentsPreview = {
      document: this.data,
      otherDocuments: []
    }
    this.overlay.open(BasePreviewComponent, {
      position: 'center',
      origin: 'global',
      panelClass: ['file-preview-overlay-container', 'light-black-background'],
      data: documentsPreview,
    });
  }

  restoreDocumentVersion(version: DocumentVersion) {
    this.sub$.sink = this.commandDialogService.deleteConformationDialog("你确定要还原版本吗？")
      .subscribe((isTrue) => {
        if (isTrue) {
          this.sub$.sink = this.homeService.restoreDocumentVersion(this.data.id, version.id)
            .subscribe(() => {
              this.toastrService.success('版本还原成功')
              this.dialogRef.close();
            })
        }
      })
  }

  downloadDocument(version: DocumentVersion) {
    var doc: Documents = {
      id: version.id,
      isVersion: true,
      name: '',
      physicalFolderId: '',
      extension: '',
      path: '',
      size: '',
      modifiedDate: new Date(),
      thumbnailPath: '',
      isFromPreview: false
    }
    this.sub$.sink = this.homeService.downloadDocument(doc).subscribe(
      (event) => {
        if (event.type === HttpEventType.Response) {
          this.downloadFile(event);
        }
      },
      (error) => {
        this.toastrService.error('error while downloading document');
      }
    );
  }

  private downloadFile(data: HttpResponse<Blob>) {
    const downloadedFile = new Blob([data.body], { type: data.body.type });
    const a = document.createElement('a');
    a.setAttribute('style', 'display:none;');
    document.body.appendChild(a);
    a.download = this.data.name;
    a.href = URL.createObjectURL(downloadedFile);
    a.target = '_blank';
    a.click();
    document.body.removeChild(a);
  }
}
