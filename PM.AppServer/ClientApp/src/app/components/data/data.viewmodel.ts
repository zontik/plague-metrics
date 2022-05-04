import {interval, Subscription} from 'rxjs';
import {DataModel} from './data.model';
import {EventEmitter, Injectable, OnInit} from '@angular/core';
import {DataService} from '../../services/data.service';
import {AppSettingsService} from '../../services/appSettings.service';

@Injectable({
  providedIn: 'root'
})
export class DataViewModel implements OnInit {
  private dataTypeSubscription: Subscription;
  private dataSubscription: Subscription;

  private updateTimer: Subscription;

  dataChanged: EventEmitter<void> = new EventEmitter<void>();

  model: DataModel;

  constructor(private appSettingsService: AppSettingsService, private dataService: DataService) {
    this.model = new DataModel();
  }

  ngOnInit(): void {
    this.dataTypeSubscription = this.model.selectedDataTypeChanged.subscribe((newValue: PlagueDataType) => {
      this.fetchData(newValue.tokenPath);
      this.initDataUpdate(newValue.tokenPath);
    });

    this.dataSubscription = this.model.dataChanged.subscribe(() => {
      this.dataChanged.emit();
    });

    this.dataService.getDataTypes().subscribe((dataTypes: PlagueDataType[]) => {
      this.model.listDataTypes = dataTypes;
      this.model.selectedDataType = dataTypes[0];
    });
  }

  onDestroy(): void {
    this.dataTypeSubscription.unsubscribe();
    this.dataSubscription.unsubscribe();
    this.updateTimer.unsubscribe();

    this.model = null;
  }

  private initDataUpdate(tokenPath: string) {
    if (!this.model.appSettings) {
      this.appSettingsService.getSettings().subscribe((settings: AppSettings) => {
        this.model.appSettings = settings;
        this.restartTimer(tokenPath, settings.cacheTtlMs);
      });
    } else {
      this.restartTimer(tokenPath, this.model.appSettings.cacheTtlMs);
    }
  }

  private restartTimer(tokenPath: string, cacheTtlMs: number) {
    if (this.updateTimer) {
      this.updateTimer.unsubscribe();
    }

    this.updateTimer = interval(cacheTtlMs).subscribe(() => {
      this.fetchData(tokenPath);
    });
  }

  private fetchData(tokenPath: string) {
    this.dataService.getData(tokenPath).subscribe((data: PlagueData[]) => {
      this.model.listData = data;
    });
  }
}