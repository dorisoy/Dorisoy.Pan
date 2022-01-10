import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { CommonHttpErrorService } from '@core/error-handler/common-http-error.service';
import { User } from '@core/domain-classes/user';
import { Observable } from 'rxjs';
import { CommonError } from '@core/error-handler/common-error';
import { catchError } from 'rxjs/operators';
import { UserResource } from '@core/domain-classes/user-resource';

@Injectable({ providedIn: 'root' })
export class UserService {

  constructor(
    private httpClient: HttpClient,
    private commonHttpErrorService: CommonHttpErrorService) { }

  updateUser(user: User): Observable<User | CommonError> {
    const url = `user/${user.id}`;
    return this.httpClient.put<User>(url, user)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  addUser(user: User): Observable<User | CommonError> {
    const url = `user`;
    return this.httpClient.post<User>(url, user)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  deleteUser(id: string): Observable<void | CommonError> {
    const url = `user/${id}`;
    return this.httpClient.delete<void>(url)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  getUser(id: string): Observable<User | CommonError> {
    const url = `user/${id}`;
    return this.httpClient.get<User>(url)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  resetPassword(user: User): Observable<User | CommonError> {
    const url = `user/resetpassword`;
    return this.httpClient.post<User>(url, user)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  changePassword(user: User): Observable<User | CommonError> {
    const url = `user/changepassword`;
    return this.httpClient.post<User>(url, user)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  updateUserProfile(user: User): Observable<User | CommonError> {
    const url = `user/profile`;
    return this.httpClient.put<User>(url, user)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  getUserProfile(): Observable<User | CommonError> {
    const url = `user/profile`;
    return this.httpClient.get<User>(url)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  getUsers(resource: UserResource): Observable<HttpResponse<User[]> | CommonError> {
    const url = `user/GetUsers`;
    const customParams = new HttpParams()
      .set('Fields', resource.fields)
      .set('OrderBy', resource.orderBy)
      .set('PageSize', resource.pageSize.toString())
      .set('Skip', resource.skip.toString())
      .set('SearchQuery', resource.searchQuery)
      .set('firstName', resource.first_name.toString())
      .set('lastName', resource.last_name.toString())
      .set('email', resource.email.toString())
      .set('phoneNumber', resource.phone_number.toString())
      .set('isActive', resource.is_active? '1':'0')

    return this.httpClient.get<User[]>(url, {
      params: customParams,
      observe: 'response'
    }).pipe(catchError(this.commonHttpErrorService.handleError));
  }

  getRecentlyRegisteredUsers(): Observable<User[] | CommonError> {
    const url = `user/GetRecentlyRegisteredUsers`;
    return this.httpClient.get<User[]>(url)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }

  updateProfilePhoto(form: FormData): Observable<User | CommonError> {
    const url = `user/UpdateUserProfilePhoto`;
    return this.httpClient.post<User>(url, form)
      .pipe(catchError(this.commonHttpErrorService.handleError));
  }
}
