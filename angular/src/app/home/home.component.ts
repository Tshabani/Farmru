import { Component, Injector, ChangeDetectionStrategy } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { GoogleChartInterface, GoogleChartType } from 'ng2-google-charts';

@Component({
  templateUrl: './home.component.html',
  animations: [appModuleAnimation()],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class HomeComponent extends AppComponentBase {
  //stats: AppStatisticsDto;
  public pieChart: GoogleChartInterface = {
    chartType: GoogleChartType.PieChart,
    dataTable: [
      ['System Stats', ''],
      ['Roles', 0],
      ['Users', 0],
      ['Alerts', 0],
      ['Logs', 0]
    ],
    //firstRowIsData: true,
    options: {'title': 'System stats'},
  };

  public barChart: GoogleChartInterface = {
    chartType: GoogleChartType.BarChart,
    dataTable: [
      ['System Stats', 'Roles', 'Users', 'Alerts', 'Logs'],
      ['total', 0, 0, 0, 0]
    ],
    options: {
      chart: {
        title: 'System stats',
        subtitle: 'Total system stats'
      }
    }
  };

  constructor(injector: Injector) {
    super(injector);

    this.pieChart.dataTable = [
      ['System Stats', ''],
      ['Roles', 5],
      ['Users', 3],
      ['Alerts', 7],
      ['Logs', 10]
    ];

    this.barChart.dataTable = [
      ['System Stats', 'Roles', 'Users', 'Alerts', 'Logs'],
      ['total', 4, 7, 5, 1]
    ];
  }
}
