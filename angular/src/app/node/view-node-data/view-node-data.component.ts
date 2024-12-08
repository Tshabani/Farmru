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

  graphOptions = [
    { key: 'moisture', label: 'Moisture', selected: true },
    { key: 'nitrogen', label: 'Nitrogen', selected: true },
    { key: 'phosphorus', label: 'Phosphorus', selected: true },
    { key: 'potassium', label: 'Potassium', selected: true },
    { key: 'soilPH', label: 'Soil PH', selected: true },
    { key: 'soilTemperature', label: 'Soil Temperature', selected: true },
    { key: 'solarPanelVoltage', label: 'Solar Panel Voltage', selected: true },
    { key: 'batteryVoltage', label: 'Battery Voltage', selected: true },
  ];

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
  
    this.graphOptions = this.graphOptions.map(option => ({
      ...option,
      selected: true
    }));
  
    this.getDataPage(1);
    this.updateChart(); 
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
  
    this._nodesService.getNodeDataByNodeId(
      this.nodeId,
      request.startDate,
      request.endDate,
      request.predefinedPeriod,
      request.skipCount,
      request.maxResultCount
    )
      .pipe(finalize(() => finishedCallback()))
      .subscribe((result: NodeDataDtoPagedResultDto) => {
        this.nodeData = result.items;
        this.showPaging(result, pageNumber);
  
        this.lineChart = null;
        this.cd.detectChanges();
  
        if (result.items.length <= 0) {
          return;
        }
  
        this.updateChart();
      });
  }


updateChart() {
  const selectedOptions = this.graphOptions.filter(option => option.selected);

  if (selectedOptions.length === 0) {
    this.lineChart = null; 
    this.cd.detectChanges();
    return;
  }

  const headers = ['Creation Time', ...selectedOptions.map(option => option.label)];
  const lineChartData: (Date | number | string)[][] = [headers];

  this.nodeData.forEach(item => {
    const creationTime = item.creationTime instanceof Date
      ? item.creationTime
      : item.creationTime.toDate();

    const row = [
      creationTime,
      ...selectedOptions.map(option => Number(item[option.key]))
    ];
    lineChartData.push(row);
  });

  this.lineChart = {
    chartType: GoogleChartType.LineChart,
    dataTable: lineChartData,
    options: {
      title: 'System Metrics Over Time',
      hAxis: { title: 'Time', format: 'MMM dd, yyyy HH:mm' },
      vAxis: { title: 'Values' },
      height: 600,
      series: selectedOptions.reduce(
        (acc, option, index) => ({
          ...acc,
          [index]: { color: this.getColor(index) },
        }),
        {}
      )
    }
  };
  this.cd.detectChanges();
}

toggleGraph(key: string, event: any) {
  const isSelected = event.target.checked;
  this.graphOptions = this.graphOptions.map(option =>
    option.key === key ? { ...option, selected: isSelected } : option
  );
  this.updateChart();
}

getColor(index: number): string {
  const colors = ['blue', 'green', 'orange', 'purple', 'red', 'pink', 'brown', 'black'];
  return colors[index % colors.length];
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
