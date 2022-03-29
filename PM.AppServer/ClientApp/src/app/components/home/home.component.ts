import {Component, Inject, OnInit} from '@angular/core';
import {HttpClient, HttpParams} from "@angular/common/http";
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
  public dataTypeSelected: PlagueDataType;

  constructor(httpClient: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.http = httpClient;
    this.baseUrl = baseUrl;
  }

  ngOnInit() {
    this.initSettings();
    this.initData();
  }

  plagueDataTypeChange($event) {
    this.dataTypeSelected = $event;
    this.fetchData(this.dataTypeSelected.key);
  }

  private initSettings() {
    let url = this.baseUrl + 'api/settings';

    this.http.get<AppSettings>(url).subscribe(res => {
      debugger;

      interval(res.cacheTtlMs).subscribe(() => {
        this.fetchData(this.dataTypeSelected.key);
      });
    }, err => console.error(err));
  }

  private initData() {
    let url = this.baseUrl + 'api/data/types';
    this.http.get<PlagueDataType[]>(url).subscribe(res => {
      this.dataTypes = res;
      this.dataTypeSelected = this.dataTypes[0];

      this.fetchData(this.dataTypeSelected.key);
    }, err => console.error(err));
  }

  private fetchData(typeKey: string) {
    let params = new HttpParams().set("typeKey", typeKey);
    let url = this.baseUrl + 'api/data';
    this.http.get<PlagueData[]>(url, {params: params}).subscribe(res => {
      this.data = res;
    }, err => console.error(err));
  }
}
