import { AfterViewInit, Component, ElementRef, Inject, Input, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import { Documents } from '@core/domain-classes/document';
import { environment } from '@environments/environment';
import { OverlayPanelRef } from '@shared/overlay-panel/overlay-panel-ref';
import { BaseComponent } from 'src/app/base.component';
import { HomeService } from 'src/app/home/home.service';

@Component({
  selector: 'app-office-viewer',
  templateUrl: './office-viewer.component.html',
  styleUrls: ['./office-viewer.component.scss']
})
export class OfficeViewerComponent extends BaseComponent implements OnInit, AfterViewInit {
  @ViewChild('iframe') iframe: ElementRef<HTMLIFrameElement>;
  @Input() document: Documents;
  isLive = true;
  isLoading: boolean = false;
  token = '';
  constructor(
    private homeService: HomeService,
    private overlayRef: OverlayPanelRef
  ) {
    super();
  }

  ngOnInit(): void {
    if (environment.apiUrl.indexOf('localhost') >= 0) {
      this.isLive = false;
    }
    this.document.isVersion = this.document.isVersion ? this.document.isVersion : false;
  }

  ngAfterViewInit() {
    if (this.isLive) {
      this.getDocumentToken();
    }
  }

  getDocumentToken() {
    this.isLoading = true;
    this.sub$.sink = this.homeService.getDocumentToken(this.document.id, this.document.isVersion).subscribe((token: { [key: string]: string }) => {
      this.token = token['result'];
      const host = location.host;
      const protocal = location.protocol;
      const url = environment.apiUrl === '/' ? `${protocal}//${host}/` : environment.apiUrl;
      this.iframe.nativeElement.src = 'https://view.officeapps.live.com/op/embed.aspx?src=' + encodeURIComponent(`${url}api/document/${this.document.id}/officeviewer?token=${this.token}&isVersion=${this.document.isVersion}`);
      this.isLoading = false;
    }, (err) => {
      this.isLoading = false;
    });
  }

  onCancel() {
    if (this.isLive) {
      this.homeService.deleteDocumentToken(this.token).subscribe(() => { });
    }
    this.overlayRef.close();
  }
}
