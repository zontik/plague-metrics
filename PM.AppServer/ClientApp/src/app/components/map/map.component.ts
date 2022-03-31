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
  @Input() plagueData: PlagueData[];
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
    this.reDrawMap();
  }

  private drawMap() {
    let self = this;

    d3.select('#states')
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

    d3.select('#states')
      .selectAll('.state')
      .style('fill', function (state: any) {
        return self.getColor(state.id, self.colors);
      });
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
