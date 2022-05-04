import {EventEmitter} from '@angular/core';

export class DataModel {
  selectedDataTypeChanged: EventEmitter<PlagueDataType> = new EventEmitter<PlagueDataType>();
  dataChanged: EventEmitter<void> = new EventEmitter<void>();

  private _listDataTypes: PlagueDataType[];
  private _selectedDataType: PlagueDataType;

  private _listData: PlagueData[];

  private _appSettings: AppSettings;

  constructor() {
  }

  public set listDataTypes(value: PlagueDataType[]) {
    this._listDataTypes = value;
  }

  public get listDataTypes() {
    return this._listDataTypes;
  }

  public get selectedDataType(): PlagueDataType {
    return this._selectedDataType;
  }

  public set selectedDataType(value: PlagueDataType) {
    this._selectedDataType = value;
    this.selectedDataTypeChanged.emit(value);
  }

  public get listData(): PlagueData[] {
    return this._listData;
  }

  public set listData(value: PlagueData[]) {
    this._listData = value;
    this.dataChanged.emit();
  }

  public get appSettings(): AppSettings {
    return this._appSettings;
  }

  public set appSettings(value: AppSettings) {
    this._appSettings = value;
  }
}
