import { Injectable, NgZone } from '@angular/core';
import { Subject } from 'rxjs';
import { AppConsts } from '@shared/AppConsts';
import { UtilsService } from 'abp-ng2-module';
import { AlertDto } from './alert-api.service';
import { IncidentDto } from '../../incidents/services/incident-api.service';
import * as signalR from '@microsoft/signalr';

@Injectable({ providedIn: 'root' })
export class AlertSignalRService {
  private connection: signalR.HubConnection;
  private started = false;
  readonly alertChanged$ = new Subject<{ action: string; alert: AlertDto }>();
  readonly monitoringEvent$ = new Subject<any>();
  readonly monitoringSummary$ = new Subject<any>();
  readonly mapUpdate$ = new Subject<any>();
  readonly incidentChanged$ = new Subject<{ action: string; incident: IncidentDto }>();

  constructor(private zone: NgZone) {}

  start(): void {
    if (this.started) {
      return;
    }

    const encryptedAuthToken = new UtilsService().getCookieValue(AppConsts.authorization.encryptedAuthTokenName);
    const url = `${AppConsts.remoteServiceBaseUrl}/signalr-alerts?${AppConsts.authorization.encryptedAuthTokenName}=${encodeURIComponent(encryptedAuthToken)}`;

    this.connection = new signalR.HubConnectionBuilder().withUrl(url).withAutomaticReconnect().build();

    this.connection.on('alertChanged', (payload: { action: string; alert: AlertDto }) => {
      this.zone.run(() => this.alertChanged$.next(payload));
    });

    this.connection.on('monitoringEvent', (payload: any) => {
      this.zone.run(() => this.monitoringEvent$.next(payload));
    });

    this.connection.on('monitoringSummary', (payload: any) => {
      this.zone.run(() => this.monitoringSummary$.next(payload));
    });

    this.connection.on('mapUpdate', (payload: any) => {
      this.zone.run(() => this.mapUpdate$.next(payload));
    });

    this.connection.on('incidentChanged', (payload: { action: string; incident: IncidentDto }) => {
      this.zone.run(() => this.incidentChanged$.next(payload));
    });

    this.connection
      .start()
      .then(() => this.connection.invoke('JoinTenantAlerts'))
      .then(() => (this.started = true))
      .catch(() => undefined);
  }
}
