import { HttpClient, HttpEvent, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CopyDocument } from '@core/domain-classes/copy-document';
import { Documents } from '@core/domain-classes/document';
import { DocumentComment } from '@core/domain-classes/document-comment';
import { DocumentShareableLink } from '@core/domain-classes/document-shareable-link';
import { DocumentSource } from '@core/domain-classes/document-source';
import { DocumentVersion } from '@core/domain-classes/document-version';
import { Folder } from '@core/domain-classes/folder';
import { MoveFolder } from '@core/domain-classes/move-folder';
import { RemoveFolderUser } from '@core/domain-classes/remove-folder-user';
import { SharedFileUser } from '@core/domain-classes/shared-file-user';
import { CommonError } from '@core/error-handler/common-error';
import { CommonHttpErrorService } from '@core/error-handler/common-http-error.service';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class HomeService {

  constructor(private httpClient: HttpClient,
    private commonHttpErrorService: CommonHttpErrorService) { }

  getFolders(id: string): Observable<Folder[] | CommonError> {
    const url = `virtualFolder/${id}`;
    return this.httpClient.get<Folder[]>(url)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  getFoldersForCopyAndMove(id: string, sourceId: string): Observable<Folder[] | CommonError> {
    const url = `virtualFolder/${id}/source/${sourceId}`;
    return this.httpClient.get<Folder[]>(url)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  createFolder(folder: Folder) {
    const url = `VirtualFolder/`;
    return this.httpClient.post<Folder>(url, folder)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  getDocumentsByFolderId(id: string): Observable<Documents[] | CommonError> {
    const url = `document/folder/${id}`;
    return this.httpClient.get<Documents[]>(url)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  getFolderParentsById(folderId: string): Observable<Folder[] | CommonError> {
    const url = `folder/${folderId}/parents`;
    return this.httpClient.get<Folder[]>(url)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  AddFolderUserPermission(sharedFileUser: SharedFileUser): Observable<boolean> {
    const url = `VirtualFolder/Folder/shared`;
    return this.httpClient.post<boolean>(url, sharedFileUser)
  }

  AddDocumentUserPermission(sharedFileUser: SharedFileUser): Observable<boolean> {
    const url = `VirtualFolder/Document/shared`;
    return this.httpClient.post<boolean>(url, sharedFileUser)
  }

  removePhysicalUsers(folder: RemoveFolderUser) {
    const url = `VirtualFolder/remove-right/folder/${folder.folderId}/${folder.physicalFolderId}/user/${folder.userId}`;
    return this.httpClient.delete<boolean>(url);
  }

  removeDocumentUsers(documentId: string, userId: string) {
    const url = `document/${documentId}/remove-right/user/${userId}`;
    return this.httpClient.delete<boolean>(url);
  }

  deleteFolder(id: string) {
    const url = `virtualfolder/${id}`;
    return this.httpClient.delete<boolean>(url)
  }

  getDocumentVersion(id: string) {
    const url = `documentversion/${id}`;
    return this.httpClient.get<DocumentVersion[]>(url)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  moveFolder(moveFolder: MoveFolder): Observable<boolean | CommonError> {
    const url = `Folder/move`;
    return this.httpClient.post<boolean>(url, moveFolder)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }
  moveDocument(copyDocument: CopyDocument): Observable<boolean | CommonError> {
    const url = `Document/move`;
    return this.httpClient.post<boolean>(url, copyDocument)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  copyFolder(moveFolder: MoveFolder): Observable<Folder | CommonError> {
    const url = `Folder/copy`;
    return this.httpClient.post<Folder>(url, moveFolder)
  }
  copyDocument(copyDocument: CopyDocument): Observable<boolean | CommonError> {
    const url = `Document/copy`;
    return this.httpClient.post<boolean>(url, copyDocument)
  }
  toggleVirtualFolderStarred(id: string) {
    const url = `virtualfolderstarred/${id}`;
    return this.httpClient.post<boolean>(url, {})
  }

  toggleDocumentStarred(id: string) {
    const url = `documentstarred/${id}`;
    return this.httpClient.post<boolean>(url, {})
  }

  addComment(comment: DocumentComment) {
    const url = `documentcomment/`;
    return this.httpClient.post<DocumentComment>(url, comment)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  getDocumentComments(id: string) {
    const url = `documentcomment/${id}`;
    return this.httpClient.get<DocumentComment[]>(url)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  deleteDocument(id: string) {
    const url = `document/${id}`;
    return this.httpClient.delete<Documents>(url)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  downloadDocument(document: Documents): Observable<HttpEvent<Blob>> {
    const isVersion = document.isVersion ? document.isVersion : false;
    let url = '';
    if (document.isFromPreview) {
      url = `document/${document.id}/download/token/${document.token}?isVersion=${isVersion}`;
    } else {
      url = `document/${document.id}/download?isVersion=${isVersion}`;
    }
    return this.httpClient.get(url, {
      reportProgress: true,
      observe: 'events',
      responseType: 'blob',
    });
  }

  viewerDocument(id: string): Observable<DocumentSource | CommonError> {
    const url = `document/${id}/viewer`;
    return this.httpClient.get<DocumentSource>(url)
      .pipe(
        catchError(this.commonHttpErrorService.handleError));
  }

  downloadFolder(id: string): Observable<HttpEvent<Blob>> {
    const url = `folder/${id}/download`;
    return this.httpClient.get(url, {
      reportProgress: true,
      observe: 'events',
      responseType: 'blob',
    });
  }

  downloadAll(documentIds: string[], folderIds: string[]): Observable<HttpEvent<Blob>> {
    const url = `folder/DownloadFilesAndFolders`;
    return this.httpClient.post(url, {
      documentIds,
      folderIds
    }, {
      reportProgress: true,
      observe: 'events',
      responseType: 'blob',
    });
  }

  getDocumentToken(id: string, isVersion: boolean): Observable<{ [key: string]: string } | CommonError> {
    isVersion = isVersion ? isVersion : false;
    const url = `document/${id}/token?isVersion=${isVersion}`;
    return this.httpClient.get<{ [key: string]: string }>(url);
  }

  deleteDocumentToken(token: string): Observable<void | CommonError> {
    const url = `document/token/${token}`;
    return this.httpClient.delete<void>(url)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  readDocument(document: Documents): Observable<{ [key: string]: string[] } | CommonError> {
    const isVersion = document.isVersion ? document.isVersion : false;
    let url = '';
    if (document.isFromPreview) {
      url = `document/${document.id}/readText/token/${document.token}?isVersion=${isVersion}`;
    } else {
      url = `document/${document.id}/readText?isVersion=${isVersion}`;
    }
    return this.httpClient.get<{ [key: string]: string[] }>(url);
  }

  searchDocument(searchString: string): Observable<Documents[] | CommonError> {
    const url = `document/search/${searchString}`;
    return this.httpClient.get<Documents[]>(url)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  searchFolder(searchString: string): Observable<Folder[] | CommonError> {
    const url = `VirtualFolder/search/${searchString}`;
    return this.httpClient.get<Folder[]>(url)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  getDocumentShareableLink(id: string): Observable<DocumentShareableLink | CommonError> {
    const url = `DocumentShareableLink/${id}`;
    return this.httpClient.get<DocumentShareableLink>(url)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  createDocumentShareableLink(link: DocumentShareableLink): Observable<DocumentShareableLink | CommonError> {
    const url = `DocumentShareableLink`;
    return this.httpClient.post<DocumentShareableLink>(url, link)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  checkLinkPassword(id: string, password: string): Observable<boolean | CommonError> {
    const url = `DocumentLink/${id}/check/${password}`;
    return this.httpClient.get<boolean>(url)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  getDocumentByLinkId(id: string): Observable<DocumentShareableLink | CommonError> {
    const url = `DocumentLink/${id}/document`;
    return this.httpClient.get<DocumentShareableLink>(url)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  getLinkInfoByCode(code: string): Observable<DocumentShareableLink | CommonError> {
    const url = `DocumentLink/info/${code}`;
    return this.httpClient.get<DocumentShareableLink>(url)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  deleteDocumentShareableLInk(id: string): Observable<boolean | CommonError> {
    const url = `DocumentShareableLink/${id}`;
    return this.httpClient.delete<boolean>(url)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  restoreDocumentVersion(id: string, versionId: string) {
    const url = `documentversion/${id}/restore/${versionId}`;
    return this.httpClient.post<boolean>(url, {})
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  renameDocument(id: string, name: string) {
    const url = `document/${id}/rename`;
    return this.httpClient.put<boolean>(url, {
      id,
      name
    }).pipe(catchError(this.commonHttpErrorService.handleError));
  }

  renameFolder(id: string, name: string) {
    const url = `VirtualFolder/${id}/rename`;
    return this.httpClient.put<boolean>(url, {
      id,
      name
    }).pipe(catchError(this.commonHttpErrorService.handleError));
  }
}
