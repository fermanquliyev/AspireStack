import { inject } from "@angular/core";
import { CurrentUserService } from "../../../../services/current-user.service";

export class AppBaseComponent {
    currentUser: CurrentUserService = inject(CurrentUserService);


    hasPermission(permission: string): boolean {
        return this.currentUser.hasPermission(permission);
    }
}
