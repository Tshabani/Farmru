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
  protected cd: ChangeDetectorRef;
  public pieChart: GoogleChartInterface = {
    chartType: GoogleChartType.PieChart,
    dataTable: [
      ['System Stats', ''],
      ['Roles', 1],
      ['Users', 2],
      ['Nodes', 3],
      ['ActiveNodes', 4]
    ],
    //firstRowIsData: true,
    options: {'title': 'System stats'},
  };

  public barChart: GoogleChartInterface = {
    chartType: GoogleChartType.BarChart,
    dataTable: [
      ['System Stats', 'Roles', 'Users', 'Nodes', 'ActiveNodes'],
      ['total', 1, 2, 3, 4]
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
    this.cd = cd;
    this.list(() => {
      console.log('Finished callback executed in derived class');
    });
  }

ngOnInit(): void {

}

refreshCharts() {
  // Call detectChanges after a brief delay to allow data tables to update
  setTimeout(() => this.cd.detectChanges(), 50);
}

protected list(
    finishedCallback: Function
): void {
    this._homeService
    .getAppStats(
    )
    .pipe(
        finalize(() => {
          finishedCallback();
        })
    )
    .subscribe((result) => {
      debugger;
        this.stats = result;
        this.pieChart.dataTable = [
          ['System Stats', ''],
          ['Roles', this.stats.totalNumberOfRoles],
          ['Users', this.stats.totalNumberOfUsers],
          ['Nodes', this.stats.totalNumberOfNodes],
          ['ActiveNodes', this.stats.totalNumberOfActiveNodes]
        ];

        this.barChart.dataTable = [
          ['System Stats', 'Roles', 'Users', 'Nodes', 'ActiveNodes'],
          ['total', this.stats.totalNumberOfRoles, this.stats.totalNumberOfUsers, this.stats.totalNumberOfNodes, this.stats.totalNumberOfActiveNodes]
        ];     
        
       // Trigger change detection after updating chart data tables
      //this.cd.detectChanges();
            // Refresh charts after data is updated
            this.refreshCharts();
    });
  }
}

