import { Injectable } from '@angular/core';
import {jwtDecode} from 'jwt-decode';
import { JwtPayload } from 'src/types/JwtPayload';
import { AuthService } from './AuthService.service';
@Injectable({
  providedIn: 'root'
})
export class CurrentUserService {
  private userId: string | null = null;
  private username: string | null = null;
  private isAuthenticated: boolean = false;

  constructor(private authService: AuthService) {
    this.loadUserFromToken();
  }

  public loadUserFromToken(authToken?:string): void {
    const token = authToken ?? this.authService.getAuthToken();
    if (token) {
      try {
        const decodedToken = jwtDecode<JwtPayload>(token);
        const currentTime = Math.floor(Date.now() / 1000);
        if (decodedToken.exp > currentTime) {
          this.userId = decodedToken.sub;
          this.username = decodedToken.name;
          this.isAuthenticated = true;
        }
      } catch (error) {
        console.error('Error decoding token', error);
      }
    }
  }

  getUserId(): string | null {
    return this.userId;
  }

  getUsername(): string | null {
    return this.username;
  }

  getIsAuthenticated(): boolean {
    return this.isAuthenticated;
  }

  clear(): void {
    this.userId = null;
    this.username = null;
    this.isAuthenticated = false;
  }
}