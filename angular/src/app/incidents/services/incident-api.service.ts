import { Injectable, Inject, Optional } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from '@shared/service-proxies/service-proxies';

export interface EntityRef {
  id: string;
  displayText: string;
}

export interface IncidentDto {
  id: string;
  tenantId: number;
  title: string;
  description?: string;
  status: number;
  priority: number;
  slaStatus: number;
  escalationLevel: number;
  isEscalated: boolean;
  createdDate: string;
  assignedAt?: string;
  acknowledgedAt?: string;
  firstResponseAt?: string;
  resolvedDate?: string;
  responseDueAt: string;
  resolutionDueAt: string;
  slaResponseBreached: boolean;
  slaResolutionBreached: boolean;
  resolutionNotes?: string;
  assignedTeamName?: string;
  latitude?: number;
  longitude?: number;
  facilityId?: string;
  nodeId?: string;
  createdBy?: EntityRef;
  assignedTo?: EntityRef;
  facility?: EntityRef;
}

export interface IncidentDetailDto extends IncidentDto {
  timeline: IncidentTimelineEventDto[];
  assignments: IncidentAssignmentDto[];
  attachments: IncidentAttachmentDto[];
}

export interface IncidentTimelineEventDto {
  id: string;
  eventType: number;
  title: string;
  description?: string;
  creationTime: string;
  creatorUserId?: number;
}

export interface IncidentAssignmentDto {
  id: string;
  assignedPerson?: EntityRef;
  assignedTeamName?: string;
  assignedAt: string;
  unassignedAt?: string;
  dispatchNotes?: string;
  isActive: boolean;
}

export interface IncidentAttachmentDto {
  id: string;
  fileName: string;
  contentType: string;
  fileSizeBytes: number;
  caption?: string;
  creationTime: string;
}

export interface IncidentDashboardDto {
  totalActive: number;
  overdueCount: number;
  slaBreachedCount: number;
  escalatedCount: number;
  unassignedCount: number;
  averageResponseMinutes: number;
  averageResolutionMinutes: number;
  slaCompliancePercent: number;
  kanban: IncidentKanbanColumnDto[];
  technicianWorkload: TechnicianWorkloadDto[];
}

export interface IncidentKanbanColumnDto {
  status: number;
  items: IncidentDto[];
}

export interface TechnicianWorkloadDto {
  personId: string;
  displayName: string;
  activeAssignments: number;
}

export interface TechnicianDispatchSuggestionDto {
  personId: string;
  displayName: string;
  activeIncidents: number;
  distanceKm?: number;
}

export interface PagedIncidentResult {
  totalCount: number;
  items: IncidentDto[];
}

@Injectable({ providedIn: 'root' })
export class IncidentApiService {
  private baseUrl: string;

  constructor(
    private http: HttpClient,
    @Optional() @Inject(API_BASE_URL) baseUrl?: string
  ) {
    this.baseUrl = baseUrl ?? '';
  }

  getDashboard(): Observable<IncidentDashboardDto> {
    return this.http.get<IncidentDashboardDto>(`${this.baseUrl}/api/services/app/Incident/GetDashboard`);
  }

  getIncidents(params: Record<string, unknown>): Observable<PagedIncidentResult> {
    let httpParams = new HttpParams();
    Object.keys(params).forEach((key) => {
      const value = params[key];
      if (value !== undefined && value !== null) {
        httpParams = httpParams.set(key.charAt(0).toUpperCase() + key.slice(1), String(value));
      }
    });
    return this.http.get<PagedIncidentResult>(`${this.baseUrl}/api/services/app/Incident/GetIncidents`, { params: httpParams });
  }

  getIncident(id: string): Observable<IncidentDetailDto> {
    return this.http.get<IncidentDetailDto>(`${this.baseUrl}/api/services/app/Incident/GetIncident`, {
      params: new HttpParams().set('Id', id),
    });
  }

  getActiveIncidents(): Observable<IncidentDto[]> {
    return this.http.get<IncidentDto[]>(`${this.baseUrl}/api/services/app/Incident/GetActiveIncidents`);
  }

  getDispatchSuggestions(incidentId: string): Observable<TechnicianDispatchSuggestionDto[]> {
    return this.http.get<TechnicianDispatchSuggestionDto[]>(
      `${this.baseUrl}/api/services/app/Incident/GetDispatchSuggestions`,
      { params: new HttpParams().set('Id', incidentId) }
    );
  }

  create(body: Record<string, unknown>): Observable<IncidentDto> {
    return this.http.post<IncidentDto>(`${this.baseUrl}/api/services/app/Incident/Create`, body);
  }

  assign(body: { incidentId: string; personId: string; teamName?: string; dispatchNotes?: string }): Observable<IncidentDto> {
    return this.http.post<IncidentDto>(`${this.baseUrl}/api/services/app/Incident/Assign`, body);
  }

  changeStatus(body: { incidentId: string; status: number; notes?: string }): Observable<IncidentDto> {
    return this.http.post<IncidentDto>(`${this.baseUrl}/api/services/app/Incident/ChangeStatus`, body);
  }

  acknowledge(incidentId: string): Observable<IncidentDto> {
    return this.http.post<IncidentDto>(`${this.baseUrl}/api/services/app/Incident/Acknowledge`, { incidentId });
  }

  startWork(incidentId: string): Observable<IncidentDto> {
    return this.http.post<IncidentDto>(`${this.baseUrl}/api/services/app/Incident/StartWork`, { incidentId });
  }

  escalate(incidentId: string, notes?: string): Observable<IncidentDto> {
    return this.http.post<IncidentDto>(`${this.baseUrl}/api/services/app/Incident/Escalate`, { incidentId, notes });
  }

  resolve(incidentId: string, notes?: string): Observable<IncidentDto> {
    return this.http.post<IncidentDto>(`${this.baseUrl}/api/services/app/Incident/Resolve`, { incidentId, notes });
  }

  addComment(incidentId: string, comment: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/api/services/app/Incident/AddComment`, { incidentId, comment });
  }
}
