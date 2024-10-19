import {
  Component,
  ChangeDetectionStrategy,
  Injector,
  OnInit
} from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';

@Component({
  selector: 'header-user-panel',
  templateUrl: './header-user-panel.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class HeaderUserPanelComponent extends AppComponentBase
  implements OnInit {
  shownLoginName = '';

  constructor(injector: Injector) {
    super(injector);
  }

  ngOnInit() {
    this.shownLoginName = this.appSession.getShownLoginName();
  }
}
