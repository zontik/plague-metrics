import {HttpErrorResponse} from '@angular/common/http';
import {of} from 'rxjs';

export class ErrorHandlerService {
  handleError(error: HttpErrorResponse) {
    if (error.status === 0) {
      console.error('An error occurred:', error.error);
    } else {
      console.error(`Backend returned code ${error.status}, body was: `, error.error);
    }

    return of();
  }
}
