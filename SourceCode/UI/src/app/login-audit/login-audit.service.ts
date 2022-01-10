import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { LoginAudit } from '@core/domain-classes/login-audit';
import { LoginAuditResource } from '@core/domain-classes/login-audit-resource';
import { CommonError } from '@core/error-handler/common-error';
import { CommonHttpErrorService } from '@core/error-handler/common-http-error.service';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({
    providedIn: 'root'
})
export class LoginAuditService {

    constructor(
        private httpClient: HttpClient,
        private commonHttpErrorService: CommonHttpErrorService) { }

    getLoginAudits(resource: LoginAuditResource): Observable<HttpResponse<LoginAudit[]> | CommonError> {
        const url = `LoginAudit`;
        const customParams = new HttpParams()
            .set('Fields', resource.fields)
            .set('OrderBy', resource.orderBy)
            .set('PageSize', resource.pageSize.toString())
            .set('Skip', resource.skip.toString())
            .set('SearchQuery', resource.searchQuery)
            .set('id', resource.id.toString())
            .set('userName', resource.userName.toString())

        return this.httpClient.get<LoginAudit[]>(url, {
            params: customParams,
            observe: 'response'
        }).pipe(catchError(this.commonHttpErrorService.handleError));
    }
}
