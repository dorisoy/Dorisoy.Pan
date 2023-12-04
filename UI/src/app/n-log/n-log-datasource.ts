import { DataSource } from '@angular/cdk/table';
import { HttpResponse } from '@angular/common/http';
import { NLog } from '@core/domain-classes/n-log';
import { NLogResource } from '@core/domain-classes/n-log-resource';
import { ResponseHeader } from '@core/domain-classes/response-header';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { catchError, finalize } from 'rxjs/operators';
import { NLogService } from './n-log.service';

export class NLogDataSource implements DataSource<NLog> {

    private nLogSubject = new BehaviorSubject<NLog[]>([]);
    private responseHeaderSubject = new BehaviorSubject<ResponseHeader>(null);
    private loadingSubject = new BehaviorSubject<boolean>(false);

    public loading$ = this.loadingSubject.asObservable();
    private _count: number = 0;


    public get count(): number {
        return this._count;
    }

    public responseHeaderSubject$ = this.responseHeaderSubject.asObservable();

    constructor(private nLogService: NLogService) { }

    connect(): Observable<NLog[]> {
        return this.nLogSubject.asObservable();
    }

    disconnect(): void {
        this.nLogSubject.complete();
        this.loadingSubject.complete();
    }

    loadNLogs(nLogResource: NLogResource) {
        this.loadingSubject.next(true);
        this.nLogService.getNLogs(nLogResource).pipe(
            catchError(() => of([])),
            finalize(() => this.loadingSubject.next(false))
        )
            .subscribe(
                (resp: HttpResponse<NLog[]>) => {
                    const paginationParam = JSON.parse(
                        resp.headers.get('X-Pagination')
                    ) as ResponseHeader;
                    this.responseHeaderSubject.next(paginationParam);
                    const nLogs = [...resp.body];
                    this._count = nLogs.length;
                    this.nLogSubject.next(nLogs);
                }
            );
    }
}
