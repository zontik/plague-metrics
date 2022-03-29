import {Component, Inject, OnInit} from '@angular/core';
import {HttpClient, HttpParams} from "@angular/common/http";
import {interval} from 'rxjs';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit {
  private readonly baseUrl: string;
  private apiUrl: string = 'api/data';
  private refreshInterval: number = 30_000;

  private http: HttpClient;

  public data: PlagueData[];
  public dataTypes: PlagueDataType[];
  public dataTypeSelected: PlagueDataType;

  constructor(httpClient: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.http = httpClient;
    this.baseUrl = baseUrl;
  }

  ngOnInit() {
    let url = this.baseUrl + this.apiUrl + '/types';
    this.http.get<PlagueDataType[]>(url).subscribe(res => {
      this.dataTypes = res;
      this.dataTypeSelected = this.dataTypes[0];

      this.fetchData(this.dataTypeSelected.key);

      interval(this.refreshInterval).subscribe(() => {
        this.fetchData(this.dataTypeSelected.key);
      });
    }, err => console.error(err));
  }

  plagueDataTypeChange($event) {
    this.dataTypeSelected = $event;
    this.fetchData(this.dataTypeSelected.key);
  }

  private fetchData(typeKey: string) {
    let params = new HttpParams().set("typeKey", typeKey);
    let url = this.baseUrl + this.apiUrl;
    this.http.get<PlagueData[]>(url, {params: params}).subscribe(res => {
      this.data = res;
    }, err => console.error(err));
  }
}
