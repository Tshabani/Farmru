import { Injectable, Inject, Optional } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '@shared/service-proxies/service-proxies';

export interface AlertDto {
  id: string;
  tenantId: number;
  nodeId?: string;
  facilityId?: string;
  node?: { id: string; displayText: string };
  facility?: { id: string; displayText: string };
  alertType: number;
  severity: number;
  title: string;
  description: string;
  isAcknowledged: boolean;
  isResolved: boolean;
  triggeredAt: string;
  lastTriggeredAt?: string;
  isActive: boolean;
  resolutionNotes?: string;
}

export interface AlertStatisticsDto {
  totalActive: number;
  totalCritical: number;
  totalWarning: number;
  totalInfo: number;
  totalUnacknowledged: number;
  totalResolvedToday: number;
  bySeverity: { severity: number; count: number }[];
  byType: { alertType: number; count: number }[];
  byFacility: { facilityName: string; activeCount: number; criticalCount: number }[];
}

export interface PagedAlertResult {
  totalCount: number;
  items: AlertDto[];
}

@Injectable({ providedIn: 'root' })
export class AlertApiService {
  private baseUrl: string;

  constructor(
    private http: HttpClient,
    @Optional() @Inject(API_BASE_URL) baseUrl?: string
  ) {
    this.baseUrl = baseUrl ?? '';
  }

  getStatistics(): Observable<AlertStatisticsDto> {
    return this.http.get<AlertStatisticsDto>(`${this.baseUrl}/api/services/app/Alert/GetStatistics`);
  }

  getActiveAlerts(): Observable<AlertDto[]> {
    return this.http.get<AlertDto[]>(`${this.baseUrl}/api/services/app/Alert/GetActiveAlerts`);
  }

  getCriticalAlerts(): Observable<AlertDto[]> {
    return this.http.get<AlertDto[]>(`${this.baseUrl}/api/services/app/Alert/GetCriticalAlerts`);
  }

  getAlerts(params: {
    skipCount?: number;
    maxResultCount?: number;
    severity?: number;
    alertType?: number;
    facilityId?: string;
    nodeId?: string;
    activeOnly?: boolean;
    criticalOnly?: boolean;
    unacknowledgedOnly?: boolean;
  }): Observable<PagedAlertResult> {
    let httpParams = new HttpParams();
    Object.keys(params).forEach((key) => {
      const value = (params as any)[key];
      if (value !== undefined && value !== null) {
        httpParams = httpParams.set(key.charAt(0).toUpperCase() + key.slice(1), value);
      }
    });
    return this.http.get<PagedAlertResult>(`${this.baseUrl}/api/services/app/Alert/GetAlerts`, { params: httpParams });
  }

  getAlert(id: string): Observable<AlertDto> {
    return this.http.get<AlertDto>(`${this.baseUrl}/api/services/app/Alert/GetAlert`, { params: { Id: id } });
  }

  getAlertsByNode(nodeId: string): Observable<AlertDto[]> {
    return this.http.get<AlertDto[]>(`${this.baseUrl}/api/services/app/Alert/GetAlertsByNode`, { params: { Id: nodeId } });
  }

  acknowledge(alertId: string): Observable<AlertDto> {
    return this.http.post<AlertDto>(`${this.baseUrl}/api/services/app/Alert/Acknowledge`, { alertId });
  }

  resolve(alertId: string, resolutionNotes?: string): Observable<AlertDto> {
    return this.http.post<AlertDto>(`${this.baseUrl}/api/services/app/Alert/Resolve`, { alertId, resolutionNotes });
  }
}
