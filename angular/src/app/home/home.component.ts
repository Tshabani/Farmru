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
  stats: AppStatisticsDto = new AppStatisticsDto();
  public pieChart: GoogleChartInterface;
  public barChart: GoogleChartInterface;
  
   constructor(   
    injector: Injector,
    private cd: ChangeDetectorRef,
    private _homeService: HomeServiceProxy   
  ) {
    super(injector);   
  }

  ngOnInit(): void {
    this.fetchChartData();
  }

  fetchChartData(): void {
    this. _homeService.getAppStats().pipe(
      finalize(() => {
        this.cd.detectChanges();
      })
    ).subscribe(data => {
      this.stats = data;
      this.pieChart = {
        chartType: GoogleChartType.PieChart,
        dataTable: [
          ['System Stats', ''],
          ['Roles', data.totalNumberOfRoles],
          ['Users', data.totalNumberOfUsers],
          ['Nodes', data.totalNumberOfNodes],
          ['ActiveNodes', data.totalNumberOfActiveNodes]
        ],
        options: { 'title': 'System stats' }
      };

      this.barChart = {
        chartType: GoogleChartType.BarChart,
        dataTable: [
          ['System Stats', 'Roles', 'Users', 'Nodes', 'ActiveNodes'],
          ['total', data.totalNumberOfRoles, data.totalNumberOfUsers, data.totalNumberOfNodes, data.totalNumberOfActiveNodes]
        ],
        options: {
          chart: {
            title: 'System stats',
            subtitle: 'Total system stats'
          }
        }
      };    
    });
  }
}
