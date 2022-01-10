import { ErrorHandler } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from '@environments/environment';

export class CommonErrorHandlerService implements ErrorHandler {

  constructor(private httpClient: HttpClient) { }

  handleError(error: any) {
    if (environment.production) {
      const obj = {
        errorMessage: (<Error>error).message,
        stack: (<Error>error).stack
      };
      this.saveErrorLog(obj).toPromise()
        .then((res) => { },
          err => { });
    } else {
      console.error(error);
    }
  }

  saveErrorLog(obj: any): Observable<any> {
    return this.httpClient.post<void>('nlog', obj)
  }
}
