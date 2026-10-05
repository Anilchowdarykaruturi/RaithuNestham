import {
HttpErrorResponse,
HttpInterceptorFn
} from '@angular/common/http';

import { Router } from '@angular/router';

import {
catchError,
throwError
} from 'rxjs';

import { inject } from '@angular/core';

import { environment } from '../../environments/environment';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {

const router = inject(Router);

return next(req).pipe(


catchError((error: HttpErrorResponse) => {

  /*
   * Log API errors only during development.
   */
  if (!environment.production) {
    console.error('API ERROR:', error);
  }

  let message =
    'Something went wrong. Please try again.';

  /*
   * Network / server unavailable
   */
  if (error.status === 0) {

    message =
      'Unable to connect to the server. Please make sure the server is running.';
  }

  /*
   * Bad Request
   */
  else if (error.status === 400) {

    message =
      error.error?.message ||
      'Invalid request. Please check your information.';
  }

  /*
   * Unauthorized
   */
  else if (error.status === 401) {

    message =
      'Your session has expired. Please login again.';

    localStorage.removeItem('raithu_token');

    router.navigate(['/login']);
  }

  /*
   * Forbidden
   */
  else if (error.status === 403) {

    message =
      'You do not have permission to perform this action.';
  }

  /*
   * Not Found
   */
  else if (error.status === 404) {

    message =
      'The requested information was not found.';
  }

  /*
   * Server Errors
   */
  else if (error.status >= 500) {

    message =
      'Server error. Please try again later.';
  }

  return throwError(() => ({
    ...error,
    userMessage: message
  }));

})


);
};
