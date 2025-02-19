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
import { AuthService } from '../../../services/auth-service.service';
import { Router } from '@angular/router';
import { CurrentUserService } from '../../../services/current-user.service';
import { ApiService, LoginRequest } from '../../../services/api-services/api-service-proxies';

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
    private client: ApiService,
    private authService: AuthService,
    private currentUser: CurrentUserService,
    private router: Router
  ) {}

  login(): void {
    this.client
      .login( new LoginRequest({ email: this.email(), password: this.password() }))
      .subscribe(response=>{
        const token = response.data ?? '';
        this.authService.setAuthToken(token);
        window.location.href = '/dashboard';
      });
  }
}
