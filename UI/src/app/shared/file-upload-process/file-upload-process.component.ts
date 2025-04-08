import { Component, Inject, OnInit } from '@angular/core';
import { FileProgress } from '@core/domain-classes/file-progress';
import { OVERLAY_PANEL_DATA } from '@shared/overlay-panel/overlay-panel-data';
import { OverlayPanelRef } from '@shared/overlay-panel/overlay-panel-ref';
import { Observable } from 'rxjs';
import { ObservableService } from '@core/services/observable.service';

@Component({
  selector: 'app-file-upload-process',
  templateUrl: './file-upload-process.component.html',
  styleUrls: ['./file-upload-process.component.scss'],
})
export class FileUploadProcessComponent implements OnInit {
  panelOpenState = true;
  files$: Observable<FileProgress[]>;
  constructor(private observableService: ObservableService) {}

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

  ComputCount(files: FileProgress[]) {
    let result = [];
    let md5Count = files.filter((s) => s.isComputeMd5).length;
    if (md5Count > 0) {
      result.push(`${md5Count}个预处理`);
    }
    let processCount = files.filter(
      (s) => !s.isComputeMd5 && s.percentage != 100
    ).length;
    if (processCount > 0) {
      result.push(`${processCount}个上传中`);
    }
    let okCount = files.filter(
      (s) => !s.isComputeMd5 && s.percentage == 100
    ).length;
    if (okCount > 0) {
      result.push(`${okCount}个已完成`);
    }
    return result.join('/');
  }
}
