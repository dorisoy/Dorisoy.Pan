import { HttpEventType } from '@angular/common/http';
import { Component, ElementRef, Inject, Input, OnChanges, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import { Documents } from '@core/domain-classes/document';
import { DocumentsPreview } from '@core/domain-classes/documents-preview';
import { OVERLAY_PANEL_DATA } from '@shared/overlay-panel/overlay-panel-data';
import { OverlayPanelRef } from '@shared/overlay-panel/overlay-panel-ref';
import { BaseComponent } from 'src/app/base.component';
import { HomeService } from 'src/app/home/home.service';

@Component({
  selector: 'app-audio-preview',
  templateUrl: './audio-preview.component.html',
  styleUrls: ['./audio-preview.component.scss']
})
export class AudioPreviewComponent extends BaseComponent implements OnChanges {
  @ViewChild('playerEl', { static: true }) playerEl: ElementRef;
  @Input() document: Documents;
  isLoading = false;
  htmlSource: HTMLSourceElement;
  constructor(
    public homeService: HomeService,
    public overlayRef: OverlayPanelRef) {
    super();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['document']) {
      this.getDocument();
    }
  }

  getDocument() {
    this.isLoading = true;
    this.sub$.sink = this.homeService.downloadDocument(this.document).subscribe(data => {
      if (data.type === HttpEventType.Response) {
        this.isLoading = false;
        if (this.htmlSource && this.player().hasChildNodes()) {
          this.player().removeChild(this.htmlSource);
        }
        const imgageFile = new Blob([data.body], { type: data.body.type });
        this.htmlSource = document.createElement('source');
        this.htmlSource.src = URL.createObjectURL(imgageFile);
        this.htmlSource.type = data.body.type;
        this.player().pause();
        this.player().load();
        this.player().appendChild(this.htmlSource);
        this.player().play();
      }
    }, (err) => {
      this.isLoading = false;
    });
  }

  player() {
    return this.playerEl.nativeElement as HTMLVideoElement | HTMLAudioElement;
  }

  onCancel() {
    this.overlayRef.close();
  }

}
