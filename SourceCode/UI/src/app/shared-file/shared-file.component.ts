import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { CommonDialogService } from '@core/common-dialog/common-dialog.service';
import { Documents } from '@core/domain-classes/document';
import { Folder } from '@core/domain-classes/folder';
import { ClonerService } from '@core/services/clone.service';
import { CommonService } from '@core/services/common.service';
import { SignalrService } from '@core/services/signalr.service';
import { TreeViewService } from '@core/services/tree-view.service';
import { environment } from '@environments/environment';
import { DocumentBaseComponent } from '@shared/document-base/document-base.component';
import { OverlayPanel } from '@shared/overlay-panel/overlay-panel.service';
import { ToastrService } from 'ngx-toastr';
import { HomeService } from '../home/home.service';
import { ObservableService } from '../core/services/observable.service';
import { SharedFileService } from './shared-file.service';

@Component({
  selector: 'app-shared-file',
  templateUrl: './shared-file.component.html',
  styleUrls: ['./shared-file.component.scss']
})
export class SharedFileComponent extends DocumentBaseComponent implements OnInit {
  documents: Documents[] = [];
  folders: Folder[] = [];
  isDocumentLoading = false;
  isFolderLoading = false;
  constructor(
    private sharedFileService: SharedFileService,
    private signalrService: SignalrService,
    private treeViewService: TreeViewService,
    private router: Router,
    public toastrService: ToastrService,
    public homeService: HomeService,
    public commonService: CommonService,
    public overlay: OverlayPanel,
    public dialog: MatDialog,
    public commonDialogService: CommonDialogService,
    public clonerService: ClonerService,
    public observableService: ObservableService
  ) {
    super(overlay, commonService, homeService, toastrService, dialog,
      commonDialogService, clonerService, observableService);
  }

  ngOnInit(): void {
    this.folderNotificationSubscription();
    super.rootFolderSubscription();
    this.getSharedDocuments();
    this.getSharedFolders();
    this.sub$.sink = this.deleteDocumentEvent.subscribe(c => {
      this.ondeleteDocumentEvent(c);
    });
  }

  ondeleteDocumentEvent(documentId: string) {
    this.documents = this.documents.filter(
      (c) => c.id != documentId
    );
  }

  getSharedFolders() {
    this.isFolderLoading = true;
    this.sub$.sink = this.sharedFileService.getSharedFolders()
    .subscribe((folders: Folder[]) => {
      this.isFolderLoading = false;
      this.folders = folders;
    }, () => this.isFolderLoading = false);
  }

  onFolderClick(folder: Folder) {
    this.treeViewService.setSelectedFolder(folder);
    this.router.navigate(["/", folder.id]);
  }

  folderNotificationSubscription() {
    this.sub$.sink = this.signalrService.folderNotification$
      .subscribe(() => {
        this.getSharedDocuments();
        this.getSharedFolders();
      });
  }

  getSharedDocuments() {
    this.isDocumentLoading = true;
    this.sub$.sink = this.sharedFileService.getSharedDocuments()
    .subscribe((docs: Documents[]) => {
      this.isDocumentLoading = false;
      docs.forEach(document => {
        if (document.thumbnailPath) {
          document.thumbnailPath = `${environment.apiUrl}${document.thumbnailPath}`
        }
        if (document.users) {
          document.users = document.users.map(u => {
            if (u.profilePhoto) {
              u.profilePhoto = `${environment.apiUrl}${u.profilePhoto}`
            }
            return u;
          });
        }
      });
      this.documents = docs;
    }, () => this.isDocumentLoading = false);
  }
}
