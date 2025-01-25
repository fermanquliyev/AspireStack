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
import { WebApiResult } from '../../../../types/WebApiResult';
import { AuthService } from 'src/app/services/AuthService.service';
import { Router } from '@angular/router';
import { WeatherForecasts } from 'src/types/weatherForecast';
import { CurrentUserService } from 'src/app/services/CurrentUser.service';

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
    private httpClient: HttpClient,
    private authService: AuthService,
    private currentUser: CurrentUserService,
    private router: Router
  ) {
  }

  login(): void {
    this.httpClient
      .post<WebApiResult<string>>('/api/Auth/login', {
        email: this.email(),
        password: this.password(),
      })
      .subscribe({
        next: (result) => {
          if (result.success && result.data) {
            this.authService.setAuthToken(result.data, 1);
            this.currentUser.loadUserFromToken(result.data);
            this.router.navigate(['/dashboard']);
          } else {
            console.error('Failed to login', result);
            alert(result.message);
          }
        },
        error: console.error,
      });
  }
}
