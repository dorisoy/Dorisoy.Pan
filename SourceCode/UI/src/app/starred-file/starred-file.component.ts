import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { CommonDialogService } from '@core/common-dialog/common-dialog.service';
import { Documents } from '@core/domain-classes/document';
import { Folder } from '@core/domain-classes/folder';
import { ClonerService } from '@core/services/clone.service';
import { CommonService } from '@core/services/common.service';
import { TreeViewService } from '@core/services/tree-view.service';
import { environment } from '@environments/environment';
import { DocumentBaseComponent } from '@shared/document-base/document-base.component';
import { OverlayPanel } from '@shared/overlay-panel/overlay-panel.service';
import { ToastrService } from 'ngx-toastr';
import { HomeService } from '../home/home.service';
import { ObservableService } from '../core/services/observable.service';
import { StarredFileService } from './starred-file.service';

@Component({
  selector: 'app-starred-file',
  templateUrl: './starred-file.component.html',
  styleUrls: ['./starred-file.component.scss']
})
export class StarredFileComponent extends DocumentBaseComponent implements OnInit {
  documents: Documents[] = [];
  folders: Folder[] = [];
  isDocumentLoading = false;
  isFolderLoading = false;
  constructor(
    private treeViewService: TreeViewService,
    private router: Router,
    private starredFileService: StarredFileService,
    public overlay: OverlayPanel,
    public commonService: CommonService,
    public homeService: HomeService,
    public toastrService: ToastrService,
    public dialog: MatDialog,
    public commonDialogService: CommonDialogService,
    public clonerService: ClonerService,
    public observableService: ObservableService) {
    super(overlay, commonService, homeService, toastrService, dialog, commonDialogService, clonerService, observableService)
  }

  ngOnInit(): void {
    super.rootFolderSubscription();
    this.getStarredDocuments();
    this.getStarredFolders();
  }

  getStarredDocuments() {
    this.isDocumentLoading = true;
    this.sub$.sink = this.starredFileService.getStarredDocuments()
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

  getStarredFolders() {
    this.isFolderLoading = true;
    this.sub$.sink = this.starredFileService.getStarredFolders().subscribe((folders: Folder[]) => {
      this.isFolderLoading = false;
      this.folders = folders;
    }, () => { this.isFolderLoading = false });
  }

  onFolderClick(folder: Folder) {
    this.treeViewService.setSelectedFolder(folder);
    this.router.navigate(["/", folder.id]);
  }

  removeDocumentStarred(document: Documents) {
    this.sub$.sink = this.homeService.toggleDocumentStarred(document.id).subscribe(() => {
      this.toastrService.success(`Unstarred ${document.name}.`);
      this.documents = this.documents.filter(d => d.id != document.id);
    });
  }

  removeFolderStarred(folder: Folder) {
    this.sub$.sink = this.homeService.toggleVirtualFolderStarred(folder.id).subscribe(() => {
      this.toastrService.success(`Unstarred ${folder.name}.`);
      this.folders = this.folders.filter(d => d.id != folder.id);
    });
  }

}
