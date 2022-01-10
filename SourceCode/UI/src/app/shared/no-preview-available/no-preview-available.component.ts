import { HttpEventType, HttpResponse } from '@angular/common/http';
import { Component, Inject, Input, OnInit } from '@angular/core';
import { Documents } from '@core/domain-classes/document';
import { DocumentsPreview } from '@core/domain-classes/documents-preview';
import { OVERLAY_PANEL_DATA } from '@shared/overlay-panel/overlay-panel-data';
import { ToastrService } from 'ngx-toastr';
import { BaseComponent } from 'src/app/base.component';
import { HomeService } from 'src/app/home/home.service';

@Component({
  selector: 'app-no-preview-available',
  templateUrl: './no-preview-available.component.html',
  styleUrls: ['./no-preview-available.component.scss']
})
export class NoPreviewAvailableComponent extends BaseComponent implements OnInit {
  @Input() document: Documents;
  constructor(private homeService: HomeService,
    private toastrService: ToastrService) {
    super();
  }

  ngOnInit(): void {
  }

  download() {
    this.sub$.sink = this.homeService.downloadDocument(this.document).subscribe(
      (event) => {
        if (event.type === HttpEventType.Response) {
          this.downloadFile(event, this.document);
        }
      },
      (error) => {
        this.toastrService.error('error while downloading document');
      }
    );
  }

  private downloadFile(data: HttpResponse<Blob>, doc: Documents) {
    const downloadedFile = new Blob([data.body], { type: data.body.type });
    const a = document.createElement('a');
    a.setAttribute('style', 'display:none;');
    document.body.appendChild(a);
    a.download = doc.name;
    a.href = URL.createObjectURL(downloadedFile);
    a.target = '_blank';
    a.click();
    document.body.removeChild(a);
  }

}
