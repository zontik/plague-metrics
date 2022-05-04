import {HttpClient} from '@angular/common/http';
import {Inject, Injectable} from '@angular/core';
import {catchError} from 'rxjs/operators';
import {ErrorHandlerService} from './errorHandler.service';

@Injectable({
  providedIn: 'root'
})
export class AppSettingsService extends ErrorHandlerService {
  private readonly url: string;
  private http: HttpClient;

  constructor(httpClient: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    super();
    this.http = httpClient;
    this.url = baseUrl + 'api/settings';
  }

  getSettings() {
    return this.http.get<AppSettings>(this.url).pipe(catchError(this.handleError));
  }
}
