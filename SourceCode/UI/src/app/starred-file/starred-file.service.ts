import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Documents } from '@core/domain-classes/document';
import { Folder } from '@core/domain-classes/folder';
import { CommonHttpErrorService } from '@core/error-handler/common-http-error.service';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class StarredFileService {

  constructor(private httpClient: HttpClient,
    private commonHttpErrorService: CommonHttpErrorService) { }

  getStarredDocuments() {
    const url = `documentstarred`;
    return this.httpClient.get<Documents[]>(url)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  getStarredFolders() {
    const url = `VirtualFolderStarred`;
    return this.httpClient.get<Folder[]>(url)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }
}
