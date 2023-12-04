import { DataSource } from '@angular/cdk/table';
import { HttpResponse } from '@angular/common/http';
import { ResponseHeader } from '@core/domain-classes/response-header';
import { Notification } from '@core/domain-classes/user-notification';
import { NotificationResource } from '@core/domain-classes/user-notification-source';
import { UserResource } from '@core/domain-classes/user-resource';
import { CommonService } from '@core/services/common.service';
import { environment } from '@environments/environment';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { catchError, finalize } from 'rxjs/operators';

export class NotificationDataSource implements DataSource<Notification> {
    private notificationSubject = new BehaviorSubject<Notification[]>([]);
    private responseHeaderSubject = new BehaviorSubject<ResponseHeader>(null);
    private loadingSubject = new BehaviorSubject<boolean>(false);

    public loading$ = this.loadingSubject.asObservable();
    private _count: number = 0;


    public get count(): number {
        return this._count;
    }

    public responseHeaderSubject$ = this.responseHeaderSubject.asObservable();

    constructor(private commonService: CommonService) { }

    connect(): Observable<Notification[]> {
        return this.notificationSubject.asObservable();
    }

    disconnect(): void {
        this.notificationSubject.complete();
        this.loadingSubject.complete();
    }

    loadNotifications(userResource: NotificationResource) {
        this.loadingSubject.next(true);
        this.commonService.getAllNotifications(userResource).pipe(
            catchError(() => of([])),
            finalize(() => this.loadingSubject.next(false)))
            .subscribe((resp: HttpResponse<Notification[]>) => {
                const paginationParam = JSON.parse(
                    resp.headers.get('X-Pagination')
                ) as ResponseHeader;
                this.responseHeaderSubject.next(paginationParam);
                const notifications = [...resp.body];
                notifications.forEach(n => {
                    if (n.documentThumbnail) {
                        n.documentThumbnail = `${environment.apiUrl}${n.documentThumbnail}`
                    }
                })
                this._count = notifications.length;
                this.notificationSubject.next(notifications);
            });
    }
}
