import { ChangeDetectorRef, Component, OnDestroy, OnInit, Output, EventEmitter } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { Folder } from '@core/domain-classes/folder';
import { Documents } from '@core/domain-classes/document';
import { ClonerService } from '@core/services/clone.service';
import { BaseComponent } from '../base.component';
import { AddFolderComponent } from './add-folder/add-folder.component';
import { HomeService } from './home.service';
import { environment } from '@environments/environment';
import { ObservableService } from '../core/services/observable.service';
import { TreeViewService } from '@core/services/tree-view.service';
import { SignalrService } from '@core/services/signalr.service';
import { UserNotification } from '@core/domain-classes/notification';
import { CommonService } from '@core/services/common.service';
import { Sort } from '@angular/material/sort';
import { HttpEventType } from '@angular/common/http';
import { FileUploadProcessComponent } from '@shared/file-upload-process/file-upload-process.component';
import { OverlayPanel } from '@shared/overlay-panel/overlay-panel.service';
import { ToastrService } from 'ngx-toastr';
import { RecentActivity, RecentActivityType } from '@core/domain-classes/recent-activity';
import { MatCheckboxChange } from '@angular/material/checkbox';
import { ComputeMD5, ChunkFile } from '@core/utils/file-helper';
import { Chunk, Porgress } from '@core/core.types';

@Component({
    selector: 'app-home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.scss']
})
export class HomeComponent extends BaseComponent implements OnInit, OnDestroy {
    selectedFolder: Folder;
    rootFolder: Folder;
    listView = 'list';
    isFolderLoading = false;
    isDocumentLoading = false;
    totalFileUploaded: number;
    isCheck: boolean = false;
    constructor(private homeService: HomeService, private activeRoute: ActivatedRoute, private clonerService: ClonerService, private dialog: MatDialog, private observableService: ObservableService, private treeViewService: TreeViewService, private signalrService: SignalrService, private commonService: CommonService, private router: Router, private overlay: OverlayPanel, private toastrService: ToastrService, private cd: ChangeDetectorRef) {
        super();
    }
    ngOnInit(): void {
        this.listView = this.commonService.getListView();
        this.folderNotificationSubscription();
        this.rootFolderSubscription();
        this.checkAllSubscription();
    }
    onAllChecked(value: MatCheckboxChange) {
        this.isCheck = value.checked;
        if (this.isCheck) {
            this.observableService.setCheckAll(value.checked);
        } else {
            this.observableService.removeAll();
        }
    }
    checkAllSubscription() {
        this.sub$.sink = this.observableService.mainCheckBox$.subscribe(c => {
            this.isCheck = c;
        });
    }

    rootFolderSubscription() {
        this.sub$.sink = this.observableService.rootFolder$.subscribe(folder => {
            this.rootFolder = folder;
            this.getFolderParam();
        });
    }

    getFolderParam() {
        this.sub$.sink = this.activeRoute.paramMap.pipe().subscribe((c: Params) => {
            const id = c.get('id');
            if (id) {
                this.getFolderDetailById(id);
            }
        });
    }

    getFolderDetailById(id: string) {
        if (this.rootFolder.id != id) {
            this.sub$.sink = this.commonService.getFolderDetailById(id).subscribe((c: Folder) => {
                if (c) {
                    c.users.forEach(u => {
                        if (u.profilePhoto) {
                            u.profilePhoto = `${environment.apiUrl}${u.profilePhoto}`;
                        }
                    });
                    this.observableService.setSelectedFolder(c);
                    c.children = [];
                    this.getFoldersAndDocuments(c);
                } else {
                    this.router.navigate(['/', this.rootFolder.id]);
                }
            });
        } else {
            this.getFoldersAndDocuments(this.rootFolder);
            this.observableService.setSelectedFolder(this.rootFolder);
        }
    }

    sortData(sort: Sort) {
        if (!sort.active || sort.direction === '') {
            return;
        }
        this.selectedFolder.children = this.selectedFolder.children.sort((a, b) => {
            const isAsc = sort.direction === 'asc';
            switch (sort.active) {
                case 'name':
                    return this.compare(a.name, b.name, isAsc);
                case 'createdDate':
                    return this.compare(a.createdDate, b.createdDate, isAsc);
                default:
                    return 0;
            }
        });

        this.selectedFolder.documents = this.selectedFolder.documents.sort((a, b) => {
            const isAsc = sort.direction === 'asc';
            switch (sort.active) {
                case 'name':
                    return this.compare(a.name, b.name, isAsc);
                case 'createdDate':
                    return this.compare(a.modifiedDate, b.modifiedDate, isAsc);
                default:
                    return 0;
            }
        });
    }

    compare(a: number | string | Date, b: number | string | Date, isAsc: boolean) {
        return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
    }

    setListView(view: string) {
        this.commonService.setListView(view);
        this.listView = view;
    }

    folderNotificationSubscription() {
        this.sub$.sink = this.signalrService.folderNotification$.subscribe(c => {
            if (c && this.selectedFolder && c === this.selectedFolder.physicalFolderId) {
                this.getChildFolders(this.selectedFolder.id);
                this.getDocuments(this.selectedFolder.id);
            }
        });
    }

    getTreeviewFolderDataSubscription() {
        this.sub$.sink = this.treeViewService.selectedFolder$.subscribe(c => {
            if (c && c.id) {
                this.selectedFolder = this.clonerService.deepClone<Folder>(c);
                this.observableService.setFolderPath(c.id);
                this.getChildFolders(c.id);
                this.getDocuments(c.id);
            }
        });
    }

    getFoldersAndDocuments(parent: Folder) {
        this.selectedFolder = this.clonerService.deepClone<Folder>(parent);
        this.observableService.setFolderPath(parent.id);
        this.observableService.resetDocumentOrFolderId();
        this.getChildFolders(parent.id);
        this.getDocuments(parent.id);
    }

    onFolderClickEvent(parent: Folder) {
        this.router.navigate(['/', parent.id]);
    }

    deleteFolderEvent(folderId: string) {
        this.selectedFolder.children = this.selectedFolder.children.filter(c => c.id != folderId);
    }

    deleteDocumentEvent(documentId: string) {
        this.selectedFolder.documents = this.selectedFolder.documents.filter(c => c.id != documentId);
    }

    getChildFolders(parentId: string) {
        this.isFolderLoading = true;
        this.sub$.sink = this.homeService.getFolders(parentId).subscribe(
            (childs: Folder[]) => {
                childs.forEach(child => {
                    if (child.users) {
                        child.users = child.users.map(u => {
                            if (u.profilePhoto) {
                                u.profilePhoto = `${environment.apiUrl}${u.profilePhoto}`;
                            }
                            return u;
                        });
                    }
                });
                this.selectedFolder.children = childs;
                this.isFolderLoading = false;
            },
            () => (this.isFolderLoading = false)
        );
    }

    getDocuments(folderId: string) {
        this.isDocumentLoading = true;
        this.sub$.sink = this.homeService.getDocumentsByFolderId(folderId).subscribe(
            (documents: Documents[]) => {
                this.isDocumentLoading = false;
                documents.forEach(document => {
                    if (document.thumbnailPath) {
                        document.thumbnailPath = `${environment.apiUrl}${document.thumbnailPath}`;
                    }
                    if (document.users) {
                        document.users = document.users.map(u => {
                            if (u.profilePhoto) {
                                u.profilePhoto = `${environment.apiUrl}${u.profilePhoto}`;
                            }
                            return u;
                        });
                    }
                });
                this.selectedFolder.documents = documents;
            },
            () => (this.isDocumentLoading = false)
        );
    }

    addFolder(): void {
        const dialogRef = this.dialog.open(AddFolderComponent, {
            width: '300px',
            data: Object.assign({}, this.selectedFolder)
        });
        this.sub$.sink = dialogRef.afterClosed().subscribe((result: Folder) => {
            if (result) {
                this.treeViewService.setRefreshTreeView(result);
                this.sendNotification();
                this.onFolderClickEvent(result);
            }
        });
    }
    sendNotification() {
        if (this.selectedFolder.isShared) {
            const notification: UserNotification = {
                folderId: this.selectedFolder.physicalFolderId
            };
            this.sub$.sink = this.commonService.sendNotification(notification).subscribe(c => {});
        }
    }

    onFolderUploadEvent(folder: Folder) {
        if (folder) {
            const existingFolder = this.selectedFolder.children.find(c => c.id == folder.id);
            if (!existingFolder) {
                this.selectedFolder.children.push(folder);
            }
        }
    }

    onDoucmentUploadEvent(document: Documents) {
        if (document.thumbnailPath) {
            document.thumbnailPath = `${environment.apiUrl}${document.thumbnailPath}`;
        }
        let existingFolder = this.selectedFolder.documents.find(c => c.id == document.id);
        if (existingFolder) {
            this.selectedFolder.documents = this.selectedFolder.documents.map(doc => {
                return doc.id == document.id ? document : doc;
            });
        } else {
            this.selectedFolder.documents.push(document);
        }
    }

    onCopyDocumentEvent(folderId: string) {
        this.getDocuments(this.selectedFolder.id);
    }

    async uploadFile(file: File, fileCount: number) {
        this.observableService.initializeDocumentUploadProcess(file.name);
        let md5 = await ComputeMD5(file, info => {
            this.cd.markForCheck();
            this.observableService.upadteDocumentUploadProgress(file.name, info.percent, false, true);
        });

        let chunk = (info: Chunk): Promise<boolean> => {
            return new Promise(resolve => {
                let errorFunc = () => {
                    this.observableService.upadteDocumentUploadProgress(file.name, 100, true);
                    if (info.current == info.total) {
                        this.totalFileUploaded = this.totalFileUploaded + 1;
                        if (this.totalFileUploaded == fileCount) {
                            this.sendNotification();
                        }
                    }
                };
                const formData = new FormData();
                formData.append(file.name, info.file);
                // this.observableService.initializeDocumentUploadProcess(file.name);
                this.sub$.sink = this.commonService.uploadFolderDocument(formData, this.selectedFolder.physicalFolderId, info.current, info.total, md5, info.size).subscribe(
                    event => {
                        if (event.type === HttpEventType.UploadProgress) {
                            let progress = Number(((100 * (info.current - 1 + event.loaded / event.total)) / info.total).toFixed(2));
                            if (progress >= 100) {
                                //数据包已接收完成，等待后端处理完成
                                progress = 99.5;
                            }
                            console.log(file.name + progress + '%');
                            this.cd.markForCheck();
                            this.observableService.upadteDocumentUploadProgress(file.name, progress);
                        } else if (event.type === HttpEventType.Response) {
                            if (event.body == null) {
                                resolve(true);
                                return;
                            }
                            const returnDocument = event.body as Documents;
                            if (returnDocument && returnDocument.id != undefined) {
                                this.addRecentActivity(null, returnDocument);
                                this.onDoucmentUploadEvent(returnDocument);
                                this.observableService.upadteDocumentUploadProgress(file.name, 100);
                                if (info.current == info.total) {
                                    this.totalFileUploaded = this.totalFileUploaded + 1;
                                    if (this.totalFileUploaded == fileCount) {
                                        this.sendNotification();
                                    }
                                }
                                resolve(false);
                            } else {
                                errorFunc();
                                this.toastrService.error(`上传错误: ${JSON.stringify(event.body).substring(0, 20)}`);
                                resolve(false);
                            }
                        }
                    },
                    error => {
                        errorFunc();
                        resolve(false);
                    }
                );
            });
        };

        ChunkFile(file, chunk);
    }

    async fileEvent($event) {
        let files: File[] = $event.target.files;
        if (files.length == 0) {
            return;
        }
        this.totalFileUploaded = 0;
        if (!this.observableService.progressBarOverlay) {
            this.observableService.progressBarOverlay = this.overlay.open(FileUploadProcessComponent, {
                origin: 'global',
                hasBackdrop: false,
                position: { right: '10px', bottom: '10px' },
                mobilePosition: { left: 0, bottom: 0 }
            });
        }

        for (let index = 0; index < files.length; index++) {
            try {
                const file = files[index];
                this.uploadFile(file, files.length);
            } catch (error) {
                this.toastrService.error(error);
            }
        }
    }
    addRecentActivity(folder: Folder, documents: Documents) {
        const recentActivity: RecentActivity = {
            folderId: folder ? folder.id : null,
            documentId: documents ? documents.id : null,
            action: RecentActivityType.CREATED
        };
        this.sub$.sink = this.commonService.addRecentActivity(recentActivity).subscribe(c => {});
    }

    ngOnDestroy() {
        super.ngOnDestroy();
    }
}
