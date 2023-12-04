import { Component, Inject, OnInit } from '@angular/core';
import { Documents } from '@core/domain-classes/document';
import { DocumentMoveDelete } from '@core/domain-classes/document-move-delete';
import { DocumentsPreview } from '@core/domain-classes/documents-preview';
import { RecentActivity, RecentActivityType } from '@core/domain-classes/recent-activity';
import { ClonerService } from '@core/services/clone.service';
import { CommonService } from '@core/services/common.service';
import { environment } from '@environments/environment';
import { OVERLAY_PANEL_DATA } from '@shared/overlay-panel/overlay-panel-data';
import { OverlayPanel } from '@shared/overlay-panel/overlay-panel.service';
import { BaseComponent } from 'src/app/base.component';

@Component({
  selector: 'app-base-preview',
  templateUrl: './base-preview.component.html',
  styleUrls: ['./base-preview.component.scss']
})
export class BasePreviewComponent extends BaseComponent implements OnInit {
  type = '';
  currentDoc: Documents;
  isLoading: boolean= false;

  constructor(public overlay: OverlayPanel,
    private commonService: CommonService,
    @Inject(OVERLAY_PANEL_DATA) public data: DocumentsPreview,
    private clonerService: ClonerService) {
    super();
  }

  ngOnInit(): void {
    this.onDocumentView(this.data.document);
  }

  onDocumentView(document: Documents) {
    this.currentDoc = this.clonerService.deepClone<Documents>(document);
    if (document.extension.toLowerCase() === '.pdf') {
      this.type = 'pdf';
    } else if (
      environment.officeDocumentExtensions.indexOf(
        document.extension.toLowerCase()
      ) >= 0
    ) {
      this.type = 'office-preview';
    } else if (
      environment.imageExtensions.indexOf(document.extension.toLowerCase()) >= 0
    ) {
      this.type = 'image';
    } else if (document.extension.toLowerCase() == '.txt') {
      this.type = 'text';
    } else if (
      environment.audioFileExtension.indexOf(
        document.extension.toLowerCase()
      ) >= 0
    ) {
      this.type = 'audio';
    } else if (
      environment.videoFileExtension.indexOf(
        document.extension.toLowerCase()
      ) >= 0
    ) {
      this.type = 'video';
    } else {
      this.type = 'no-preview';
    }
  }

  addRecentActivity(documents: Documents) {
    const recentActivity: RecentActivity = {
      folderId: null,
      documentId: documents.id,
      action: RecentActivityType.VIEWED,
    };
    this.sub$.sink = this.commonService
      .addRecentActivity(recentActivity)
      .subscribe((c) => { });
  }
  onMoveDeleteItem(documentMoveDelete: DocumentMoveDelete) {

  }

}
