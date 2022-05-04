import {BrowserModule} from '@angular/platform-browser';
import {NgModule} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {HttpClientModule} from '@angular/common/http';
import {RouterModule} from '@angular/router';
import {FlexLayoutModule} from '@angular/flex-layout';

import {AppComponent} from './app.component';
import {DataViewComponent} from './components/data/data.view.component';
import {NoopAnimationsModule} from '@angular/platform-browser/animations';
import {MatSelectModule} from '@angular/material/select';
import {AppSettingsService} from './services/appSettings.service';
import {DataService} from './services/data.service';
import {MapService} from './services/map.service';

@NgModule({
  declarations: [
    AppComponent,
    DataViewComponent,
  ],
  imports: [
    BrowserModule.withServerTransition({appId: 'ng-cli-universal'}),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      {path: '', component: DataViewComponent, pathMatch: 'full'},
    ]),
    FlexLayoutModule,
    NoopAnimationsModule,
    MatSelectModule
  ],
  providers: [AppSettingsService, DataService, MapService],
  bootstrap: [AppComponent]
})
export class AppModule {
}
