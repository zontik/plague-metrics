import {Component, Inject, OnInit} from '@angular/core';
import {HttpClient, HttpParams} from '@angular/common/http';
import {interval, Subscription} from 'rxjs';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit {
  private readonly baseUrl: string;
  private appSettings: AppSettings;

  private http: HttpClient;
  private timer: Subscription;

  public plagueData: PlagueData[];
  public plagueDataTypes: PlagueDataType[];
  public tokenPath: string;

  constructor(httpClient: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.http = httpClient;
    this.baseUrl = baseUrl;
  }

  ngOnInit() {
    this.initSettings();
    this.initPlagueData();
  }

  plagueDataTypeChange(tokenPath) {
    this.tokenPath = tokenPath;
    this.fetchPlagueData(this.tokenPath);
    this.startUpdateInterval(this.appSettings.cacheTtlMs);
  }

  private initSettings() {
    let url = this.baseUrl + 'api/settings';

    this.http.get<AppSettings>(url).subscribe(res => {
      this.appSettings = res;
      this.startUpdateInterval(this.appSettings.cacheTtlMs);
    }, err => console.error(err));
  }

  private startUpdateInterval(cacheTtlMs: number) {
    if (this.timer) {
      this.timer.unsubscribe();
    }

    this.timer = interval(cacheTtlMs).subscribe(() => {
      this.fetchPlagueData(this.tokenPath);
    });
  }

  private initPlagueData() {
    let url = this.baseUrl + 'api/plague_data/types';
    this.http.get<PlagueDataType[]>(url).subscribe(res => {
      this.plagueDataTypes = res;
      this.tokenPath = this.plagueDataTypes[0].tokenPath;

      this.fetchPlagueData(this.tokenPath);
    }, err => console.error(err));
  }

  private fetchPlagueData(tokenPath: string) {
    let params = new HttpParams().set('tokenPath', tokenPath);
    let url = this.baseUrl + 'api/plague_data';
    this.http.get<PlagueData[]>(url, {params: params}).subscribe(res => {
      this.plagueData = res;
    }, err => console.error(err));
  }
}
