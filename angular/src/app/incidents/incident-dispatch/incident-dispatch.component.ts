import { ChangeDetectorRef, Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import {
  IncidentApiService,
  IncidentDto,
  TechnicianDispatchSuggestionDto,
} from '../services/incident-api.service';

@Component({
  templateUrl: './incident-dispatch.component.html',
  animations: [appModuleAnimation()],
})
export class IncidentDispatchComponent extends AppComponentBase implements OnInit {
  incidents: IncidentDto[] = [];
  selected: IncidentDto;
  suggestions: TechnicianDispatchSuggestionDto[] = [];
  teamName = '';
  dispatchNotes = '';
  assignPersonId = '';
  loading = true;

  constructor(
    injector: Injector,
    private incidentApi: IncidentApiService,
    private cd: ChangeDetectorRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.incidentApi.getActiveIncidents().subscribe((list) => {
      this.incidents = list.filter((i) => i.status === 0 || i.status === 4);
      this.loading = false;
      this.cd.detectChanges();
    });
  }

  select(incident: IncidentDto): void {
    this.selected = incident;
    this.suggestions = [];
    this.incidentApi.getDispatchSuggestions(incident.id).subscribe((s) => {
      this.suggestions = s;
      this.cd.detectChanges();
    });
  }

  assignSuggestion(s: TechnicianDispatchSuggestionDto): void {
    if (!this.selected) {
      return;
    }
    this.incidentApi
      .assign({
        incidentId: this.selected.id,
        personId: s.personId,
        teamName: this.teamName,
        dispatchNotes: this.dispatchNotes,
      })
      .subscribe(() => {
        this.notify.success(this.l('SavedSuccessfully'));
        this.ngOnInit();
      });
  }
}
