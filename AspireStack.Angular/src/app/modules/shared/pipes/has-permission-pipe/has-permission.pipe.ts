import { Pipe, PipeTransform } from '@angular/core';
import { CurrentUserService } from '../../../../services/current-user.service';

@Pipe({
  name: 'has-permission',
  // TODO pure: false
})
export class HasPermissionPipe implements PipeTransform {
  constructor(private currentUserService: CurrentUserService) {}

  transform(permission: string): boolean {
    return this.currentUserService.hasPermission(permission);
  }
}
