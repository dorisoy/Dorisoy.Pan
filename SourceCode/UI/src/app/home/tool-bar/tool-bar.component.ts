import { HttpErrorResponse, HttpEventType, HttpResponse } from '@angular/common/http';
import { Component, Input, OnInit } from '@angular/core';
import { DocumentFolderId } from '@core/domain-classes/document-folder-id';
import { ToastrService } from 'ngx-toastr';
import { BaseComponent } from 'src/app/base.component';
import { HomeService } from '../home.service';
import { ObservableService } from '../../core/services/observable.service';
import { Folder } from '@core/domain-classes/folder';
import { forkJoin } from 'rxjs';
import { ClonerService } from '@core/services/clone.service';
import { SignalrService } from '@core/services/signalr.service';
import { TreeViewService } from '@core/services/tree-view.service';

@Component({
  selector: 'app-tool-bar',
  templateUrl: './tool-bar.component.html',
  styleUrls: ['./tool-bar.component.scss']
})
export class ToolBarComponent extends BaseComponent implements OnInit {
  documentAndFolderIds: DocumentFolderId[] = [];
  @Input() selectedFolder: Folder;
  isLoading: boolean = false;
  constructor(private observableService: ObservableService,
    private homeService: HomeService,
    private toastrService: ToastrService,
    private cloneService: ClonerService,
    private signalrService: SignalrService,
    private treeViewService: TreeViewService) {
    super();
  }

  ngOnInit(): void {
    this.setIsDownload();
  }

  downloadAll() {
    this.isLoading = true;
    let documentIds = this.documentAndFolderIds.filter(c => c.folderId == '').map(cs => cs.documentId);
    let folderIds = this.documentAndFolderIds.filter(c => c.documentId == '').map(cs => cs.folderId);
    this.homeService.downloadAll(documentIds, folderIds).subscribe((event) => {
      if (event.type === HttpEventType.Response) {
        this.isLoading = false;
        this.downloadFile(event);
        this.observableService.resetDocumentOrFolderId();
      }
    }, (error: HttpErrorResponse) => {
      this.isLoading = false;
      if (error.status == 404) {
        this.toastrService.error('selected folder(s) are empty.');
      } else {
        this.toastrService.error('error while downloading folder');
      }
    });
  }
  deleteAll() {
    this.isLoading = true;
    let deleteFolderOrDocuments = [];
    this.documentAndFolderIds.forEach(element => {
      if (element.documentId) {
        deleteFolderOrDocuments.push(this.homeService.deleteDocument(element.documentId))
      } else if (element.folderId) {
        deleteFolderOrDocuments.push(this.homeService.deleteFolder(element.folderId))
      }
    });
    this.sub$.sink = forkJoin(deleteFolderOrDocuments)
      .subscribe(results => {
        this.isLoading = false;
        this.documentAndFolderIds.forEach(element => {
          if (element.folderId) {
            this.treeViewService.setRemovedFolderId(element.folderId);
          }
        });
        this.observableService.setDocumentAndFolderIdsEmpty();
        this.toastrService.success('Selected files or folders deleted.');
        this.signalrService.setNotification(this.selectedFolder.physicalFolderId);
      }, err => {
        this.isLoading = false;
      });
  }

  download() {
    this.isLoading = true;
    this.sub$.sink = this.homeService.downloadFolder(this.selectedFolder.physicalFolderId)
      .subscribe(
        (event) => {
          if (event.type === HttpEventType.Response) {
            this.isLoading = false;
            this.downloadAllFileAndFoler(event, this.selectedFolder);
          }
        },
        (error: HttpErrorResponse) => {
          this.isLoading = false;
          if (error.status == 404) {
            this.toastrService.error('Folder is Empty');
          } else {
            this.toastrService.error('error while downloading folder');
          }
        }
      );
  }

  private downloadAllFileAndFoler(data: HttpResponse<Blob>, folder: Folder) {
    const downloadedFile = new Blob([data.body], { type: data.body.type });
    const a = document.createElement('a');
    a.setAttribute('style', 'display:none;');
    document.body.appendChild(a);
    a.download = folder.name;
    a.href = URL.createObjectURL(downloadedFile);
    a.target = '_blank';
    a.click();
    document.body.removeChild(a);
  }

  private downloadFile(data: HttpResponse<Blob>) {
    const downloadedFile = new Blob([data.body], { type: data.body.type });
    const a = document.createElement('a');
    a.setAttribute('style', 'display:none;');
    document.body.appendChild(a);
    a.download = 'dorisoy.pan';
    a.href = URL.createObjectURL(downloadedFile);
    a.target = '_blank';
    a.click();
    document.body.removeChild(a);
  }

  setIsDownload() {
    this.sub$.sink = this.observableService.documentAndFolderIds$
      .subscribe(ids => {
        this.documentAndFolderIds = this.cloneService.deepClone<DocumentFolderId[]>(ids);
      });
  }
}
