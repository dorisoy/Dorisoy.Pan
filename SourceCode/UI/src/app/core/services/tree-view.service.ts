import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { TreeViewFolder } from '@core/domain-classes/tree-view-folder';
import { Folder } from '@core/domain-classes/folder';
import { ClonerService } from './clone.service';

@Injectable({ providedIn: 'root' })
export class TreeViewService {
  private _refreshTreeView$: BehaviorSubject<Folder> = new BehaviorSubject<Folder>(null);

  public get refreshTreeView$(): Observable<Folder> {
    return this._refreshTreeView$.asObservable();
  }

  setRefreshTreeView(folder: Folder) {
    this._refreshTreeView$.next(this.cloneService.deepClone<Folder>(folder));
  }

  private _selectedFolder$: BehaviorSubject<Folder> = new BehaviorSubject<Folder>(null);

  private _removedFolder$: BehaviorSubject<Folder> = new BehaviorSubject<Folder>(null);

  private _removedFolderId$: BehaviorSubject<string> = new BehaviorSubject<string>(null);

  public get selectedFolder$(): Observable<Folder> {
    return this._selectedFolder$.asObservable();
  }

  public get removeFolder$(): Observable<Folder> {
    return this._removedFolder$.asObservable();
  }

  public get removeFolderId$(): Observable<string> {
    return this._removedFolderId$.asObservable();
  }

  constructor(
    private httpClient: HttpClient,
    private cloneService: ClonerService) {
  }

  getFolderData(id: string): Observable<TreeViewFolder[]> {
    return this.httpClient.get<TreeViewFolder[]>(`VirtualFolder/${id}/children`);
  }

  setSelectedFolder(folder: Folder): void {
    this._selectedFolder$.next(this.cloneService.deepClone<Folder>(folder));
  }

  setRemovedFolder(folder: Folder): void {
    this._removedFolder$.next(this.cloneService.deepClone<Folder>(folder));
  }
  setRemovedFolderId(id: string): void {
    this._removedFolderId$.next(id);
  }

  getTotalSize(): Observable<number>{
    return this.httpClient.get<number>(`folder/totalSize`);
  }

}
