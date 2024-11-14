import { Injector, Pipe, PipeTransform } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { getPersonTitles } from '@shared/helpers/userHealper';
import { RefListPersonTitle } from '@shared/service-proxies/service-proxies';

@Pipe({
    name: 'title'
})
export class TitlePipe extends AppComponentBase implements PipeTransform {

    constructor(injector: Injector) {
        super(injector);
    }

    transform(value: RefListPersonTitle): string {
        const option = getPersonTitles().find(option => option.value === value);
        return option ? option.label : 'Unknown';
      }
}
