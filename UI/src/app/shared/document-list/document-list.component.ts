import { AfterViewInit, Component, Input, OnInit, Output, QueryList, ViewChild, ViewChildren } from '@angular/core';
import { MatCheckbox } from '@angular/material/checkbox';
import { MatDialog } from '@angular/material/dialog';
import { CommonDialogService } from '@core/common-dialog/common-dialog.service';
import { Documents } from '@core/domain-classes/document';
import { ClonerService } from '@core/services/clone.service';
import { CommonService } from '@core/services/common.service';
import { DocumentBaseComponent } from '@shared/document-base/document-base.component';
import { OverlayPanel } from '@shared/overlay-panel/overlay-panel.service';
import { ToastrService } from 'ngx-toastr';
import { ObservableService } from '@core/services/observable.service';
import { HomeService } from '../../home/home.service';
import { BreakpointsService } from '@core/services/breakpoints.service';
import { fromEvent, of, Subscription } from 'rxjs';
import { MatMenuTrigger } from '@angular/material/menu';
import { tap } from 'rxjs/operators';
import { Download } from '@core/utils/file-helper';

@Component({
    selector: 'app-document-list',
    templateUrl: './document-list.component.html',
    styleUrls: ['./document-list.component.scss']
})
export class DocumentListComponent extends DocumentBaseComponent implements OnInit, AfterViewInit {
    @Input() listView: string;
    @ViewChildren('checkboxes') public checkboxes: MatCheckbox[];

    menuSubscription: Subscription;
    windowClickSubscription: Subscription;

    contextMenuPosition = { x: 0, y: 0 };
    @ViewChild('documentMenuTrigger') contextMenuTrigger: MatMenuTrigger;

    disabled: boolean = false;
    constructor(public toastrService: ToastrService, public homeService: HomeService, public commonService: CommonService, public overlay: OverlayPanel, public dialog: MatDialog, public commonDialogService: CommonDialogService, public clonerService: ClonerService, public observableService: ObservableService, private breakpointsService: BreakpointsService) {
        super(overlay, commonService, homeService, toastrService, dialog, commonDialogService, clonerService, observableService);
    }

    ngOnInit(): void {
        this.folderClickNotification();
        this.isMobileOrTabletDevice();
        super.rootFolderSubscription();
        this.unCheckAllCheckbox();
        this.moveDocumentSubscription();
        this.windowSubscription();
    }
    checkAllSubscription() {
        this.sub$.sink = this.observableService.mainCheckBox$.subscribe(c => {
            this.isCheck = c;
        });
    }

    ngAfterViewInit() {
        this.subscribeCheckAll();
    }

    subscribeCheckAll() {
        this.sub$.sink = this.observableService.checkAll$.subscribe(c => {
            if (this.documents) {
                this.documents.forEach(c => {
                    this.observableService.setDocumentOrFolderIdAll({
                        documentId: c.id,
                        folderId: ''
                    });
                });
            }
            this.checkboxes.forEach(element => {
                element.checked = c;
            });
        });
    }

    onDocumentView(document: Documents) {
        super.onDocumentView(document, this.documents);
    }

    async onDownload(document: Documents) {
        await Download(document);
    }

    windowSubscription() {
        this.sub$.sink = fromEvent(window, 'click').subscribe(_ => {
            if (this.contextMenuTrigger && this.contextMenuTrigger.menuOpen) {
                this.contextMenuTrigger.closeMenu();
                this.documents.forEach(c => {
                    c.isRightClicked = false;
                });
            }
        });
    }

    folderClickNotification() {
        this.sub$.sink = this.observableService.selectedFolder$.subscribe(c => {
            if (this.documents && this.documents.length > 0) {
                this.documents.forEach(c => {
                    c.isRightClicked = false;
                });
            }
        });
    }

    onClickDocument(document: Documents) {
        this.documents.forEach(c => {
            if (c.id === document.id) {
                c.isRightClicked = true;
            } else {
                c.isRightClicked = false;
            }
        });
        this.observableService.setSelectedDocument(document);
    }

    onContextMenu(event: MouseEvent, selectDocument: Documents) {
        event.preventDefault();
        this.observableService.setSelectedDocument(selectDocument);
        this.documents.map(c => {
            if (c.id === selectDocument.id) {
                c.isRightClicked = true;
            } else {
                c.isRightClicked = false;
            }
        });
        this.menuSubscription && this.menuSubscription.unsubscribe();
        this.menuSubscription = of(1)
            .pipe(
                tap(() => {
                    this.contextMenuTrigger.closeMenu();
                    this.contextMenuPosition.x = event.clientX;
                    this.contextMenuPosition.y = event.clientY;
                    this.contextMenuTrigger.menuData = {
                        document: selectDocument,
                        type: 'file'
                    };
                    this.contextMenuTrigger.openMenu();
                    let backdrop: HTMLElement = null;
                })
            )
            .subscribe();
    }

    isMobileOrTabletDevice() {
        this.sub$.sink = this.breakpointsService.isMobile$.subscribe(c => {
            if (c) {
                this.disabled = c;
            }
        });
        this.sub$.sink = this.breakpointsService.isTablet$.subscribe(c => {
            if (c) {
                this.disabled = c;
            }
        });
    }

    moveDocumentSubscription() {
        this.sub$.sink = this.commonService.moveDocumentNotification$.subscribe(c => {
            if (c) {
                this.documents = this.documents.filter(f => f.id != c);
            }
        });
    }

    unCheckAllCheckbox() {
        this.sub$.sink = this.observableService.unCheckAllCheckBox.subscribe(() => {
            if (this.checkboxes) {
                this.checkboxes.forEach(element => {
                    element.checked = false;
                });
            }
        });
    }

    public trackById(index: number, entry: Documents): string {
        return entry.id;
    }
}
