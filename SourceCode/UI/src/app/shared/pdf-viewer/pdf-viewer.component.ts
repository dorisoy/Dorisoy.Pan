import { HttpEventType, HttpResponse } from '@angular/common/http';
import { Component, Inject, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { Documents } from '@core/domain-classes/document';
import { DocumentsPreview } from '@core/domain-classes/documents-preview';
import { OVERLAY_PANEL_DATA } from '@shared/overlay-panel/overlay-panel-data';
import { BaseComponent } from 'src/app/base.component';
import { HomeService } from 'src/app/home/home.service';

@Component({
  selector: 'app-pdf-viewer',
  templateUrl: './pdf-viewer.component.html',
  styleUrls: ['./pdf-viewer.component.scss']
})
export class PdfViewerComponent extends BaseComponent implements OnChanges {
  @Input() document: Documents;
  
  constructor(
    private homeService: HomeService) {
    super();
  }
  documentUrl: any = null;
  isLoading: boolean = false;

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['document']) {
      this.getDocument();
    }
  }

  getDocument() {
    this.isLoading = true;
    this.sub$.sink = this.homeService.downloadDocument(this.document)
      .subscribe((event) => {
        if (event.type === HttpEventType.Response) {
          this.isLoading = false;
          this.downloadFile(event);
        }
      }, (err) => {
        this.isLoading = false;
      });
  }

  downloadFile(data: HttpResponse<Blob>) {
    this.documentUrl = new Blob([data.body], { type: data.body.type });
  }
}
