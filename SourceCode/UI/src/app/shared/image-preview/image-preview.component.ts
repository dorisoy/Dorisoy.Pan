import { HttpEventType } from '@angular/common/http';
import { ChangeDetectorRef, Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { Documents } from '@core/domain-classes/document';
import { OverlayPanelRef } from '@shared/overlay-panel/overlay-panel-ref';
import { delay } from 'rxjs/operators';
import { BaseComponent } from 'src/app/base.component';
import { HomeService } from 'src/app/home/home.service';

@Component({
  selector: 'app-image-preview',
  templateUrl: './image-preview.component.html',
  styleUrls: ['./image-preview.component.scss']
})
export class ImagePreviewComponent extends BaseComponent implements OnInit, OnChanges {
  imageUrl: SafeUrl;
  isLoading = false;
  @Input() document: Documents;
  constructor(
    private overlayRef: OverlayPanelRef,
    private homeServie: HomeService,
    private sanitizer: DomSanitizer,
    private ref: ChangeDetectorRef) {
    super();
  }

  ngOnInit(): void {
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['document']) {
      this.getImage();
    }
  }

  getImage() {
    this.isLoading = true;
    this.sub$.sink = this.homeServie.downloadDocument(this.document)
      .pipe(
        delay(500)
      )
      .subscribe(data => {
        this.isLoading = false;
        if (data.type === HttpEventType.Response) {
          const imgageFile = new Blob([data.body], { type: data.body.type });
          this.imageUrl = this.sanitizer.bypassSecurityTrustUrl(URL.createObjectURL(imgageFile));
          this.ref.markForCheck();
        }
      }, (err) => {
        this.isLoading = false;
      })
  }

  onCancel() {
    this.overlayRef.close();
  }

}
