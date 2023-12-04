import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute } from '@angular/router';
import { CommonDialogService } from '@core/common-dialog/common-dialog.service';
import { Documents } from '@core/domain-classes/document';
import { DocumentShareableLink } from '@core/domain-classes/document-shareable-link';
import { ClonerService } from '@core/services/clone.service';
import { CommonService } from '@core/services/common.service';
import { DocumentBaseComponent } from '@shared/document-base/document-base.component';
import { OverlayPanel } from '@shared/overlay-panel/overlay-panel.service';
import { ToastrService } from 'ngx-toastr';
import { HomeService } from 'src/app/home/home.service';
import { ObservableService } from '@core/services/observable.service';
import { DocumentLinkPreviewPasswordComponent } from './document-link-preview-password/document-link-preview-password.component';

@Component({
  selector: 'app-document-link-preview',
  templateUrl: './document-link-preview.component.html',
  styleUrls: ['./document-link-preview.component.css']
})
export class DocumentLinkPreviewComponent extends DocumentBaseComponent implements OnInit {
  isLinkExpired = false;
  code: string;
  constructor(
    private route: ActivatedRoute,
    public toastrService: ToastrService,
    public homeService: HomeService,
    public commonService: CommonService,
    public overlay: OverlayPanel,
    public dialog: MatDialog,
    public commonDialogService: CommonDialogService,
    public clonerService: ClonerService,
    public observableService: ObservableService,
  ) {
    super(overlay, commonService, homeService, toastrService, dialog,
      commonDialogService, clonerService, observableService);
  }

  ngOnInit(): void {
    this.code = this.route.snapshot.params.code;
    this.getLinkInfo();
    super.rootFolderSubscription();
  }

  getLinkInfo() {
    this.sub$.sink = this.homeService.getLinkInfoByCode(this.code)
      .subscribe((info: DocumentShareableLink) => {
        if (info.isLinkExpired) {
          this.isLinkExpired = true;
        } else if (info.hasPassword) {
          const overlayRef = this.dialog.open(DocumentLinkPreviewPasswordComponent, {
            data: info,
            disableClose: true,
            backdropClass: 'black-background',
            width: '500px'
          });
          this.sub$.sink = overlayRef.afterClosed().subscribe((isTrue: boolean) => {
            if (isTrue) {
              this.getDocument(info);
            }
          })
        } else {
          this.getDocument(info);
        }
      });
  }

  getDocument(info: DocumentShareableLink) {
    this.sub$.sink = this.homeService.getDocumentByLinkId(info.id)
    .subscribe((doc: DocumentShareableLink) => {
      const document: Documents = {
        id: doc.documentId,
        name: doc.documentName,
        physicalFolderId: '',
        extension: doc.extension,
        path: '',
        size: '',
        modifiedDate: new Date(),
        thumbnailPath: '',
        isBackDisabled: true,
        isDownloadEnabled: doc.isAllowDownload,
        token: this.code,
        isFromPreview: true
      }
      this.onDocumentView(document, [], true);
    });
  }
}
