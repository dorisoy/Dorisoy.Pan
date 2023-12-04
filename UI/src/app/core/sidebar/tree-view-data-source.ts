import { CollectionViewer, DataSource, SelectionChange } from '@angular/cdk/collections';
import { FlatTreeControl } from '@angular/cdk/tree';
import { Folder } from '@core/domain-classes/folder';
import { TreeViewFolder } from '@core/domain-classes/tree-view-folder';
import { ClonerService } from '@core/services/clone.service';
import { TreeViewService } from '@core/services/tree-view.service';
import { BehaviorSubject, merge, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { SubSink } from 'SubSink';

export class TreeViewDataSource implements DataSource<TreeViewFolder> {

  dataChange = new BehaviorSubject<TreeViewFolder[]>([]);
  subSink$: SubSink

  get data(): TreeViewFolder[] { return this.dataChange.value; }
  set data(value: TreeViewFolder[]) {
    this._treeControl.dataNodes = value;
    this.dataChange.next(value);
  }
  constructor(
    private _treeControl: FlatTreeControl<TreeViewFolder>,
    private _database: TreeViewService,
    private clonerService: ClonerService,
    private rootFolder: Folder) {
    this.subSink$ = new SubSink();
  }

  connect(collectionViewer: CollectionViewer): Observable<TreeViewFolder[]> {
    this._treeControl.expansionModel.changed.subscribe(change => {
      if ((change as SelectionChange<TreeViewFolder>).added ||
        (change as SelectionChange<TreeViewFolder>).removed) {
        this.handleTreeControl(change as SelectionChange<TreeViewFolder>);
      }
    });
    return merge(collectionViewer.viewChange, this.dataChange).pipe(map(() => this.data));
  }

  disconnect(collectionViewer: CollectionViewer): void {

  }

  /** Handle expand/collapse behaviors */
  handleTreeControl(change: SelectionChange<TreeViewFolder>) {
    if (change.added) {
      change.added.forEach(node => this.toggleNode(node, true));
    }
    if (change.removed) {
      change.removed.slice().reverse().forEach(node => this.toggleNode(node, false));
    }
  }

  public add(folder: Folder) {
    if (folder.parentId === this.rootFolder.id) {
      const existFolder = this.data.find(c => c.id == folder.id);
      if (!existFolder) {
        const newTreeviewNode: TreeViewFolder = {
          expandable: false,
          physicalFolderId: folder.physicalFolderId,
          level: 0,
          id: folder.id,
          name: folder.name,
          parentId: folder.parentId,
          isLoading: false,
          isShared: folder.isShared
        };
        this.data.splice(0, 0, newTreeviewNode);
        this.dataChange.next(this.clonerService.deepClone<TreeViewFolder[]>(this.data));
      }
    } else {
      const newParent = this._getParent(folder.parentId);
      if (newParent) {
        this.toggleNode(newParent, true);
      }
    }
  }

  /** Remove node from tree */
  public remove(node: TreeViewFolder) {
    this._remove(node);
  }

  public removeById(id: string) {
    const latestNode = this.data.find(c => c.id == id || c.physicalFolderId == id);
    if (latestNode) {
      this._remove(latestNode);
    }
  }

  _getParent(parentId: string) {
    return this.data.find(c => c.id == parentId);
  }

  _remove(node: TreeViewFolder): boolean {
    const latestNode = this.data.find(c => c.id == node.id);
    if (latestNode) {
      const index = this.data.indexOf(latestNode);
      this.toggleNode(latestNode, false);
      this.data.splice(index, 1);
      this.dataChange.next(this.clonerService.deepClone<TreeViewFolder[]>(this.data));
    }
    return true;
  }

  /**
   * Toggle the node, remove from display list
   */
  toggleNode(node: TreeViewFolder, expand: boolean) {
    if (expand) {
      this.subSink$.sink = this._database.getFolderData(node.id).subscribe(c => {
        node.expandable = true;
        let finalNodes: TreeViewFolder[] = [];
        const nodes = c.map(fol => {
          const exist = this.data.find(data => data.id == fol.id);
          if (!exist) {
            fol.level = node.level + 1;
            fol.expandable = false;
            fol.isLoading = false;
            finalNodes.push(fol);
          }

          return fol;
        });
        this.data[index].expandable = true;
        this.data[index].isLoading = false;
        this.data.splice(index + 1, 0, ...finalNodes);
        // notify the change
        this.dataChange.next(this.clonerService.deepClone<TreeViewFolder[]>(this.data));
      });
    } else {
      const index = this.data.indexOf(node);
      this.data[index].expandable = false;
      this.data[index].isLoading = false;
      let count = 0;
      for (let i = index + 1; i < this.data.length
        && this.data[i].level > node.level; i++, count++) { }
      this.data.splice(index + 1, count);
      this.dataChange.next(this.clonerService.deepClone<TreeViewFolder[]>(this.data));
    }
    const index = this.data.indexOf(node);
    node.isLoading = true;
  }
}
