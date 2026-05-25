import { Injectable, Inject, Optional } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '@shared/service-proxies/service-proxies';

export interface MonitoringDashboardDto {
  onlineDevices: number;
  offlineDevices: number;
  staleTelemetryDevices: number;
  activeAlerts: number;
  criticalAlerts: number;
  escalatedAlerts: number;
  monitoringEnabled: boolean;
  lastExecutionAt?: string;
  lastExecutionSucceeded: boolean;
  lastExecution?: MonitoringExecutionHistoryDto;
}

export interface MonitoringExecutionHistoryDto {
  id: string;
  jobType: number;
  startedAt: string;
  completedAt?: string;
  durationMs: number;
  succeeded: boolean;
  errorMessage?: string;
  alertsGenerated: number;
  alertsResolved: number;
  devicesEvaluated: number;
  escalationsPerformed: number;
}

export interface MonitoringConfigurationDto {
  id?: string;
  minimumBatteryPercent: number;
  criticalBatteryPercent: number;
  maximumTemperature: number;
  minimumTemperature: number;
  minimumMoisturePercent: number;
  offlineTimeoutMinutes: number;
  autoResolveWhenNormalized: boolean;
  staleTelemetryThresholdMinutes: number;
  escalationTimeoutMinutes: number;
  anomalySensitivityPercent: number;
  monitoringEnabled: boolean;
}

export interface MonitoringEventDto {
  eventType: string;
  message: string;
  occurredAt: string;
  nodeId?: string;
  nodeDisplay?: string;
  alertId?: string;
  severity?: number;
}

@Injectable({ providedIn: 'root' })
export class MonitoringApiService {
  private baseUrl: string;

  constructor(private http: HttpClient, @Optional() @Inject(API_BASE_URL) baseUrl?: string) {
    this.baseUrl = baseUrl ?? '';
  }

  getDashboard(): Observable<MonitoringDashboardDto> {
    return this.http.get<MonitoringDashboardDto>(`${this.baseUrl}/api/services/app/Monitoring/GetDashboard`);
  }

  getExecutionHistory(skipCount = 0, maxResultCount = 20): Observable<{ totalCount: number; items: MonitoringExecutionHistoryDto[] }> {
    const params = new HttpParams().set('SkipCount', skipCount).set('MaxResultCount', maxResultCount);
    return this.http.get<{ totalCount: number; items: MonitoringExecutionHistoryDto[] }>(
      `${this.baseUrl}/api/services/app/Monitoring/GetExecutionHistory`,
      { params }
    );
  }

  getConfiguration(facilityId?: string): Observable<MonitoringConfigurationDto> {
    let params = new HttpParams();
    if (facilityId) {
      params = params.set('FacilityId', facilityId);
    }
    return this.http.get<MonitoringConfigurationDto>(`${this.baseUrl}/api/services/app/Monitoring/GetConfiguration`, { params });
  }

  updateConfiguration(config: MonitoringConfigurationDto & { facilityId?: string }): Observable<MonitoringConfigurationDto> {
    return this.http.post<MonitoringConfigurationDto>(`${this.baseUrl}/api/services/app/Monitoring/UpdateConfiguration`, config);
  }
}
