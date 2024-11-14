import { Component, Injector, ChangeDetectionStrategy, ChangeDetectorRef, ViewChild, ElementRef, HostListener } from '@angular/core';
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
  
  public soilTemperatureGauge: GoogleChartInterface;
  public soilPHGauge: GoogleChartInterface;
  public moistureGauge: GoogleChartInterface;
  public phosphorusGauge: GoogleChartInterface;

  public potassiumGauge: GoogleChartInterface;
  public nitrogenGauge: GoogleChartInterface;
  public solarPanelVoltageGauge: GoogleChartInterface;
  public batteryVoltageGauge: GoogleChartInterface;

  constructor(   
    injector: Injector,
    private cd: ChangeDetectorRef,
    private _homeService: HomeServiceProxy   
  ) {
    super(injector);   
  }

  @HostListener('window:resize', ['$event'])
  onResize(event: Event): void {
    this.refreshCharts();
  }

  ngOnInit(): void {
    this.fetchStatsData();
    this.fetchAverageNodeData();
  }
  
  refreshCharts(): void {
    // Temporarily set chart to null to force re-render
    const pieChartData = this.pieChart;
    const barChartData = this.barChart;

    this.pieChart = null;
    this.barChart = null;

    this.cd.detectChanges();

    setTimeout(() => {
      this.pieChart = pieChartData;
      this.barChart = barChartData;
      this.cd.detectChanges();
    });
  }

  fetchStatsData(): void {
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



  fetchAverageNodeData(): void {
    this._homeService.getSensorData().pipe(
      finalize(() => {
        this.cd.detectChanges();
      })
    ).subscribe(data => {
      this.soilTemperatureGauge = {
        chartType: GoogleChartType.Gauge,
        dataTable: [
          ['Label', 'Value'],
          ['Soil Temp', data.avgSoilTemperature] // avgSoilTemperature
        ],
        options: {
          width: 400,
          height: 150,
          redFrom: 35, 
          redTo: 50,
          yellowFrom: 25, 
          yellowTo: 35,
          minorTicks: 5,
          max: 50,
          min: 0
        }
      };
  
      this.soilPHGauge = {
        chartType: GoogleChartType.Gauge,
        dataTable: [
          ['Label', 'Value'],
          ['Soil pH', data.avgSoilPH] // avgSoilPH
        ],
        options: {
          width: 400,
          height: 150,
          redFrom: 8, 
          redTo: 14,
          yellowFrom: 7, 
          yellowTo: 8,
          minorTicks: 5,
          max: 14,
          min: 0
        }
      };
  
      this.moistureGauge = {
        chartType: GoogleChartType.Gauge,
        dataTable: [
          ['Label', 'Value'],
          ['Moisture', data.avgMoisture] // avgMoisture
        ],
        options: {
          width: 400,
          height: 150,
          redFrom: 15, 
          redTo: 20,
          yellowFrom: 10, 
          yellowTo: 15,
          minorTicks: 5,
          max: 20,
          min: 0
        }
      };
  
      this.phosphorusGauge = {
        chartType: GoogleChartType.Gauge,
        dataTable: [
          ['Label', 'Value'],
          ['Phosphorus', data.avgPhosphorus] // avgPhosphorus
        ],
        options: {
          width: 400,
          height: 150,
          redFrom: 150, 
          redTo: 200,
          yellowFrom: 100, 
          yellowTo: 150,
          minorTicks: 5,
          max: 200,
          min: 0
        }
      };

      this.potassiumGauge = {
        chartType: GoogleChartType.Gauge,
        dataTable: [
          ['Label', 'Value'],
          ['Potassium', data.avgPotassium] // avgPotassium
        ],
        options: {
          width: 400,
          height: 150,
          redFrom: 150, 
          redTo: 200,
          yellowFrom: 100, 
          yellowTo: 150,
          minorTicks: 5,
          max: 200,
          min: 0
        }
      };
  
      this.nitrogenGauge = {
        chartType: GoogleChartType.Gauge,
        dataTable: [
          ['Label', 'Value'],
          ['Nitrogen', data.avgNitrogen] // avgNitrogen
        ],
        options: {
          width: 400,
          height: 150,
          redFrom: 20, 
          redTo: 30,
          yellowFrom: 15, 
          yellowTo: 20,
          minorTicks: 5,
          max: 30,
          min: 0
        }
      };
  
      this.solarPanelVoltageGauge = {
        chartType: GoogleChartType.Gauge,
        dataTable: [
          ['Label', 'Value'],
          ['Solar Voltage', data.avgSolarPanelVoltage] // avgSolarPanelVoltage
        ],
        options: {
          width: 400,
          height: 150,
          redFrom: 12, 
          redTo: 15,
          yellowFrom: 10, 
          yellowTo: 12,
          minorTicks: 5,
          max: 15,
          min: 0
        }
      };
  
      this.batteryVoltageGauge = {
        chartType: GoogleChartType.Gauge,
        dataTable: [
          ['Label', 'Value'],
          ['Battery Voltage', data.avgBatteryVoltage] // avgBatteryVoltage
        ],
        options: {
          width: 400,
          height: 150,
          redFrom: 12, 
          redTo: 15,
          yellowFrom: 10, 
          yellowTo: 12,
          minorTicks: 5,
          max: 15,
          min: 0
        }
      };
    });
  }
}
