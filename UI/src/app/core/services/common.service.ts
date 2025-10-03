import { Injectable } from '@angular/core';
import {
  HttpClient,
  HttpEvent,
  HttpParams,
  HttpRequest,
  HttpResponse,
} from '@angular/common/http';
import { CommonError } from '@core/error-handler/common-error';
import { CommonHttpErrorService } from '@core/error-handler/common-http-error.service';
import { BehaviorSubject, Observable } from 'rxjs';
import { User } from '@core/domain-classes/user';
import { catchError, tap } from 'rxjs/operators';
import { Folder } from '@core/domain-classes/folder';
import { UserResource } from '@core/domain-classes/user-resource';
import { Documents } from '@core/domain-classes/document';
import { UserNotification } from '@core/domain-classes/notification';
import { RecentActivity } from '@core/domain-classes/recent-activity';
import { TreeViewService } from './tree-view.service';
import { HierarchyShared } from '@core/domain-classes/hierarchy-shared';
import { ObservableService } from './observable.service';
import { Notification } from '@core/domain-classes/user-notification';
import { NotificationResource } from '@core/domain-classes/user-notification-source';
import { Email } from '@core/domain-classes/email';

@Injectable({ providedIn: 'root' })
export class CommonService {
  public searchString$: BehaviorSubject<string> = new BehaviorSubject<string>(
    ''
  );
  constructor(
    private httpClient: HttpClient,
    private commonHttpErrorService: CommonHttpErrorService,
    private treeViewService: TreeViewService,
    private observableService: ObservableService
  ) {}

  private _moveDocumentNotification$: BehaviorSubject<string> =
    new BehaviorSubject<string>('');

  public get moveDocumentNotification$(): Observable<string> {
    return this._moveDocumentNotification$.asObservable();
  }

  setMoveDocumentNotification(documentId: string) {
    this._moveDocumentNotification$.next(documentId);
  }

  private listView = '';

  getAllUsers(): Observable<User[] | CommonError> {
    const url = `user/getAllUsers`;
    return this.httpClient
      .get<User[]>(url)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  getRootFolder(): Observable<Folder | CommonError> {
    const url = `virtualFolder/root`;
    return this.httpClient.get<Folder>(url).pipe(
      tap((c) => {
        this.treeViewService.setSelectedFolder(c);
        this.observableService.setRootFolder(c);
      }),
      catchError(this.commonHttpErrorService.handleError)
    );
  }

  getNewNotifications() {
    const url = `UserNotification/new`;
    return this.httpClient
      .get<Notification[]>(url)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  getAllNotifications(
    resource: NotificationResource
  ): Observable<HttpResponse<Notification[]> | CommonError> {
    const url = `UserNotification/all`;
    const customParams = new HttpParams()
      .set('Fields', resource.fields)
      .set('OrderBy', resource.orderBy)
      .set('PageSize', resource.pageSize.toString())
      .set('Skip', resource.skip.toString());

    return this.httpClient
      .get<Notification[]>(url, {
        params: customParams,
        observe: 'response',
      })
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  getUserNotificationCount() {
    const url = `UserNotification/count`;
    return this.httpClient
      .get<number>(url)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  markAsReadNotification(id: string) {
    const url = `UserNotification/${id}`;
    return this.httpClient
      .post(url, {})
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  getUsers(resource: UserResource): Observable<User[]> {
    const url = `User/Shared/Users`;
    const customParams = new HttpParams()
      .set('Fields', resource.fields)
      .set('OrderBy', resource.orderBy)
      .set('FolderId', resource.folderId)
      .set('DocumentId', resource.documentId)
      .set('PageSize', resource.pageSize.toString())
      .set('Skip', resource.skip.toString())
      .set('SearchQuery', resource.searchQuery)
      .set('firstName', resource.first_name.toString())
      .set('lastName', resource.last_name.toString())
      .set('email', resource.email.toString())
      .set('phoneNumber', resource.phone_number.toString())
      .set('FolderId', resource.physicalFolderId.toString())
      .set('type', resource.type.toString())
      .set('isActive', resource.is_active ? '1' : '0');

    return this.httpClient.get<User[]>(url, {
      params: customParams,
    });
  }

  getDocumentById(documentId: string): Observable<Documents | CommonError> {
    const url = `document/${documentId}`;
    return this.httpClient
      .get<Documents>(url)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  sendNotification(
    notification: UserNotification
  ): Observable<boolean | CommonError> {
    const url = `Folder/notification`;
    return this.httpClient
      .post<boolean>(url, notification)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  addRecentActivity(
    recentActivity: RecentActivity
  ): Observable<boolean | CommonError> {
    const url = `RecentActivity`;
    return this.httpClient
      .post<boolean>(url, recentActivity)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  getRecentActivities(): Observable<RecentActivity[] | CommonError> {
    const url = `RecentActivity`;
    return this.httpClient
      .get<RecentActivity[]>(url)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  getListView(): string {
    if (!this.listView) {
      if (!localStorage.getItem('listview')) {
        localStorage.setItem('listview', 'list');
        this.listView = 'list';
      } else {
        this.listView = localStorage.getItem('listview');
      }
    }
    return this.listView;
  }
  getFolderDetailById(id: string): Observable<Folder | CommonError> {
    const url = `VirtualFolder/detail/${id}`;
    return this.httpClient
      .get<Folder>(url)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  setListView(view: string) {
    this.listView = view;
    localStorage.setItem('listview', view);
  }

  isParentChildShared(id: string): Observable<HierarchyShared> {
    const url = `VirtualFolder/${id}/parentchild/shared`;
    return this.httpClient.get<HierarchyShared>(url);
  }

  isParentShared(id: string): Observable<HierarchyShared> {
    const url = `VirtualFolder/${id}/parent/shared`;
    return this.httpClient.get<HierarchyShared>(url);
  }

  uploadFolderDocument(
    form: FormData,
    folderId: string,
    index: number,
    total: number,
    md5: string,
    size: number
  ): Observable<HttpEvent<any>> {
    const url = `folder/${folderId}/${index}/${total}/${md5}/${size}`;
    const request = new HttpRequest('POST', url, form, {
      reportProgress: true,
    });
    return this.httpClient.request(request);
  }

  createChildFoders(
    paths: string[],
    folderId: string,
    physicalFolderId: string
  ): Observable<Folder[] | CommonError> {
    const url = `folder/folder/${folderId}`;
    return this.httpClient
      .post<Folder[]>(url, {
        paths,
        physicalFolderId,
      })
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  sendDocumentEmail(email: Email): Observable<void | CommonError> {
    const url = 'Email/SendDocument';
    return this.httpClient
      .post<void>(url, email)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  sendFolderEmail(email: Email): Observable<void | CommonError> {
    const url = 'Email/SendFolder';
    return this.httpClient
      .post<void>(url, email)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }
}
