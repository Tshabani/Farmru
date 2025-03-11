import { ChangeDetectorRef, Component, EventEmitter, Injector, OnInit, Output, output } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/app-component-base';
import { forEach as _forEach, map as _map } from 'lodash-es';
import { FacilityDto, FacilityServiceProxy, GuidNullableEntityWithDisplayNameDto, NodeDto, NodeServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  templateUrl: './edit-node.component.html',
})
export class EditNodeComponent extends AppComponentBase
  implements OnInit {
    id: string;
    saving = false;
    node = new NodeDto();  
    facilities: FacilityDto[] | undefined;
    selectedFacility: any;

    @Output() onSave = new EventEmitter<any>();
  
    constructor(
      injector: Injector,
      private _nodeService: NodeServiceProxy,
      private _facilityService: FacilityServiceProxy,
      public bsModalRef: BsModalRef,
      private cd: ChangeDetectorRef
    ) {
      super(injector);
    }
  
    ngOnInit(): void {
      this._nodeService.get(this.id).subscribe((result) => {
        this.node = result; 
        this.selectedFacility = result.facility ? result.facility.id : null;   
        this.cd.detectChanges();  
      });

      this._facilityService.getAll(undefined, true, 0, 50).subscribe((result) => {
        this.facilities = result.items;     
            
        this.cd.detectChanges();  
      }); 
    }
      
    save(): void {
      this.saving = true;  
      const node = new NodeDto();
      node.init(this.node);
  
      this._nodeService
        .update(node)
        .subscribe(
          () => {
            this.notify.info(this.l('SavedSuccessfully'));
            this.bsModalRef.hide();   
            this.onSave.emit();         
          },
          () => {
            this.saving = false;
            this.cd.detectChanges();
          }
        );
    }

    trackById(index: number, item: any): any { 
        return item.id;
      }
    
      onFaclityChange(selectedFacilityId: string): void {
          const selectedFacil = this.facilities.find(facility => facility.id === selectedFacilityId);
    
          if (!this.node.facility) {
            this.node.facility = new GuidNullableEntityWithDisplayNameDto;
          }
                           
          if (selectedFacil) {
            this.node.facility.id = selectedFacil.id;
            this.node.facility.displayText = selectedFacil.name;
          } else {
            this.node.facility.id = null;
            this.node.facility.displayText = null;
          }
        }
}
