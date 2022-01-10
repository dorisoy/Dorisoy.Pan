import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonDialogService } from '@core/common-dialog/common-dialog.service';
import { Documents } from '@core/domain-classes/document';
import { Folder } from '@core/domain-classes/folder';
import { RecentActivity } from '@core/domain-classes/recent-activity';
import { User } from '@core/domain-classes/user';
import { ClonerService } from '@core/services/clone.service';
import { CommonService } from '@core/services/common.service';
import { TreeViewService } from '@core/services/tree-view.service';
import { environment } from '@environments/environment';
import { DocumentBaseComponent } from '@shared/document-base/document-base.component';
import { OverlayPanel } from '@shared/overlay-panel/overlay-panel.service';
import { ToastrService } from 'ngx-toastr';
import { HomeService } from '../home/home.service';
import { ObservableService } from '../core/services/observable.service';

@Component({
  selector: 'app-recent-activity',
  templateUrl: './recent-activity.component.html',
  styleUrls: ['./recent-activity.component.scss']
})
export class RecentActivityComponent extends DocumentBaseComponent implements OnInit {
  isLoading = false;
  mainUrl = environment.apiUrl;
  recentActivities: RecentActivity[] = [];
  constructor(
    public overlay: OverlayPanel,
    public commonService: CommonService,
    public homeService: HomeService,
    public toastrService: ToastrService,
    public dialog: MatDialog,
    public commonDialogService: CommonDialogService,
    public clonerService: ClonerService,
    public observableService: ObservableService,
    private activeRoute: ActivatedRoute,
    private router: Router,
    private treeViewService: TreeViewService
  ) {
    super(
      overlay,
      commonService,
      homeService,
      toastrService,
      dialog,
      commonDialogService,
      clonerService,
      observableService
    );
  }

  ngOnInit(): void {
    super.rootFolderSubscription();
    this.getRecentActivities();
  }

  getTooltip(users: User[]) {
    return users
      .map((c) => `${c.firstName} ${c.lastName}`)
      .join(', ');
  }

  onFolderClick(folder: RecentActivity) {
    this.getFolderDetail(folder);
  }

  getFolderDetail(folder: RecentActivity) {
    this.sub$.sink = this.commonService.getFolderDetailById(folder.folderId)
      .subscribe((c: Folder) => {
        this.treeViewService.setSelectedFolder(c);
        this.router.navigate(["/", c.id]);
      });
  }

  getRecentActivities() {
    this.isLoading = true;
    this.sub$.sink = this.commonService.getRecentActivities()
      .subscribe((c: RecentActivity[]) => {
        this.isLoading = false;
        this.recentActivities = c;
      }, () => this.isLoading = false);
  }
  onShared1(folder: RecentActivity, type: string) {

  }

  onOperationCommand(recentActivity: RecentActivity, command: string) {
    const document: Documents = {
      id: recentActivity.documentId,
      name: recentActivity.name,
      physicalFolderId: recentActivity.document ? recentActivity.document.physicalFolderId : null,
      extension: recentActivity.document ? recentActivity.document.extension : null,
      modifiedDate: null,
      path: null,
      size: null,
      thumbnailPath: null,
      users: recentActivity.users,
      documentComments: null,
      documentVersions: null
    };
    switch (command) {
      case 'share': {
        this.onShared(document, 'file');
        break;
      }
      case 'delete': {
        this.deleteDocument(document);
        break;
      }
      case 'history': {
        this.onVersionHistoryClick(document);
        break;
      }
      case 'comments': {
        this.onCommentClick(document);
        break;
      }
      case 'download': {
        this.downloadDocument(document);
        break;
      }
    }
  }

}
