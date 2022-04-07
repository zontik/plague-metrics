import {Component, Input, OnChanges, OnInit, ElementRef, Inject} from '@angular/core';
import {MapService} from '../../services/map.service';
import * as d3 from 'd3';
import {HttpClient, HttpParams} from '@angular/common/http';
import {interval, Subscription} from 'rxjs';

@Component({
  selector: 'app-map',
  templateUrl: './map.component.html',
  styleUrls: ['./map.component.css'],
  providers: [MapService]
})
export class MapComponent implements OnInit, OnChanges {
  @Input() tokenPath: string;
  private readonly baseUrl: string;
  private appSettings: AppSettings;

  private http: HttpClient;
  private timer: Subscription;

  plagueData: PlagueData[];

  colors: string[];
  statesPaths: any[];
  mapElement: ElementRef;

  constructor(httpClient: HttpClient, @Inject('BASE_URL') baseUrl: string, private mapService: MapService, mapElement: ElementRef) {
    this.http = httpClient;
    this.baseUrl = baseUrl;

    this.colors = this.mapService.colors;
    this.statesPaths = this.mapService.statesPaths;
    this.mapElement = mapElement;
  }

  ngOnInit() {
    this.initSettings();
    this.fetchPlagueData(() => {
      this.drawMap();
    });
  }

  ngOnChanges() {
    if (!this.plagueData) {
      return
    }

    this.fetchPlagueData(() => {
      this.reDrawMap();
    });

    this.startUpdateInterval(this.appSettings.cacheTtlMs);
  }

  private initSettings() {
    let url = this.baseUrl + 'api/settings';

    this.http.get<AppSettings>(url).subscribe(res => {
      this.appSettings = res;
      this.startUpdateInterval(this.appSettings.cacheTtlMs);
    }, err => console.error(err));
  }

  private fetchPlagueData(cb) {
    let params = new HttpParams().set('tokenPath', this.tokenPath);
    let url = this.baseUrl + 'api/plague_data';
    this.http.get<PlagueData[]>(url, {params: params}).subscribe(res => {
      this.plagueData = res;
      cb();
    }, err => console.error(err));
  }

  private startUpdateInterval(cacheTtlMs: number) {
    if (this.timer) {
      this.timer.unsubscribe();
    }

    this.timer = interval(cacheTtlMs).subscribe(() => {
      this.fetchPlagueData(() => {
        this.reDrawMap();
      });
    });
  }

  private drawMap() {
    let self = this;

    d3.select(self.getSvgElement())
      .selectAll('.state')
      .data(self.statesPaths).enter().append('path')
      .attr('class', 'state')
      .attr('d', function (state) {
        return state.d;
      })
      .style('fill', function (state) {
        return self.getColor(state.id, self.colors);
      })
      .on('mouseover', (d, s) => {
        d3.select('#tooltip').transition().duration(200).style('opacity', .9);
        let stateData = self.plagueData.find(d => d.stateId.toLocaleLowerCase() == s.id.toLocaleLowerCase());
        d3.select('#tooltip').html(MapComponent.toolTipHtml(s.n, stateData ? stateData.level : 'no data'))
          .style('left', d.pageX + 'px')
          .style('top', (d.pageY - 28) + 'px');
      })
      .on('mouseout', () => {
        d3.select('#tooltip').transition().duration(500).style('opacity', 0);
      });
  }

  private reDrawMap() {
    let self = this;

    d3.select(self.getSvgElement())
      .selectAll('.state')
      .style('fill', function (state: any) {
        return self.getColor(state.id, self.colors);
      });
  }

  private getSvgElement() {
    return this.mapElement.nativeElement.querySelector('svg');
  }

  private getColor(stateId: string, colors: string[]): string {
    let stateData = this.plagueData.find(d => d.stateId.toLocaleLowerCase() == stateId.toLocaleLowerCase());
    if (stateData) {
      if (stateData.level <= 1) {
        return colors[0];
      } else if (stateData.level >= colors.length) {
        return colors[colors.length - 1];
      }

      return colors[stateData.level - 1];
    }

    return this.mapService.noDataColor;
  }

  private static toolTipHtml(header, value): string {
    return `<h4>${header}</h4><span>Level: ${value}</span>`;
  }
}
