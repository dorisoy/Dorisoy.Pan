import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { Folder } from '@core/domain-classes/folder';
import { FolderPath } from '@core/domain-classes/folder-path';
import { TreeViewService } from '@core/services/tree-view.service';
import { BaseComponent } from 'src/app/base.component';
import { ObservableService } from '../../core/services/observable.service';
import { HomeService } from '../home.service';

@Component({
  selector: 'app-folder-path',
  templateUrl: './folder-path.component.html',
  styleUrls: ['./folder-path.component.scss']
})
export class FolderPathComponent extends BaseComponent implements OnInit {
  folderPaths: FolderPath[] = [];
  @Output() onPathClickEvent: EventEmitter<Folder> = new EventEmitter<Folder>();
  constructor(
    private observableService: ObservableService,
    private homeService: HomeService,
    private treeViewService: TreeViewService) {
    super();
  }

  ngOnInit(): void {
    this.setFolderPath();
    this.getTreeviewSubscription();
  }

  getTreeviewSubscription() {
    this.sub$.sink = this.treeViewService.selectedFolder$.subscribe(c => {
      if (c) {
        this.getPath(c.id);
      }
    });
  }

  setFolderPath() {
    this.sub$.sink = this.observableService.folderPath$
    .subscribe(id => {
      if (id) {
        this.getPath(id);
      } else {
        this.folderPaths = [];
      }
    });
  }
  getPath(id){
    this.sub$.sink = this.homeService.getFolderParentsById(id)
    .subscribe((path: FolderPath[]) => {
      this.folderPaths = path;
    });
  }

  onPathClick(folder: Folder) {
    this.onPathClickEvent.emit(folder);
  }

}
