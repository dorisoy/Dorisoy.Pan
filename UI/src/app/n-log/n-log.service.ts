import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { NLog } from '@core/domain-classes/n-log';
import { NLogResource } from '@core/domain-classes/n-log-resource';
import { CommonError } from '@core/error-handler/common-error';
import { CommonHttpErrorService } from '@core/error-handler/common-http-error.service';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({
    providedIn: 'root'
})
export class NLogService {

    constructor(
        private httpClient: HttpClient,
        private commonHttpErrorService: CommonHttpErrorService) { }

    getNLogs(resource: NLogResource): Observable<HttpResponse<NLog[]> | CommonError> {
        const url = `NLog`;
        const customParams = new HttpParams()
            .set('Fields', resource.fields)
            .set('OrderBy', resource.orderBy)
            .set('PageSize', resource.pageSize.toString())
            .set('Skip', resource.skip.toString())
            .set('SearchQuery', resource.searchQuery)
            .set('level', resource.level)
            .set('source', resource.source)
            .set('message', resource.message.toString())

        return this.httpClient.get<NLog[]>(url, {
            params: customParams,
            observe: 'response'
        }).pipe(catchError(this.commonHttpErrorService.handleError));
    }

    getLogDetails(id: string): Observable<NLog | CommonError> {
        const url = `NLog/${id}`;
        return this.httpClient.get<NLog>(url)
            .pipe(catchError(this.commonHttpErrorService.handleError));
    }
}