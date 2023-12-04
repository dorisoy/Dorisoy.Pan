import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { CommonDialogService } from '@core/common-dialog/common-dialog.service';
import { Documents } from '@core/domain-classes/document';
import { Folder } from '@core/domain-classes/folder';
import { UserAuth } from '@core/domain-classes/user-auth';
import { Notification } from '@core/domain-classes/user-notification';
import { SecurityService } from '@core/security/security.service';
import { BreakpointsService } from '@core/services/breakpoints.service';
import { ClonerService } from '@core/services/clone.service';
import { CommonService } from '@core/services/common.service';
import { ObservableService } from '@core/services/observable.service';
import { SignalrService } from '@core/services/signalr.service';
import { TreeViewService } from '@core/services/tree-view.service';
import { environment } from '@environments/environment';
import { DocumentBaseComponent } from '@shared/document-base/document-base.component';
import { OverlayPanelRef } from '@shared/overlay-panel/overlay-panel-ref';
import { OverlayPanel } from '@shared/overlay-panel/overlay-panel.service';
import { SearchComponent } from '@shared/search/search.component';
import { ToastrService } from 'ngx-toastr';
import { of } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap } from 'rxjs/operators';
import { HomeService } from 'src/app/home/home.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent extends DocumentBaseComponent implements OnInit, AfterViewInit {
  appUserAuth: UserAuth = null;
  isSearchBoxVisible = true;
  overlayRef: OverlayPanelRef<SearchComponent, any>;
  notifications: Notification[] = [];
  @ViewChild('search') searchElement: ElementRef;
  totalNotifications = 0;
  searchCtl: FormControl = new FormControl('');
  isMobile: boolean = false;
  oldToogle: string = '';

  constructor(
    public toastrService: ToastrService,
    public homeService: HomeService,
    public commonService: CommonService,
    public overlay: OverlayPanel,
    public dialog: MatDialog,
    public commonDialogService: CommonDialogService,
    public clonerService: ClonerService,
    public observableService: ObservableService,
    private router: Router,
    private securityService: SecurityService,
    private signalrService: SignalrService,
    private treeViewService: TreeViewService,
    private breakpointsService: BreakpointsService) {
    super(overlay, commonService, homeService, toastrService, dialog,
      commonDialogService, clonerService, observableService);
  }

  ngOnInit(): void {
    this.setTopLogAndName();
    this.searchCtlSubscription();
    this.isMobileOrTabletDevice();
  }

  ngAfterViewInit(): void {
    this.getUserNotification();
    this.getUserNotificationCount();
    this.folderNotificationSubscription();

  }

  isMobileOrTabletDevice() {
    this.sub$.sink = this.breakpointsService.isMobile$
      .subscribe(c => {
        if (c) {
          this.isMobile = c;
        }
      });
    this.sub$.sink = this.breakpointsService.isTablet$
      .subscribe(c => {
        if (c) {
          this.isMobile = c;
        }
      });
  }

  getUserNotification() {
    this.sub$.sink = this.observableService.toggle$.subscribe(c => {
      if (c && this.isMobile && this.oldToogle != c) {
        this.oldToogle = c;
        this.toggleSideBar();
      }
    });
    this.sub$.sink = this.commonService.getNewNotifications().subscribe((notifications: Notification[]) => {
      this.notifications = notifications;
      this.notifications.forEach(n => {
        if (n.documentThumbnail) {
          n.documentThumbnail = `${environment.apiUrl}${n.documentThumbnail}`
        }
      })
    });
  }

  folderNotificationSubscription() {
    this.sub$.sink = this.signalrService.folderNotification$
      .subscribe(() => {
        this.getUserNotification();
        this.getUserNotificationCount();
      });
  }

  getUserNotificationCount() {
    this.sub$.sink = this.commonService.getUserNotificationCount().subscribe((count: number) => this.totalNotifications = count);
  }

  onNotificationClick(notification: Notification) {
    this.sub$.sink = this.commonService.markAsReadNotification(notification.id).subscribe(() => {
      this.notifications = this.notifications.filter(n => n.id != notification.id);
      this.totalNotifications--;
    }, () => { });
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

  getFolderDetail(folderId) {
    this.sub$.sink = this.commonService.getFolderDetailById(folderId)
      .subscribe((c: Folder) => {
        this.treeViewService.setSelectedFolder(c);
        this.router.navigate(["/", c.id]);
      });
  }

  searchCtlSubscription() {
    this.sub$.sink = this.searchCtl.valueChanges.pipe(
      debounceTime(500),
      distinctUntilChanged(),
      switchMap(c => {
        this.commonService.searchString$.next(c);
        return c ? c : of('');
      })
    ).subscribe();
  }

  setTopLogAndName() {
    this.sub$.sink = this.securityService.securityObject$.subscribe(c => {
      if (c) {
        this.appUserAuth = c;
      }
    })
  }

  onLogout(): void {
    this.signalrService.logout(this.appUserAuth.id);
    this.securityService.logout();
    this.router.navigate(['/login']);
  }

  toggleSideBar() {
    document.body.classList.toggle('sb-sidenav-toggled');
  }

  onSearchClick() {
    this.isSearchBoxVisible = false;
    setTimeout(() => {
      const input: HTMLInputElement = this.searchElement.nativeElement as HTMLInputElement;
      input.focus();
      input.select();
    }, 100);
    this.overlayRef = this.overlay.open(SearchComponent, {
      origin: 'global',
      panelClass: ['file-preview-overlay-container', 'white-background'],
      position: { left: '250px', top: '60px' },
      hasBackdrop: false
    });

    this.sub$.sink = this.overlayRef.overlayRef.outsidePointerEvents().subscribe(data => {
      this.overlayRef.close();
    });

    this.sub$.sink = this.overlayRef.afterClosed().subscribe(() => {
      this.searchCtl.setValue('');
      this.isSearchBoxVisible = true;
    })
  }

  onSearchCancel() {
    this.overlayRef.close();
  }
}
