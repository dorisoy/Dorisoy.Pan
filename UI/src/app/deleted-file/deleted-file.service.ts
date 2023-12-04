import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Documents } from '@core/domain-classes/document';
import { Folder } from '@core/domain-classes/folder';
import { CommonError } from '@core/error-handler/common-error';
import { CommonHttpErrorService } from '@core/error-handler/common-http-error.service';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class DeletedFileService {

  constructor(private httpClient: HttpClient,
    private commonHttpErrorService: CommonHttpErrorService) { }

  getDeletedFolders(): Observable<Folder[] | CommonError> {
    const url = `deletedfolder`;
    return this.httpClient.get<Folder[]>(url)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  restoreDeletedFolder(id: string): Observable<boolean | CommonError> {
    const url = `deletedfolder/${id}`;
    return this.httpClient.post<boolean>(url, {})
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  deleteFolder(id: string): Observable<boolean | CommonError> {
    const url = `deletedfolder/${id}`;
    return this.httpClient.delete<boolean>(url)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  getDeletedDocuments() {
    const url = `deleteddocument`;
    return this.httpClient.get<Documents[]>(url)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  restoreDeletedDocument(id: string): Observable<boolean | CommonError> {
    const url = `deleteddocument/${id}`;
    return this.httpClient.post<boolean>(url, {})
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  deleteDocument(id: string): Observable<boolean | CommonError> {
    const url = `deleteddocument/${id}`;
    return this.httpClient.delete<boolean>(url)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }
  deleteFoldersAndDocuments(): Observable<boolean | CommonError> {
    const url = `deletedFolder/all`;
    return this.httpClient.delete<boolean>(url)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }


}
