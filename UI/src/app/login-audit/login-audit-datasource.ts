import { DataSource } from '@angular/cdk/table';
import { HttpResponse } from '@angular/common/http';
import { LoginAudit } from '@core/domain-classes/login-audit';
import { LoginAuditResource } from '@core/domain-classes/login-audit-resource';
import { ResponseHeader } from '@core/domain-classes/response-header';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { catchError, finalize } from 'rxjs/operators';
import { LoginAuditService } from './login-audit.service';

export class LoginAuditDataSource implements DataSource<LoginAudit> {

    private loginAuditSubject = new BehaviorSubject<LoginAudit[]>([]);
    private responseHeaderSubject = new BehaviorSubject<ResponseHeader>(null);
    private loadingSubject = new BehaviorSubject<boolean>(false);

    public loading$ = this.loadingSubject.asObservable();
    private _count: number = 0;


    public get count(): number {
        return this._count;
    }

    public responseHeaderSubject$ = this.responseHeaderSubject.asObservable();

    constructor(private loginAuditService: LoginAuditService) { }

    connect(): Observable<LoginAudit[]> {
        return this.loginAuditSubject.asObservable();
    }

    disconnect(): void {
        this.loginAuditSubject.complete();
        this.loadingSubject.complete();
    }

    loadLoginAudits(loginAuditResource: LoginAuditResource) {
        this.loadingSubject.next(true);
        this.loginAuditService.getLoginAudits(loginAuditResource).pipe(
            catchError(() => of([])),
            finalize(() => this.loadingSubject.next(false))
        )
            .subscribe(
                (resp: HttpResponse<LoginAudit[]>) => {
                    const paginationParam = JSON.parse(
                        resp.headers.get('X-Pagination')
                    ) as ResponseHeader;
                    this.responseHeaderSubject.next(paginationParam);
                    const loginAuditTrails = [...resp.body];
                    this._count = loginAuditTrails.length;
                    this.loginAuditSubject.next(loginAuditTrails);
                }
            );
    }
}
