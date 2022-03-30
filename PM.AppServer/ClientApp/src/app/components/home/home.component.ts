import {Component, Inject, OnInit} from '@angular/core';
import {HttpClient, HttpParams} from '@angular/common/http';
import {interval} from 'rxjs';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit {
  private readonly baseUrl: string;

  private http: HttpClient;

  public data: PlagueData[];
  public dataTypes: PlagueDataType[];
  public tokenPath: string;

  constructor(httpClient: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.http = httpClient;
    this.baseUrl = baseUrl;
  }

  ngOnInit() {
    this.initSettings();
    this.initData();
  }

  plagueDataTypeChange(tokenPath) {
    this.tokenPath = tokenPath;
    this.fetchData(this.tokenPath);
  }

  private initSettings() {
    let url = this.baseUrl + 'api/settings';

    this.http.get<AppSettings>(url).subscribe(res => {
      interval(res.cacheTtlMs).subscribe(() => {
        this.fetchData(this.tokenPath);
      });
    }, err => console.error(err));
  }

  private initData() {
    let url = this.baseUrl + 'api/data/types';
    this.http.get<PlagueDataType[]>(url).subscribe(res => {
      this.dataTypes = res;
      this.tokenPath = this.dataTypes[0].tokenPath;

      this.fetchData(this.tokenPath);
    }, err => console.error(err));
  }

  private fetchData(tokenPath: string) {
    let params = new HttpParams().set('tokenPath', tokenPath);
    let url = this.baseUrl + 'api/data';
    this.http.get<PlagueData[]>(url, {params: params}).subscribe(res => {
      this.data = res;
    }, err => console.error(err));
  }
}
