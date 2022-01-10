import { Component, Input, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { Documents } from '@core/domain-classes/document';
import { Folder } from '@core/domain-classes/folder';
import { HierarchyShared } from '@core/domain-classes/hierarchy-shared';
import { SharedData } from '@core/domain-classes/shared-data';
import { CommonService } from '@core/services/common.service';
import { filter } from 'rxjs/operators';
import { BaseComponent } from 'src/app/base.component';
import { ObservableService } from '@core/services/observable.service';
import { PreventSharedFolderComponent } from 'src/app/home/shared-folder-file/prevent-shared-folder/prevent-shared-folder.component';
import { SharedFolderFileComponent } from 'src/app/home/shared-folder-file/shared-folder-file.component';
import { environment } from '@environments/environment';

@Component({
  selector: 'app-right-sidebar',
  templateUrl: './right-sidebar.component.html',
  styleUrls: ['./right-sidebar.component.scss'],
})
export class RightSidebarComponent extends BaseComponent implements OnInit {
  currentRoute: string = '';
  rootFolder: Folder;
  folderId: string = '';
  length: number = 0;
  selectedFolder: Folder;
  selectedDocument: Documents
  constructor(
    private router: Router,
    private activeRoute: ActivatedRoute,
    private observableService: ObservableService,
    private commonService: CommonService,
    private dialog: MatDialog
  ) {
    super();
  }

  ngOnInit(): void {
    this.rootFolderSubscription();
    this.folderSubscriptionSubscribe();
    this.documentSubscriptionSubscribe();
    this.sub$.sink = this.router.events
      .pipe(filter((event) => event instanceof NavigationEnd))
      .subscribe((event: NavigationEnd) => {
        this.currentRoute = event.url;
        this.length = this.currentRoute.length;
      });
  }

  rootFolderSubscription() {
    this.sub$.sink = this.observableService.rootFolder$.subscribe(folder => {
      this.rootFolder = folder;
    });
  }

  folderSubscriptionSubscribe() {
    this.sub$.sink = this.observableService.selectedFolder$.subscribe(
      (c: Folder) => {
        if (c && c.id !== this.rootFolder.id) {
          c.users.forEach(u => {
            if (u.profilePhoto) {
              u.profilePhoto = `${u.profilePhoto}`;
            }
          })
          this.selectedDocument = null;
          this.selectedFolder = c;
        } else {
          this.selectedDocument = null;
          this.selectedFolder = null;
        }
      }
    );
  }
  documentSubscriptionSubscribe() {
    this.sub$.sink = this.observableService.selectedDocument$.subscribe(
      (c: Documents) => {
        if (c) {
          c.users.forEach(u => {
            if (u.profilePhoto) {
              u.profilePhoto = `${u.profilePhoto}`;
            }
          })
          this.selectedDocument = c;
          this.selectedFolder = null;
        }
      }
    );
  }

  getFolderDetail(id: string) {
    this.sub$.sink = this.commonService
      .getFolderDetailById(id)
      .subscribe((c: Folder) => {
        c.users.forEach(u => {
          if (u.profilePhoto) {
            u.profilePhoto = `${environment.apiUrl}${u.profilePhoto}`;
          }
        });
        this.selectedFolder = c;
      });
  }

  onSharedFolder() {
    if (this.selectedFolder.isShared) {
      this.openSharedFolderFileDialog(this.selectedFolder, null);
    } else {
      this.isParentChildShared(this.selectedFolder, 'Shared');
    }
  }
  onSharedDocument() {
    this.openSharedFolderFileDialog(null, this.selectedDocument);
  }

  isParentChildShared(folder: Folder, operation: string) {
    this.sub$.sink = this.commonService
      .isParentChildShared(folder.id)
      .subscribe((c: HierarchyShared) => {
        c.name = folder.name;
        c.operation = operation;
        if (c.isChildShared || c.isParentShared) {
          this.dialog.open(PreventSharedFolderComponent, {
            panelClass: 'custom-modalbox-450',
            minHeight: '100px',
            data: c,
          });
        } else {
          this.openSharedFolderFileDialog(folder, null);
        }
      });
  }

  openSharedFolderFileDialog(folder: Folder, document: Documents) {
    const sharedDocument: SharedData = {
      type: folder ? 'folder' : 'document',
      document: document,
      folder: folder,
      parentPhysicalFolderId: this.selectedFolder.physicalFolderId,
    };
    this.dialog.open(SharedFolderFileComponent, {
      panelClass: 'custom-modalbox-450',
      minHeight: '350px',
      data: sharedDocument,
    });
  }
}
