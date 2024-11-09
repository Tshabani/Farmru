import { Component, Injector, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { GoogleChartInterface, GoogleChartType } from 'ng2-google-charts';
import { AppStatisticsDto, HomeServiceProxy } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs';

@Component({
  templateUrl: './home.component.html',
  animations: [appModuleAnimation()],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class HomeComponent extends AppComponentBase {
  stats: AppStatisticsDto = new AppStatisticsDto;

  public pieChart: GoogleChartInterface = {
    chartType: GoogleChartType.PieChart,
    dataTable: [
      ['System Stats', ''],
      ['Roles', 0],
      ['Users', 0],
      ['Nodes', 0],
      ['ActiveNodes', 0]
    ],
    //firstRowIsData: true,
    options: {'title': 'System stats'},
  };

  public barChart: GoogleChartInterface = {
    chartType: GoogleChartType.BarChart,
    dataTable: [
      ['System Stats', 'Roles', 'Users', 'Nodes', 'ActiveNodes'],
      ['total', 0, 0, 0, 0]
    ],
    options: {
      chart: {
        title: 'System stats',
        subtitle: 'Total system stats'
      }
    }
  };

  constructor(
    injector: Injector,
    cd: ChangeDetectorRef,
    private _homeService: HomeServiceProxy   
    ) {
    super(injector);
  }

ngOnInit(): void {
  this.list();
}

protected list(
    //finishedCallback: Function
): void {
    this._homeService
    .getAppStats(
    )
    .pipe(
        finalize(() => {
          //finishedCallback();
        })
    )
    .subscribe((result) => {
        this.stats = result;
        this.pieChart.dataTable = [
          ['System Stats', ''],
          ['Roles', result.totalNumberOfRoles],
          ['Users', result.totalNumberOfUsers],
          ['Nodes', result.totalNumberOfNodes],
          ['ActiveNodes', result.totalNumberOfActiveNodes]
        ];

        this.barChart.dataTable = [
          ['System Stats', 'Roles', 'Users', 'Nodes', 'ActiveNodes'],
          ['total', result.totalNumberOfRoles, result.totalNumberOfUsers, result.totalNumberOfNodes, result.totalNumberOfActiveNodes]
        ];
    });
  }
}

