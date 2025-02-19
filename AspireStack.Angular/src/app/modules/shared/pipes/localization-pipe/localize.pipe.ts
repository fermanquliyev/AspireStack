import { inject, Pipe, PipeTransform } from '@angular/core';
import { LocalizationService } from 'src/app/services/localization/localization.service';

@Pipe({
  name: 'localize'
})
export class LocalizePipe implements PipeTransform {

  localizationService: LocalizationService = inject(LocalizationService);

  transform(value: string, args?: any): string {
    return this.localizationService.getTranslation(value, args);
  }

}
