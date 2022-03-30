import {Component, Input, OnChanges, OnInit} from '@angular/core';
import {MapService} from '../../services/map.service';
import * as d3 from 'd3';

@Component({
  selector: 'app-map',
  templateUrl: './map.component.html',
  styleUrls: ['./map.component.css'],
  providers: [MapService]
})
export class MapComponent implements OnInit, OnChanges {
  @Input() data: PlagueData[];
  colors: string[];
  statesPaths: any[];

  constructor(private mapService: MapService) {
    this.colors = this.mapService.colors;
    this.statesPaths = this.mapService.statesPaths;
  }

  ngOnInit() {
    this.drawMap();
  }

  ngOnChanges() {
    d3.selectAll('#states > *').remove();
    this.drawMap();
  }

  private drawMap() {
    let self = this;
    let noDataColor = this.mapService.noDataColor;

    d3.select('#states')
      .selectAll('.state')
      .data(self.statesPaths).enter().append('path')
      .attr('class', 'state')
      .attr('d', function (state) {
        return state.d;
      })
      .style('fill', function (state) {
        return self.getColor(state.id, self.colors, noDataColor);
      });
  }

  private getColor(stateId: string, colors: string[], noDataColor: string): string {
    let stateData = this.data.find(d => d.stateId.toLocaleLowerCase() == stateId.toLocaleLowerCase());
    if (stateData) {
      if (stateData.level <= 1) {
        return colors[0];
      } else if (stateData.level >= colors.length) {
        return colors[colors.length - 1];
      }

      return colors[stateData.level - 1];
    }

    return noDataColor;
  }
}
