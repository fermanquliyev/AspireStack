import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app.component';
import { CurrentUserService } from './app/services/current-user.service';
import { getAllPermissions, gettingTranslations } from './initialization-functions';

const environmentInitializer = () => {
  CurrentUserService.loadUserFromToken();
  getAllPermissions();
  gettingTranslations();
}

environmentInitializer();

bootstrapApplication(AppComponent, appConfig)
  .catch((err) => console.error(err));
