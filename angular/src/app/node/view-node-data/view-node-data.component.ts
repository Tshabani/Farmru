import { ChangeDetectorRef, Component, HostListener, Injector } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { NodeDataDto, NodeDataDtoPagedResultDto, NodeDataServiceProxy } from '@shared/service-proxies/service-proxies';
import moment from 'moment';
import { Moment } from 'moment';
import { GoogleChartInterface, GoogleChartType } from 'ng2-google-charts';
import { BsModalService } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs';

class PagedNodeDataRequestDto extends PagedRequestDto {
  predefinedPeriod:string;
  startDate:Moment | undefined;
  endDate:Moment | undefined;
  keyword: string;
}

@Component({
  templateUrl: './view-node-data.component.html',
  animations: [appModuleAnimation()]
})

export class ViewNodeDataComponent extends PagedListingComponentBase<NodeDataDto> {
  nodeId: string;
  nodeData: NodeDataDto[] = [];
  keyword:string = '';
  predefinedPeriod:string = '';
  _startDate:Moment | undefined = undefined;
  _endDate:Moment | undefined = undefined;
  advancedFiltersVisible = false;

  public pieChart: GoogleChartInterface;
  public barChart: GoogleChartInterface;
  public lineChart: GoogleChartInterface;

  constructor(
    injector: Injector,
    private _nodesService: NodeDataServiceProxy,
    private route: ActivatedRoute,
    cd: ChangeDetectorRef
  ) {
    super(injector, cd);
  }

  set startDate(value: string | null) {
    this._startDate = value ? moment(value, 'YYYY-MM-DD') : undefined;
  }

  set endDate(value: string | null) {
    this._endDate = value ? moment(value, 'YYYY-MM-DD') : undefined;
  }


  @HostListener('window:resize', ['$event'])
  onResize(event: Event): void {
    this.refreshCharts();
  }
  
  clearFilters(): void {
    this.keyword = '';
    this.predefinedPeriod = '';
    this.startDate = undefined;
    this.endDate = undefined;
    this.getDataPage(1);
  }

  refreshCharts(): void {
    const pieChartData = this.pieChart;
    const barChartData = this.barChart;
    const lineChartData = this.lineChart;

    this.pieChart = null;
    this.barChart = null;
    this.lineChart = null;

    this.cd.detectChanges();

    setTimeout(() => {
      this.pieChart = pieChartData;
      this.barChart = barChartData;
      this.lineChart = lineChartData;
      this.cd.detectChanges();
    });
  }

  list(
    request: PagedNodeDataRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    request.keyword = this.keyword;
    request.predefinedPeriod = this.predefinedPeriod;
    request.startDate = this._startDate;
    request.endDate = this._endDate;
    
    this.nodeId = this.route.snapshot.paramMap.get('id');

    this._nodesService.getNodeDataByNodeId(this.nodeId,request.startDate,request.endDate,request.predefinedPeriod, request.skipCount, request.maxResultCount)
      .pipe(
        finalize(() => {
          finishedCallback();
        })
      )
      .subscribe((result: NodeDataDtoPagedResultDto) => {
        this.nodeData = result.items;
        this.showPaging(result, pageNumber);

        this.lineChart = null;
        this.cd.detectChanges();

        if(result.items.length <= 0)
        {          
          return;
        }     
        
        const lineChartData: (Date | number | string)[][] = [
          ['Logging Time', 'Moisture', 'Nitrogen', 'Phosphorus', 'Potassium', 'Soil PH', 'Soil Temperature', 'Solar Panel Voltage', 'Battery Voltage']
        ];        

        result.items.forEach(item => {
          const loggingTime = item.loggingTime instanceof Date
          ? item.loggingTime
          : item.loggingTime.toDate();

          lineChartData.push([
            loggingTime, 
            Number(item.moisture),
            Number(item.nitrogen),
            Number(item.phosphorus),
            Number(item.potassium),
            Number(item.soilPH),
            Number(item.soilTemperature),
            Number(item.solarPanelVoltage),
            Number(item.batteryVoltage)
          ]);
        });

        this.lineChart = {
          chartType: GoogleChartType.LineChart,
          dataTable: lineChartData,
          options: {
            title: 'System Metrics Over Time',
            hAxis: {
              title: 'Time',
              format: 'MMM dd, yyyy HH:mm'
            },
            vAxis: {
              title: 'Values'
            },
            height: 600, 
            series: {
              0: { color: 'blue' },
              1: { color: 'green' },
              2: { color: 'orange' },
              3: { color: 'purple' },
              4: { color: 'red' },
              5: { color: 'pink' },
              6: { color: 'brown' },
              7: { color: 'black' }
            }
          }
        };

        this.cd.detectChanges();
      });
  }

  delete(node: NodeDataDto): void {
    abp.message.confirm(
      this.l('NodeDeleteWarningMessage', node.id),
      undefined,
      (result: boolean) => {
        if (result) {
          this._nodesService
            .delete(node.id)
            .pipe(
              finalize(() => {
                abp.notify.success(this.l('SuccessfullyDeleted'));
                this.refresh();
              })
            )
            .subscribe(() => {});
        }
      }
    );
  }
}
