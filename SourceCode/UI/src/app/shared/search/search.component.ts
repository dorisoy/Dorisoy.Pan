import { Component, Inject, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { CommonDialogService } from '@core/common-dialog/common-dialog.service';
import { Documents } from '@core/domain-classes/document';
import { Folder } from '@core/domain-classes/folder';
import { ClonerService } from '@core/services/clone.service';
import { CommonService } from '@core/services/common.service';
import { TreeViewService } from '@core/services/tree-view.service';
import { environment } from '@environments/environment';
import { DocumentBaseComponent } from '@shared/document-base/document-base.component';
import { OVERLAY_PANEL_DATA } from '@shared/overlay-panel/overlay-panel-data';
import { OverlayPanelRef } from '@shared/overlay-panel/overlay-panel-ref';
import { OverlayPanel } from '@shared/overlay-panel/overlay-panel.service';
import { ToastrService } from 'ngx-toastr';
import { filter } from 'rxjs/operators';
import { HomeService } from 'src/app/home/home.service';
import { ObservableService } from '@core/services/observable.service';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.scss']
})
export class SearchComponent extends DocumentBaseComponent implements OnInit {
  documents: Documents[] = [];
  folders: Folder[] = [];
  private currentRoute: string;
  isDocumentLoading = false;
  isFolderLoading = false;
  constructor(
    private router: Router,
    private overlayRef: OverlayPanelRef,
    private treeViewService: TreeViewService,
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
    this.sub$.sink = router.events.pipe(filter(event => event instanceof NavigationEnd))
      .subscribe((event: NavigationEnd) => {
        this.currentRoute = event.url;
      });
  }
  ngOnInit(): void {
    super.rootFolderSubscription();
    this.searchStringSubscription();
    this.sub$.sink = this.deleteDocumentEvent.subscribe(c => {
      this.ondeleteDocumentEvent(c);
    });
  }

  onFolderClick(folder: Folder) {
    if (this.overlayRef) {
      this.overlayRef.close();
    }
    this.treeViewService.setSelectedFolder(folder);
    if (this.currentRoute !== '/') {
      this.router.navigate(["/", folder.id]);
    }
  }

  ondeleteDocumentEvent(documentId: string) {
    this.documents = this.documents.filter(
      (c) => c.id != documentId
    );
  }

  searchStringSubscription() {
    this.sub$.sink = this.commonService.searchString$.subscribe(c => {
      if (c) {
        this.searchDocument(c);
        this.searchFolder(c);
      } else {
        this.folders = [];
        this.documents = [];
      }
    });
  }

  searchFolder(searchString) {
    this.isFolderLoading = true;
    this.sub$.sink = this.homeService.searchFolder(searchString).subscribe((folders: Folder[]) => {
      this.isFolderLoading = false;
      this.folders = folders;
    }, () => this.isFolderLoading = false)
  }

  searchDocument(searchString) {
    this.isDocumentLoading = true;
    this.sub$.sink = this.homeService.searchDocument(searchString).subscribe((documents: Documents[]) => {
      this.isDocumentLoading = false;
      documents.forEach((document) => {
        if (document.thumbnailPath) {
          document.thumbnailPath = `${environment.apiUrl}${document.thumbnailPath}`;
        }
      });
      this.documents = documents;
    }, () => this.isDocumentLoading = false)
  }
}
