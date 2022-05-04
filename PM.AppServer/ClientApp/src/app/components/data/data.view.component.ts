import {Component, ElementRef, OnDestroy, OnInit} from '@angular/core';
import {DataViewModel} from './data.viewmodel';
import {DataModel} from './data.model';
import {MapService} from '../../services/map.service';
import * as d3 from 'd3';

@Component({
  templateUrl: './data.view.component.html',
  styleUrls: ['./data.view.component.css'],
  providers: [MapService],
})
export class DataViewComponent implements OnInit, OnDestroy {
  mapDraw: boolean;

  colors: string[];
  statesPaths: any[];
  mapElement: ElementRef;

  constructor(private viewModel: DataViewModel, private mapService: MapService, mapElement: ElementRef) {
    this.colors = this.mapService.colors;
    this.statesPaths = this.mapService.statesPaths;
    this.mapElement = mapElement;
  }

  ngOnInit(): void {
    this.viewModel.ngOnInit();

    this.viewModel.dataChanged.subscribe(() => {
      if (this.mapDraw) {
        this.reDrawMap();
      } else {
        this.drawMap();
      }
    });
  }

  ngOnDestroy(): void {
    this.viewModel.onDestroy();
  }

  get model(): DataModel {
    return this.viewModel.model;
  }

  set model(_model: DataModel) {
    this.viewModel.model = _model;
  }

  private drawMap() {
    let self = this;
    this.mapDraw = true;

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
        let stateData = this.model.listData.find(d => d.stateId.toLocaleLowerCase() == s.id.toLocaleLowerCase());
        d3.select('#tooltip').html(DataViewComponent.toolTipHtml(s.n, stateData ? stateData.level : 'no data'))
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
    let stateData = this.model.listData.find(d => d.stateId.toLocaleLowerCase() == stateId.toLocaleLowerCase());
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
