import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { Router } from '@angular/router';
import { CommonDialogService } from '@core/common-dialog/common-dialog.service';
import { Documents } from '@core/domain-classes/document';
import { Folder } from '@core/domain-classes/folder';
import { ResponseHeader } from '@core/domain-classes/response-header';
import { Notification } from '@core/domain-classes/user-notification';
import { NotificationResource } from '@core/domain-classes/user-notification-source';
import { ClonerService } from '@core/services/clone.service';
import { CommonService } from '@core/services/common.service';
import { ObservableService } from '@core/services/observable.service';
import { SignalrService } from '@core/services/signalr.service';
import { TreeViewService } from '@core/services/tree-view.service';
import { DocumentBaseComponent } from '@shared/document-base/document-base.component';
import { OverlayPanel } from '@shared/overlay-panel/overlay-panel.service';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { BaseComponent } from 'src/app/base.component';
import { HomeService } from 'src/app/home/home.service';
import { NotificationDataSource } from './notification-datasource';

@Component({
  selector: 'app-notification-list',
  templateUrl: './notification-list.component.html',
  styleUrls: ['./notification-list.component.scss']
})
export class NotificationListComponent extends DocumentBaseComponent implements OnInit, AfterViewInit {
  notificationSource: NotificationDataSource;
  displayedColumns: string[] = ['id', 'action'];
  footerToDisplayed: string[] = ["footer"];
  notificationResource: NotificationResource;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  constructor(
    public toastrService: ToastrService,
    public homeService: HomeService,
    public commonService: CommonService,
    public overlay: OverlayPanel,
    public dialog: MatDialog,
    public commonDialogService: CommonDialogService,
    public clonerService: ClonerService,
    public observableService: ObservableService,
    private treeViewService: TreeViewService,
    private router: Router,
    private signalrService: SignalrService) {

    super(overlay, commonService, homeService, toastrService, dialog,
      commonDialogService, clonerService, observableService);

    this.notificationResource = new NotificationResource();
    this.notificationResource.pageSize = 10;
    this.notificationResource.orderBy = 'createdDate desc'
  }

  ngOnInit(): void {
    this.loadNotifications();
    this.folderNotificationSubscription();
  }

  ngAfterViewInit(): void {
    this.sub$.sink = this.paginator.page
      .pipe(
        tap((c: any) => {
          this.notificationResource.skip = this.paginator.pageIndex * this.paginator.pageSize;
          this.notificationResource.pageSize = this.paginator.pageSize;
          this.notificationSource.loadNotifications(this.notificationResource);
        })
      ).subscribe();
  }

  loadNotifications() {
    this.notificationSource = new NotificationDataSource(this.commonService);
    this.notificationSource.loadNotifications(this.notificationResource);
    this.getResourceParameter();
  }

  getResourceParameter() {
    this.sub$.sink = this.notificationSource.responseHeaderSubject$
      .subscribe((c: ResponseHeader) => {
        if (c) {
          this.notificationResource.pageSize = c.pageSize;
          this.notificationResource.skip = c.skip;
          this.notificationResource.totalCount = c.totalCount;
        }
      });
  }

  onNotificationClick(notification: Notification) {
    this.markAsReadNotification(notification);
    if (notification.folderId) {
      this.getFolderDetail(notification.folderId);
    } else {
      const document: Documents = {
        id: notification.documentId,
        name: notification.documentName,
        physicalFolderId: '',
        extension: notification.extension,
        path: '',
        size: '',
        modifiedDate: new Date(),
        thumbnailPath: notification.documentThumbnail,
        isVersion: false,
        isDownloadEnabled: false
      }
      this.onDocumentView(document)
    }
  }

  markAsReadNotification(notification: Notification) {
    this.sub$.sink = this.commonService.markAsReadNotification(notification.id).subscribe(() => {
      notification.isRead = true;
    }, () => { });
  }

  getFolderDetail(folderId) {
    this.sub$.sink = this.commonService.getFolderDetailById(folderId)
      .subscribe((c: Folder) => {
        this.treeViewService.setSelectedFolder(c);
        this.router.navigate(["/", c.id]);
      });
  }

  folderNotificationSubscription() {
    this.sub$.sink = this.signalrService.folderNotification$
      .subscribe(() => {
        this.loadNotifications();
      });
  }
}

