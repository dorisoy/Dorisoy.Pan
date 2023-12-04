import { CdkDragDrop } from '@angular/cdk/drag-drop';
import {
  HttpErrorResponse,
  HttpEventType,
  HttpResponse,
} from '@angular/common/http';
import {
  AfterViewInit,
  Component,
  EventEmitter,
  Input,
  OnDestroy,
  OnInit,
  Output,
  ViewChild,
  ViewChildren,
} from '@angular/core';
import { MatCheckbox, MatCheckboxChange } from '@angular/material/checkbox';
import { MatDialog } from '@angular/material/dialog';
import { CommonDialogService } from '@core/common-dialog/common-dialog.service';
import { CopyDocument } from '@core/domain-classes/copy-document';
import { Documents } from '@core/domain-classes/document';
import { Folder } from '@core/domain-classes/folder';
import { HierarchyShared } from '@core/domain-classes/hierarchy-shared';
import { MoveFolder } from '@core/domain-classes/move-folder';
import { MoveFolderRoot } from '@core/domain-classes/move-folder-root';
import { UserNotification } from '@core/domain-classes/notification';
import { SharedData } from '@core/domain-classes/shared-data';
import { User } from '@core/domain-classes/user';
import { ClonerService } from '@core/services/clone.service';
import { CommonService } from '@core/services/common.service';
import { TreeViewService } from '@core/services/tree-view.service';
import { ToastrService } from 'ngx-toastr';
import { BaseComponent } from 'src/app/base.component';
import { CopyFolderComponent } from '../copy-folder/copy-folder.component';
import { HomeService } from '../home.service';
import { MoveFolderComponent } from '../move-folder/move-folder.component';
import { ObservableService } from '../../core/services/observable.service';
import { PreventSharedFolderComponent } from '../shared-folder-file/prevent-shared-folder/prevent-shared-folder.component';
import { SharedFolderFileComponent } from '../shared-folder-file/shared-folder-file.component';
import { BreakpointsService } from '@core/services/breakpoints.service';
import { RenameFileFolderComponent } from '@shared/rename-file-folder/rename-file-folder.component';
import { SendFileFolderData } from '@core/domain-classes/send-file-folder';
import { SendMailComponent } from '@shared/send-mail/send-mail.component';
import { fromEvent, interval, of, Subscription } from 'rxjs';
import { MatMenuTrigger } from '@angular/material/menu';
import { delayWhen, tap } from 'rxjs/operators';

@Component({
  selector: 'app-folder-list',
  templateUrl: './folder-list.component.html',
  styleUrls: ['./folder-list.component.scss'],
})
export class FolderListComponent extends BaseComponent implements OnInit, AfterViewInit, OnDestroy {
  overlayRef: any;
  autoScrollDisabled: boolean = false;
  @Input() folders: Folder[] = [];
  @Input() selectedFolder: Folder;
  @Input() listView: string;
  @Output() onFolderClickEvent: EventEmitter<Folder> =
    new EventEmitter<Folder>();
  @Output() deleteFolderEvent: EventEmitter<string> =
    new EventEmitter<string>();
  rootFolder: Folder = null;
  @ViewChildren('checkboxes') public checkboxes: MatCheckbox[];
  totalFileUploaded: number = 0;
  isLoading: boolean = false;
  disabled = false;
  menuSubscription: Subscription;
  windowClickSubscription: Subscription;

  contextMenuPosition = { x: 0, y: 0 };
  @ViewChild('folderMenuTrigger') folderMenuTrigger: MatMenuTrigger;
  constructor(
    private homeService: HomeService,
    private toastrService: ToastrService,
    private dialog: MatDialog,
    private commonDialogService: CommonDialogService,
    private observableService: ObservableService,
    private clonerService: ClonerService,
    private commonService: CommonService,
    private treeViewService: TreeViewService,
    private breakpointsService: BreakpointsService
  ) {
    super();
  }

  ngOnInit(): void {
    this.isMobileOrTabletDevice();
    this.unCheckAllCheckbox();
    this.rootFolderSubscription();
    this.windowSubscription();
    this.documentClickNotification();
  }
  ngAfterViewInit() {
    this.subscribeCheckAll();
  }

  windowSubscription() {
    this.sub$.sink = fromEvent(window, 'click').subscribe((_) => {
      if (this.folderMenuTrigger && this.folderMenuTrigger.menuOpen) {
        this.folderMenuTrigger.closeMenu();
        this.folders.forEach(c => {
          c.isRightClicked = false;
        });
      }
    });
  }
  subscribeCheckAll() {
    this.sub$.sink = this.observableService.checkAll$.subscribe(c => {
      if (this.folders && c) {
        this.folders.forEach(c => {
          this.observableService.setDocumentOrFolderIdAll({
            documentId: '',
            folderId: c.physicalFolderId,
          });
        });
      }
      this.checkboxes.forEach((element) => {
        element.checked = c;
      });
    })
  }

  documentClickNotification() {
    this.sub$.sink = this.observableService.selectedDocument$
      .subscribe(c => {
        if (this.folders && this.folders.length > 0) {
          this.folders.forEach(c => {
            c.isRightClicked = false;
          });
        }
      })
  }

  isMobileOrTabletDevice() {
    this.sub$.sink = this.breakpointsService.isMobile$
      .subscribe(c => {
        if (c) {
          this.disabled = c;
        }
      });
    this.sub$.sink = this.breakpointsService.isTablet$
      .subscribe(c => {
        if (c) {
          this.disabled = c;
        }
      });
  }

  rootFolderSubscription() {
    this.sub$.sink = this.observableService.rootFolder$.subscribe(folder => {
      this.rootFolder = folder;
    });
  }

  public trackById(index: number, entry: Folder): string {
    return entry.id;
  }

  unCheckAllCheckbox() {
    this.sub$.sink = this.observableService.unCheckAllCheckBox.subscribe(() => {
      if (this.checkboxes) {
        this.checkboxes.forEach((element) => {
          element.checked = false;
        });
      }
    });
  }

  onFolderClick(folder: Folder) {
    this.onFolderClickEvent.emit(folder);
  }

  onFolderDetailClick(folder: Folder) {
    this.folders.forEach(c => {
      if (c.id === folder.id) {
        c.isRightClicked = true;
      } else {
        c.isRightClicked = false;
      }
    });
    this.observableService.setSelectedFolder(folder);
  }

  toggleFolderStarred(folder: Folder) {
    this.sub$.sink = this.homeService
      .toggleVirtualFolderStarred(folder.id)
      .subscribe((d) => {
        if (folder.isStarred) {
          this.toastrService.success(`Unstarred ${folder.name}.`);
        } else {
          this.toastrService.success(`Starred ${folder.name}.`);
        }
        folder.isStarred = !folder.isStarred;
      });
  }


  getTooltip(users: User[]) {
    return users.map((c) => `${c.firstName} ${c.lastName}`).join(', ');
  }

  sendEmail(folder: Folder) {
    const emailData: SendFileFolderData = {
      type: 'folder',
      folder: folder
    }
    this.dialog.open(SendMailComponent, {
      panelClass: ['full-width-dialog', 'min-width-dialog'],
      data: emailData,
    });
  }

  onSharedFolder(folder: Folder) {
    if (folder.isShared) {
      this.openSharedFolderFileDialog(folder, null);
    } else {
      this.isParentChildShared(folder, 'Operation');
    }
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
      width: '500px',
      data: sharedDocument,
    });
  }

  renameFolder(folder: Folder) {
    const sharedDocument: SharedData = {
      type: 'folder',
      folder: folder,
      parentPhysicalFolderId: folder.physicalFolderId,
    };
    this.dialog.open(RenameFileFolderComponent, {
      width: '300px',
      data: sharedDocument,
    });
  }

  deleteFolder(folder: Folder) {
    this.sub$.sink = this.commonDialogService
      .deleteConformationDialog(`你确定要删除 ${folder.name}`)
      .subscribe((isTrue: boolean) => {
        if (isTrue) {
          this.sub$.sink = this.homeService
            .deleteFolder(folder.id)
            .subscribe(() => {
              this.toastrService.success('文件夹删除成功');
              this.deleteFolderEvent.emit(folder.id);
              this.treeViewService.setRemovedFolder(folder);
            });
        }
      });
  }

  onMovedFolder(folder: Folder) {
    this.checkSharedFolder(folder).then((folder: Folder) => {
      this.moveFolder(folder);
    });
  }

  sendNotification(folder: Folder) {
    if (folder.isShared) {
      const notification: UserNotification = {
        folderId: folder.physicalFolderId
      };
      this.sub$.sink = this.commonService.sendNotification(notification)
        .subscribe(c => {
        });
    }
  }

  drop(event: CdkDragDrop<Folder | Documents>) {
    if (event.previousContainer === event.container) {
    } else {
      if (event.previousContainer.id.indexOf('document') >= 0) {
        this.isLoading = true;
        var moveDocumement: CopyDocument = {
          documentId: event.previousContainer.data.id,
          destinationFolderId: event.container.data.id
        }
        this.sub$.sink = this.homeService.moveDocument(moveDocumement)
          .subscribe((c: boolean) => {
            this.isLoading = false;
            this.sendNotification(event.container.data);
            this.commonService.setMoveDocumentNotification(event.previousContainer.data.id);
            this.toastrService.success('文档移动成功');
          }, (err) => {
            this.isLoading = false;
          });
      } else {
        const source = event.previousContainer.data;
        const container = event.container.data;
        if (source.id === container.id) {
          return;
        }
        this.checkSharedFolder(source).then((folder: Folder) => {
          this.isLoading = true;
          var moveFoler: MoveFolder = {
            sourceId: folder.id,
            distinationParentId: container.id
          }
          this.sub$.sink = this.homeService.moveFolder(moveFoler)
            .subscribe((c: boolean) => {
              const folder1 = this.folders.find(c => c.id == folder.id);
              const cloneFolder = this.clonerService.deepClone<Folder>(folder1)
              this.isLoading = false;
              this.folders = this.folders.filter(c => c.id !== source.id);
              this.treeViewService.setRemovedFolder(folder);
              const distinationParentId = container.id;
              cloneFolder.parentId = distinationParentId;
              this.treeViewService.setRefreshTreeView(cloneFolder);
              this.toastrService.success('文件夹移动成功');
            }, (err) => {
              this.isLoading = false;
            });
        });
      }
    }
  }

  checkSharedFolder(folder: Folder) {
    return new Promise((resolve, reject) => {
      if (folder.isShared) {
        const hierarchyShared: HierarchyShared = {
          id: folder.id,
          name: folder.name,
          isParentShared: false,
          isChildShared: false,
          isFolderShared: true,
          operation: 'Move',
          isFolder: true
        };
        this.dialog.open(PreventSharedFolderComponent, {
          panelClass: 'custom-modalbox-450',
          minHeight: '100px',
          data: hierarchyShared,
        });
        reject();
      } else {
        this.isLoading = true;
        this.sub$.sink = this.commonService
          .isParentChildShared(folder.id)
          .subscribe((c: HierarchyShared) => {
            this.isLoading = false;
            c.name = folder.name;
            c.isFolderShared = false;
            c.operation = "Move";
            c.isFolder = true;
            if (c.isChildShared || c.isParentShared) {
              this.dialog.open(PreventSharedFolderComponent, {
                panelClass: 'custom-modalbox-450',
                minHeight: '100px',
                data: c,
              });
              reject();
            } else {
              resolve(folder);
            }
          }, (err) => {
            this.isLoading = false;
          });
      }
    })
  }

  moveFolder(folder: Folder) {
    const moveFolderRoot: MoveFolderRoot = {
      sourceId: folder.id,
      sourceParentId: folder.parentId,
      root: this.rootFolder,
      sourceName: folder.name
    };
    const dialogRef = this.dialog.open(MoveFolderComponent, {
      panelClass: 'custom-modalbox',
      height: this.disabled ? 'calc(100vh - 50px)' : 'calc(100vh - 100px)',
      data: moveFolderRoot,
    });
    this.sub$.sink = dialogRef.afterClosed().subscribe((result: any) => {
      if (result['flag']) {
        const folder1 = this.folders.find(c => c.id == result['sourceId']);
        const cloneFolder = this.clonerService.deepClone<Folder>(folder1)
        this.folders = this.folders.filter((c) => c.id != result['sourceId']);
        this.treeViewService.setRemovedFolder(folder);
        const distinationParentId = result['distinationParentId'];
        cloneFolder.parentId = distinationParentId;
        this.treeViewService.setRefreshTreeView(cloneFolder);
        this.toastrService.success('文件夹移动成功')
      }
    });
  }

  onCopyFolder(folder: Folder) {
    const moveFolderRoot: MoveFolderRoot = {
      sourceId: folder.id,
      sourceParentId: folder.parentId,
      root: this.rootFolder,
      sourceName: folder.name
    };
    const dialogRef = this.dialog.open(CopyFolderComponent, {
      panelClass: 'custom-modalbox',
      height: this.disabled ? 'calc(100vh - 50px)' : 'calc(100vh - 100px)',
      data: moveFolderRoot,
    });
    this.sub$.sink = dialogRef
      .afterClosed()
      .subscribe((result: { [key: string]: boolean }) => {
        if (result['flag']) {
          this.toastrService.success('文件夹拷贝成功')
        }
      });
  }

  addOrRemoveFolderId(folderId: string, value: MatCheckboxChange) {
    this.observableService.setDocumentOrFolderId({
      documentId: '',
      folderId: folderId,
    });
    if (!value.checked) {
      this.observableService.mainCheckBox$.next(value.checked);
    }
  }

  downloadFolder(folder: Folder) {
    this.sub$.sink = this.homeService.downloadFolder(folder.physicalFolderId).subscribe(
      (event) => {
        if (event.type === HttpEventType.Response) {
          this.downloadFile(event, folder);
        }
      },
      (error: HttpErrorResponse) => {
        if (error.status == 404) {
          this.toastrService.error('Folder is Empty');
        } else {
          this.toastrService.error('error while downloading folder');
        }
      }
    );
  }

  private downloadFile(data: HttpResponse<Blob>, folder: Folder) {
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

  ngOnDestroy() {
    super.ngOnDestroy();
  }

  onContextMenu(event: MouseEvent, folder: Folder) {
    event.preventDefault();
    this.folders.map(c => {
      c.isRightClicked = false;
      return c;
    });
    folder.isRightClicked = true;
    this.observableService.setSelectedFolder(folder);
    this.sub$.sink = of(1)
      .pipe(
        tap(() => {
          if (this.folderMenuTrigger.menuOpen) {
            this.folderMenuTrigger.closeMenu();
          }
          this.contextMenuPosition.x = event.clientX;
          this.contextMenuPosition.y = event.clientY;
          this.folderMenuTrigger.menuData = { folder: folder, type: 'folder' };
          this.folderMenuTrigger.openMenu();
        })
      )
      .subscribe();
  }

  delay(delayInms: number) {
    return new Promise((resolve) => {
      setTimeout(() => {
        resolve(2);
      }, delayInms);
    });
  }
}
