import { animate, state, style, transition, trigger } from '@angular/animations';
import { FlatTreeControl } from '@angular/cdk/tree';
import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { Folder } from '@core/domain-classes/folder';
import { TreeViewFolder } from '@core/domain-classes/tree-view-folder';
import { UserAuth } from '@core/domain-classes/user-auth';
import { SecurityService } from '@core/security/security.service';
import { ClonerService } from '@core/services/clone.service';
import { ObservableService } from '@core/services/observable.service';
import { TreeViewService } from '@core/services/tree-view.service';
import { filter } from 'rxjs/operators';
import { BaseComponent } from 'src/app/base.component';
import { TreeViewDataSource } from './tree-view-data-source';
declare var $: any;

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss'],
  animations: [
    trigger('slide', [
      state('up', style({ height: 0 })),
      state('down', style({ height: '*' })),
      transition('up <=> down', animate(200))
    ])
  ]
})
export class SidebarComponent extends BaseComponent implements OnInit, OnDestroy {
  appUserAuth: UserAuth = null;
  @Input() rootFolder: Folder;
  private currentRoute: string;
  isAllFileCollapse = false;
  currentFolder: Folder;
  length: number = 0;
  totalSize: number = 0;

  constructor(
    private treeViewService: TreeViewService,
    private securityService: SecurityService,
    private cloneService: ClonerService,
    private router: Router,
    private observableService: ObservableService
  ) {
    super();
    this.sub$.sink = router.events.pipe(filter(event => event instanceof NavigationEnd))
      .subscribe((event: NavigationEnd) => {
        this.currentRoute = event.url;
        this.length = this.currentRoute.length;
      });
  }

  treeControl: FlatTreeControl<TreeViewFolder>;

  dataSource: TreeViewDataSource;

  getLevel = (node: TreeViewFolder) => node.level;

  isExpandable = (node: TreeViewFolder) => node.expandable;

  selectedFolderSubscription() {
    this.sub$.sink = this.treeViewService.selectedFolder$.subscribe(c => {
      this.currentFolder = c;
    })
  }

  getTotalSize() {
    this.sub$.sink = this.treeViewService.getTotalSize()
      .subscribe((size: number) => {
        this.totalSize = Math.round(size);
      });
  }

  onSelectedFolder(node: TreeViewFolder) {
    const data = this.cloneService.deepClone<Folder>({
      id: node.id,
      name: node.name,
      parentId: node.parentId,
      physicalFolderId: node.physicalFolderId
    });
    this.router.navigate(['/', data.id]);
    this.observableService.setToggle(data.id);
  }
  hasChild = (_: number, _nodeData: TreeViewFolder) => _nodeData.expandable;
  ngOnInit() {
    this.getTotalSize();
    this.treeControl = new FlatTreeControl<TreeViewFolder>(this.getLevel, this.isExpandable);
    if (this.rootFolder.name === "All FIles") {
      this.rootFolder.name = "全部";
    }
    this.dataSource = new TreeViewDataSource(this.treeControl, this.treeViewService, this.cloneService, this.rootFolder);
    this.setTopLogAndName();
    this.selectedFolderSubscription();
    this.refreshTreeViewSubscription();
    this.removedTreeViewSubscription();
    this.removedFolderByIdSubscription();
    this.sub$.sink = this.treeViewService.getFolderData(this.rootFolder.id)
      .subscribe(c => {
        this.dataSource.data = c.map(d => {
          d.level = 0;
          d.expandable = false
          d.isLoading = false
          return d;
        });
      });
  }

  allFileClick(id: string) {
    if (this.currentFolder.id !== id) {
      this.treeViewService.setSelectedFolder(this.rootFolder);
    }
    this.router.navigate(["/", id]);
    this.observableService.setToggle(id);
  }

  refreshTreeViewSubscription() {
    this.sub$.sink = this.treeViewService.refreshTreeView$.subscribe(c => {
      if (c) {
        this.dataSource.add(c);
      }
    });
  }

  onRedirect(routerLink: string) {
    this.observableService.setToggle(routerLink);
  }

  removedFolderByIdSubscription() {
    this.sub$.sink = this.treeViewService.removeFolderId$.subscribe(c => {
      if (c) {
        this.dataSource.removeById(c);
      }
    });
  }
  removedTreeViewSubscription() {
    this.sub$.sink = this.treeViewService.removeFolder$.subscribe(c => {
      if (c) {
        const newTreeData: TreeViewFolder = {
          id: c.id,
          name: c.name,
          parentId: c.parentId,
          level: 0,
          expandable: false,
          isLoading: false,
          physicalFolderId: c.physicalFolderId,
          isShared: false
        };
        this.dataSource.remove(newTreeData);
        // this.dataSource.removeNode(c);
      }
    });
  }

  setTopLogAndName() {
    this.sub$.sink = this.securityService.securityObject$
      .subscribe(c => {
        if (c) {
          this.appUserAuth = c;
        }
      })
  }
  loadChildren(node: TreeViewFolder) {
    this.dataSource.toggleNode(node, !node.expandable);
  }

  ngOnDestroy() {
    this.dataSource.subSink$.unsubscribe();
    super.ngOnDestroy();
  }

}
