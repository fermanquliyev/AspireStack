import { Component, inject, signal } from '@angular/core';
import { NgStyle } from '@angular/common';
import { IconDirective } from '@coreui/icons-angular';
import {
  ContainerComponent,
  RowComponent,
  ColComponent,
  CardGroupComponent,
  TextColorDirective,
  CardComponent,
  CardBodyComponent,
  FormDirective,
  InputGroupComponent,
  InputGroupTextDirective,
  FormControlDirective,
  ButtonDirective,
} from '@coreui/angular';
import { FormsModule } from '@angular/forms';
import { HttpClient, provideHttpClient } from '@angular/common/http';
import { AuthServiceProxy } from 'src/app/services/api-services/api-service-proxies';
import { AuthService } from 'src/app/services/auth-service.service';
import { Router } from '@angular/router';
import { CurrentUserService } from 'src/app/services/current-user.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
  imports: [
    FormsModule,
    ContainerComponent,
    RowComponent,
    ColComponent,
    CardGroupComponent,
    TextColorDirective,
    CardComponent,
    CardBodyComponent,
    FormDirective,
    InputGroupComponent,
    InputGroupTextDirective,
    IconDirective,
    FormControlDirective,
    ButtonDirective,
    NgStyle,
  ],
  providers: [],
})
export class LoginComponent {
  public email = signal('');
  public password = signal('');
  constructor(
    private authserviceProxy: AuthServiceProxy,
    private authService: AuthService,
    private currentUser: CurrentUserService,
    private router: Router
  ) {}

  login(): void {
    this.authserviceProxy
      .login({
        email: this.email(),
        password: this.password(),
      })
      .subscribe({
        next: (token: string) => {
          this.authService.setAuthToken(token, 1);
          this.currentUser.loadUserFromToken(token);
          this.router.navigate(['/dashboard']);
        },
        error: alert,
      });
  }
}
