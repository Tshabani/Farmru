import { ChangeDetectorRef, Component, EventEmitter, Injector, OnInit, output } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { CreateNode, FacilityDto, FacilityServiceProxy, GuidNullableEntityWithDisplayNameDto, NodeDto, NodeServiceProxy } from '@shared/service-proxies/service-proxies';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({  
  templateUrl: './create-node.component.html',
})
export class CreateNodeComponent extends AppComponentBase
implements OnInit {

  saving = false;
  node = new NodeDto();
  facilities: FacilityDto[] | undefined;
  selectedFacility: any;

  onSave = output<EventEmitter<any>>()

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
    this._facilityService.getAll(undefined, true, 0, 50).subscribe((result) => {
      this.facilities = result.items;           
      this.cd.detectChanges();  
    }); 
  }

  save(): void {
    this.saving = true;
    const node = new CreateNode(); 
    node.init(this.node);    

    this._nodeService
      .create(node)
      .subscribe(
        () => {
          this.notify.info(this.l('SavedSuccessfully'));
          this.bsModalRef.hide();
          this.onSave.emit(null);
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
