import {HttpClient, HttpParams} from '@angular/common/http';
import {Inject, Injectable} from '@angular/core';
import {catchError} from 'rxjs/operators';
import {ErrorHandlerService} from './errorHandler.service';

@Injectable({
  providedIn: 'root'
})
export class DataService extends ErrorHandlerService {
  private readonly baseUrl: string;
  private http: HttpClient;

  constructor(httpClient: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    super();
    this.http = httpClient;
    this.baseUrl = baseUrl + 'api/plague_data';
  }

  getDataTypes() {
    let url = this.baseUrl + '/types';
    return this.http.get<PlagueDataType[]>(url).pipe(catchError(this.handleError));
  }

  getData(tokenPath: string) {
    let params = new HttpParams().set('tokenPath', tokenPath);
    return this.http.get<PlagueData[]>(this.baseUrl, {params: params}).pipe(catchError(this.handleError));
  }
}
