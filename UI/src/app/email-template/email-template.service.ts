import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { EmailTemplate } from '@core/domain-classes/email-template';
import { CommonError } from '@core/error-handler/common-error';
import { CommonHttpErrorService } from '@core/error-handler/common-http-error.service';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class EmailTemplateService {

  constructor(
    private httpClient: HttpClient,
    private commonHttpErrorService: CommonHttpErrorService) { }

  updateEmailTemplate(emailTemplate: EmailTemplate): Observable<EmailTemplate | CommonError> {
    const url = `emailTemplate/${emailTemplate.id}`;
    return this.httpClient.put<EmailTemplate>(url, emailTemplate)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  addEmailTemplate(emailTemplate: EmailTemplate): Observable<EmailTemplate | CommonError> {
    const url = `emailTemplate`;
    return this.httpClient.post<EmailTemplate>(url, emailTemplate)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }
  deleteEmailTemplate(emailTemplate: EmailTemplate): Observable<EmailTemplate | CommonError> {
    const url = `emailTemplate/${emailTemplate.id}`;
    return this.httpClient.delete<EmailTemplate>(url)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  getEmailTemplate(id: string): Observable<EmailTemplate | CommonError> {
    const url = `emailTemplate/${id}`;
    return this.httpClient.get<EmailTemplate>(url)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  getEmailTemplates(): Observable<EmailTemplate[] | CommonError> {
    const url = `emailTemplate`;
    return this.httpClient.get<EmailTemplate[]>(url)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

}
