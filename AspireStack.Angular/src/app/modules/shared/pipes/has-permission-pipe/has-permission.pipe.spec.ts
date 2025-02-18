/* tslint:disable:no-unused-variable */

import { TestBed } from '@angular/core/testing';
import { HasPermissionPipe } from '../has-permission-pipe/has-permission.pipe';
import { CurrentUserService } from '../../../../services/current-user.service';

describe('Pipe: HasPermissione', () => {
  it('create an instance', () => {
    const currentUserService = jasmine.createSpyObj('CurrentUserService', ['hasPermission']);
    let pipe = new HasPermissionPipe(currentUserService);
    expect(pipe).toBeTruthy();
  });
});
