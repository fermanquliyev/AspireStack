import { Injectable } from '@angular/core';
import {jwtDecode} from 'jwt-decode';
import { JwtPayload } from '../../types/JwtPayload';
import { AuthService } from './auth-service.service';
@Injectable({
  providedIn: 'root'
})
export class CurrentUserService {
  private static userId: string | null = null;
  private static username: string | null = null;
  private static isAuthenticated: boolean = false;
  private static permissions: string[] = [];
  private static roles: string | string[] = [];
  private static email: string = '';
  private static firstName: string = '';
  private static surname: string ='';
  private static allPermissions: { [key: string]: string } = {};

  constructor(private authService: AuthService) {
  }

  public static loadUserFromToken(): number {
    const token = AuthService.getAuthToken();
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
          this.firstName = decodedToken.name;
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
    return CurrentUserService.permissions;
  }

  public getRoles(): string | string[] {
    return CurrentUserService.roles;
  }

  public getEmail(): string {
    return CurrentUserService.email;
  }

  public getFullName(): string {
    return `${CurrentUserService.firstName} ${CurrentUserService.surname}`;
  }

  public hasPermission(permissionValue: string): boolean {
    let permissionKey = Object.keys(CurrentUserService.allPermissions).find(key => CurrentUserService.allPermissions[key].startsWith(permissionValue));
    return CurrentUserService.permissions.some(p=> p.toString() == permissionKey?.toString());
  }

  public getUserId(): string | null {
    return CurrentUserService.userId;
  }

  public getUsername(): string | null {
    return CurrentUserService.username;
  }

  public getIsAuthenticated(): boolean {
    return CurrentUserService.isAuthenticated;
  }

  public static setAllPermissions(permissions: { [key: string]: string }): void { 
    this.allPermissions = permissions;
  }

  public clear(): void {
    CurrentUserService.userId = null;
    CurrentUserService.username = null;
    CurrentUserService.isAuthenticated = false;
    CurrentUserService.permissions = [];
    CurrentUserService.roles = [];
    CurrentUserService.email = '';
    CurrentUserService.firstName = '';
    CurrentUserService.surname = '';
  }
}