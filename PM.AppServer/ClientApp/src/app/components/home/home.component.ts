import {Component, Inject, OnInit} from '@angular/core';
import {HttpClient} from '@angular/common/http';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit {
  private readonly baseUrl: string;

  private http: HttpClient;

  public plagueDataTypes: PlagueDataType[];
  public tokenPath: string;

  constructor(httpClient: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.http = httpClient;
    this.baseUrl = baseUrl;
  }

  ngOnInit() {
    this.initPlagueDataTypes();
  }

  plagueDataTypeChange(tokenPath) {
    this.tokenPath = tokenPath;
  }

  private initPlagueDataTypes() {
    let url = this.baseUrl + 'api/plague_data/types';
    this.http.get<PlagueDataType[]>(url).subscribe(res => {
      this.plagueDataTypes = res;
      this.tokenPath = this.plagueDataTypes[0].tokenPath;
    }, err => console.error(err));
  }
}
