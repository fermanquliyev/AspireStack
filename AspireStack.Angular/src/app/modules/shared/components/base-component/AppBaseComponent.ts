import { inject } from "@angular/core";
import { CurrentUserService } from "../../../../services/current-user.service";
import { LocalizationService } from "src/app/services/localization/localization.service";

export abstract class AppBaseComponent {
    currentUser: CurrentUserService = inject(CurrentUserService);
    localization: LocalizationService = inject(LocalizationService);


    hasPermission(permission: string): boolean {
        return this.currentUser.hasPermission(permission);
    }

    getTranslation(key: string, ...args: any[]): string {
        return this.localization.getTranslation(key, args);
    }

    L(key: string, ...args: any[]): string {
        return this.localization.getTranslation(key, args);
    }
}
