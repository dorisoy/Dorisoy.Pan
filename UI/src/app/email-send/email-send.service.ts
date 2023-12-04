import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Email } from '@core/domain-classes/email';
import { CommonError } from '@core/error-handler/common-error';
import { CommonHttpErrorService } from '@core/error-handler/common-http-error.service';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class EmailSendService {

  constructor(private httpClient: HttpClient,
    private commonHttpErrorService: CommonHttpErrorService) { }

  sendEmail(email: Email): Observable<void | CommonError> {
    const url = 'Email';
    return this.httpClient.post<void>(url, email)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }
}
