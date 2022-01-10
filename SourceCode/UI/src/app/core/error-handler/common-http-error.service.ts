import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';

import { HttpErrorResponse } from '@angular/common/http';
import { CommonError } from './common-error';
import { } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class CommonHttpErrorService {
  constructor() { }

  handleError(httpErrorResponse: HttpErrorResponse): Observable<CommonError> {
    const customError: CommonError = {
      statusText: httpErrorResponse.statusText,
      code: httpErrorResponse.status,
      messages: httpErrorResponse.error.messages,
      friendlyMessage: 'Error from service',
      error: httpErrorResponse.error
    };
    console.error(httpErrorResponse);
    return throwError(customError);
  }
}
