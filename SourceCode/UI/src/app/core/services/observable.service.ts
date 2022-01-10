import { Injectable } from '@angular/core';
import { Documents } from '@core/domain-classes/document';
import { DocumentFolderId } from '@core/domain-classes/document-folder-id';
import { FileProgress } from '@core/domain-classes/file-progress';
import { Folder } from '@core/domain-classes/folder';
import { FileUploadProcessComponent } from '@shared/file-upload-process/file-upload-process.component';
import { OverlayPanelRef } from '@shared/overlay-panel/overlay-panel-ref';
import { BehaviorSubject, Observable } from 'rxjs';
import { ClonerService } from './clone.service';

@Injectable({
  providedIn: 'root'
})
export class ObservableService {
  private _folderPath$: BehaviorSubject<string> = new BehaviorSubject<string>(null);
  private _toggle$: BehaviorSubject<string> = new BehaviorSubject<string>('');
  private _documentUploadProgress$: BehaviorSubject<FileProgress[]> = new BehaviorSubject<FileProgress[]>([]);
  private _documentFolderIds$: BehaviorSubject<DocumentFolderId[]> = new BehaviorSubject<DocumentFolderId[]>([]);
  private _documentFolderIds: DocumentFolderId[] = [];
  private _fileProcessList: FileProgress[] = [];
  public unCheckAllCheckBox: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(null);
  public progressBarOverlay: OverlayPanelRef<FileUploadProcessComponent>;

  public mainCheckBox$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

  private _selectedFolder$: BehaviorSubject<Folder> = new BehaviorSubject<Folder>(null);

  private _selectedDocument$: BehaviorSubject<Documents> = new BehaviorSubject<Documents>(null);

  private _checkAll$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);


  public rootFolder: Folder = null;

  public get folderPath$() {
    return this._folderPath$.asObservable();
  }


  public get checkAll$(): Observable<boolean> {
    return this._checkAll$.asObservable();
  }
  public setCheckAll(flag: boolean) {
    this._checkAll$.next(flag);
  }

  public get toggle$() {
    return this._toggle$.asObservable();
  }

  public get selectedFolder$() {
    return this._selectedFolder$.asObservable();
  }
  public get selectedDocument$() {
    return this._selectedDocument$.asObservable();
  }


  public get documentAndFolderIds$() {
    return this._documentFolderIds$.asObservable();
  }

  public setDocumentAndFolderIdsEmpty() {
    return this._documentFolderIds$.next([]);
  }

  public get documentUploadProgress$() {
    return this._documentUploadProgress$.asObservable();
  }

  private _rootFolder$: BehaviorSubject<Folder> = new BehaviorSubject<Folder>(null);
  public get rootFolder$(): Observable<Folder> {
    return this._rootFolder$.asObservable();
  }

  constructor(private cloneService: ClonerService) { }

  setToggle(str: string) {
    this._toggle$.next(str);
  }

  setRootFolder(folder: Folder) {
    this._rootFolder$.next(this.cloneService.deepClone<Folder>(folder));
  }

  setSelectedFolder(folder: Folder) {
    this._selectedFolder$.next(this.cloneService.deepClone<Folder>(folder));
  }
  setSelectedDocument(document: Documents) {
    this._selectedDocument$.next(this.cloneService.deepClone<Documents>(document));
  }

  public upadteDocumentUploadProgress(fileName: string, percentage: number, isError = false) {
    const fileProcess: FileProgress = {
      id: fileName,
      percentage: percentage,
      isError: isError
    };
    this._fileProcessList = this._fileProcessList.map(obj => fileProcess.id === obj.id ? fileProcess : obj);
    this._documentUploadProgress$.next(this._fileProcessList);
  }

  public initializeDocumentUploadProcess(id: string) {
    var existing = this._fileProcessList.find(c => c.id == id);
    if (existing) {
      this._fileProcessList = this._fileProcessList.filter(c => c.id != id);
    }
    this._fileProcessList.push({ id: id, percentage: 0 });
    this._documentUploadProgress$.next(this._fileProcessList);
  }

  public resetDocumentUploadProcess() {
    this._fileProcessList = [];
    this._documentUploadProgress$.next(this._fileProcessList);
  }

  public setFolderPath(id: string) {
    this._folderPath$.next(id);
  }

  public setDocumentOrFolderId(dataObject: DocumentFolderId) {
    const isExist = this._documentFolderIds.find(c => c.documentId == dataObject.documentId && c.folderId == dataObject.folderId);
    if (isExist) {
      if (dataObject.documentId) {
        this._documentFolderIds = this._documentFolderIds.filter(c => c.documentId !== dataObject.documentId);
      } else {
        this._documentFolderIds = this._documentFolderIds.filter(c => c.folderId != dataObject.folderId);
      }
    } else {
      this._documentFolderIds.push(dataObject);
    }
    debugger;
    this._documentFolderIds$.next(this.cloneService.deepClone<DocumentFolderId[]>(this._documentFolderIds));
  }

  public setDocumentOrFolderIdAll(dataObject: DocumentFolderId) {
    const isExist = this._documentFolderIds.find(c => c.documentId == dataObject.documentId && c.folderId == dataObject.folderId);
    if (!isExist) {
      this._documentFolderIds.push(dataObject);
    }
    this._documentFolderIds$.next(this.cloneService.deepClone<DocumentFolderId[]>(this._documentFolderIds));
  }

  public removeAll() {
    this._documentFolderIds$.next(this.cloneService.deepClone<DocumentFolderId[]>([]));
    this.unCheckAllCheckBox.next(true);
  }

  public resetDocumentOrFolderId() {
    this._documentFolderIds = [];
    this._documentFolderIds$.next(this._documentFolderIds);
    this.unCheckAllCheckBox.next(true);
  }
}
