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
export class SharedFileService {

  constructor(private httpClient: HttpClient,
    private commonHttpErrorService: CommonHttpErrorService) { }

  getSharedDocuments(): Observable<Documents[] | CommonError> {
    const url = `shareddocument`;
    return this.httpClient.get<Documents[]>(url)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  getSharedFolders(): Observable<Folder[] | CommonError> {
    const url = `virtualFolder/shared`;
    return this.httpClient.get<Folder[]>(url)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }
}
