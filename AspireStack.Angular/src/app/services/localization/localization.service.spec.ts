/* tslint:disable:no-unused-variable */

import { TestBed, inject } from '@angular/core/testing';
import { LocalizationService } from './localization.service';

describe('Service: Localization', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [LocalizationService]
    });
  });

  it('should ...', inject([LocalizationService], (service: LocalizationService) => {
    expect(service).toBeTruthy();
  }));
});
