import { Component, Inject, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { Documents } from '@core/domain-classes/document';
import { DocumentsPreview } from '@core/domain-classes/documents-preview';
import { OVERLAY_PANEL_DATA } from '@shared/overlay-panel/overlay-panel-data';
import { OverlayPanelRef } from '@shared/overlay-panel/overlay-panel-ref';
import { BaseComponent } from 'src/app/base.component';
import { HomeService } from 'src/app/home/home.service';

@Component({
  selector: 'app-text-preview',
  templateUrl: './text-preview.component.html',
  styleUrls: ['./text-preview.component.scss']
})
export class TextPreviewComponent extends BaseComponent implements OnChanges {
  textLines: string[] = [];
  isLoading = false;
  @Input() document: Documents;
  constructor(
    private homeService: HomeService,
    private overlayRef: OverlayPanelRef) {
    super();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['document']) {
      this.readDocument();
    }
  }

  readDocument() {
    this.isLoading = true;
    this.sub$.sink = this.homeService.readDocument(this.document).subscribe((data: { [key: string]: string[] }) => {
      this.isLoading = false;
      this.textLines = data['result'];
    }, (err) => {
      this.isLoading = false;
    });
  }

  onCancel() {
    this.overlayRef.close();
  }

}
