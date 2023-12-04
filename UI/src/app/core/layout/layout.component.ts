import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, NavigationStart, Router } from '@angular/router';
import { Folder } from '@core/domain-classes/folder';
import { CommonService } from '@core/services/common.service';
import { SignalrService } from '@core/services/signalr.service';
import { TreeViewService } from '@core/services/tree-view.service';
import { BaseComponent } from 'src/app/base.component';

@Component({
  selector: 'app-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.scss']
})
export class LayoutComponent extends BaseComponent implements OnInit {
  rootFolder: Folder = null;
  isShowRightSide: boolean = false;
  urls: Array<string> = [
    '/deleted-files',
    '/recents',
    '/starred-files',
    '/shared-files',
    '/deleted-files',
    '/my-profile',
    'my-profile',
    '/notifications',
    '/admin',
    '/admin/logs',
    '/admin/login-audit',
    '/admin/email-template'
  ]
  constructor(
    private activeRoute: ActivatedRoute,
    private signalrService: SignalrService,
    private commonService: CommonService,
    private treeViewService: TreeViewService,
    private router: Router
  ) {
    super();
    this.getCurrentRouterurl();
  }
  getCurrentRouterurl() {
    this.sub$.sink = this.router.events
      .subscribe(
        (event: any) => {
          if (event instanceof NavigationStart) {
            if (event.url.length !=37) {
              this.isShowRightSide = false;
            } else {
              this.isShowRightSide = true;
            }
          }
        });
  }

  ngOnInit(): void {
    this.getFromResolveParam();
    this.getFolderNotificationSubscription();
    this.getRemovedFolderNotificationSubscription();
  }

  getFolderNotificationSubscription() {
    this.sub$.sink = this.signalrService.folderNotification$
      .subscribe((c: string) => {
        if (c) {
          this.getFolder(c);
        }
      });
  }

  getRemovedFolderNotificationSubscription() {
    this.sub$.sink = this.signalrService.removedFolderNotification$
      .subscribe((c: string) => {
        if (c) {
          this.treeViewService.setRemovedFolderId(c);
        }
      });
  }
  getFolder(id: string) {
    this.sub$.sink = this.commonService.getFolderDetailById(id)
      .subscribe((c: Folder) => {
        this.treeViewService.setRefreshTreeView(c);
      });
  }


  getFromResolveParam() {
    this.sub$.sink = this.activeRoute.data.subscribe(
      (data: { folder: Folder }) => {
        if (data.folder) {
          this.rootFolder = data.folder;
        }
      });
  }
}
