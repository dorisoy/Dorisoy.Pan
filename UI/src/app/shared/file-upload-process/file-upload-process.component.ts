import { Component, Inject, OnInit } from '@angular/core';
import { FileProgress } from '@core/domain-classes/file-progress';
import { OVERLAY_PANEL_DATA } from '@shared/overlay-panel/overlay-panel-data';
import { OverlayPanelRef } from '@shared/overlay-panel/overlay-panel-ref';
import { Observable } from 'rxjs';
import { ObservableService } from '@core/services/observable.service';

@Component({
  selector: 'app-file-upload-process',
  templateUrl: './file-upload-process.component.html',
  styleUrls: ['./file-upload-process.component.scss']
})
export class FileUploadProcessComponent implements OnInit {
  panelOpenState = true;
  files$: Observable<FileProgress[]>;
  constructor(private observableService: ObservableService) { }

  ngOnInit(): void {
    this.files$ = this.observableService.documentUploadProgress$;
  }

  onClose() {
    if (this.observableService.progressBarOverlay) {
      this.observableService.resetDocumentUploadProcess();
      this.observableService.progressBarOverlay.close();
      this.observableService.progressBarOverlay = null;
    }
  }

}
