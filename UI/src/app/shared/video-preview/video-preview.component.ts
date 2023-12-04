import { Component, Inject, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { Documents } from '@core/domain-classes/document';
import { DocumentsPreview } from '@core/domain-classes/documents-preview';
import { AudioPreviewComponent } from '@shared/audio-preview/audio-preview.component';
import { OVERLAY_PANEL_DATA } from '@shared/overlay-panel/overlay-panel-data';
import { OverlayPanelRef } from '@shared/overlay-panel/overlay-panel-ref';
import { HomeService } from 'src/app/home/home.service';

@Component({
  selector: 'app-video-preview',
  templateUrl: './video-preview.component.html',
  styleUrls: ['./video-preview.component.scss']
})
export class VideoPreviewComponent extends AudioPreviewComponent implements OnChanges {
  @Input() document: Documents;
  constructor(public homeService: HomeService,
    public overlayRef: OverlayPanelRef) {
    super(homeService, overlayRef);
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['document']) {
      this.getDocument();
    }
  }

}
