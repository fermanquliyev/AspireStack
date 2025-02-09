import { Injectable } from '@angular/core';
import {jwtDecode} from 'jwt-decode';
import { JwtPayload } from '../../types/JwtPayload';
import { AuthService } from './auth-service.service';
@Injectable({
  providedIn: 'root'
})
export class CurrentUserService {
  private userId: string | null = null;
  private username: string | null = null;
  private isAuthenticated: boolean = false;
  private permissions: string[] = [];
  private roles: string | string[] = [];
  private email: string = '';
  private name: string = '';
  private surname: string ='';

  constructor(private authService: AuthService) {
    this.loadUserFromToken();
  }

  public loadUserFromToken(authToken?:string): number {
    const token = authToken ?? this.authService.getAuthToken();
    if (token) {
      try {
        const decodedToken = jwtDecode<JwtPayload>(token);
        const currentUtcTime = new Date().getTime() / 1000;
        if (decodedToken.exp > currentUtcTime) {
          this.userId = decodedToken.sub;
          this.username = decodedToken.unique_name;
          this.permissions = decodedToken.permission;
          this.roles = decodedToken.role;
          this.email = decodedToken.email;
          this.name = decodedToken.name;
          this.surname = decodedToken.family_name;
          this.isAuthenticated = true;
        }
        const expInDays = (decodedToken.exp - currentUtcTime) / 86400;
        return expInDays;
      } catch (error) {
        console.error('Error decoding token', error);
      }
    }
    return 0;
  }

  public getPermissions(): string[] {
    return this.permissions;
  }

  public getRoles(): string | string[] {
    return this.roles;
  }

  public getEmail(): string {
    return this.email;
  }

  public getFullName(): string {
    return `${this.name} ${this.surname}`;
  }

  public hasPermission(permission: string): boolean {
    return this.permissions.includes(permission);
  }

  public getUserId(): string | null {
    return this.userId;
  }

  public getUsername(): string | null {
    return this.username;
  }

  public getIsAuthenticated(): boolean {
    return this.isAuthenticated;
  }

  public clear(): void {
    this.userId = null;
    this.username = null;
    this.isAuthenticated = false;
    this.permissions = [];
    this.roles = [];
    this.email = '';
    this.name = '';
    this.surname = '';
  }
}