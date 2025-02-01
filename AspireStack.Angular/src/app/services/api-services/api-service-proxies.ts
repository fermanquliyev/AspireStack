/* AUTO-GENERATED. DO NOT EDIT. */
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

// Generic API response wrapper.
export class WebApiResult<T> {
  data?: T;
  success: boolean = false;
  message?: string;
  statusCode: number = 0;
}

//#region Interfaces

// Components/Schemas
export interface LoginRequest {
  email: string;
  password: string;
  twoFactorCode?: string;
  twoFactorRecoveryCode?: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
}

export interface WeatherForecast {
  date: string;
  temperatureC: number;
  summary?: string;
  temperatureF: number;
}

// Role related types
export interface Role {
  id: string;
  name: string;
  description: string;
  permissions: any[]; // Adjust type as needed
}

export interface DeleteRoleRequest {
  allBitsSet: string;
  variant: number;
  version: number;
}

// User related types
export interface User {
  id: string;
  username: string;
  email: string;
  phoneNumber: string;
  firstName: string;
  lastName: string;
  creationTime: string;
  lastModificationTime: string;
}

export interface CreateUserRequest {
  username: string;
  firstName: string;
  lastName: string;
  email: string;
  roleIds: any[]; // Adjust type as needed
}

export interface UpdateUserRequest {
  id: string;
  username: string;
  firstName: string;
  lastName: string;
  roleIds: any[];
}

export interface DeleteUserRequest {
  allBitsSet: string;
  variant: number;
  version: number;
}

//#endregion

//#region Service Classes

// 1. AspireStack.WebApi Service Proxy (for /weatherforecast)
@Injectable({
  providedIn: 'root'
})
export class AspireStackWebApiServiceProxy {
  constructor(private http: HttpClient) {}

  public getWeatherForecast(): Observable<WeatherForecast[]> {
    const url = `/api/weatherforecast`;
    return this.http.get<WebApiResult<WeatherForecast[]>>(url).pipe(
      map(result => {
        if (result.success) {
          return result.data as WeatherForecast[];
        }
        throw new Error(result.message || 'API error');
      })
    );
  }
}

// 2. Auth Service Proxy (for /Auth endpoints)
@Injectable({
  providedIn: 'root'
})
export class AuthServiceProxy {
  constructor(private http: HttpClient) {}

  public login(body: LoginRequest): Observable<any> {
    const url = `/api/Auth/login`;
    return this.http.post<WebApiResult<any>>(url, body).pipe(
      map(result => {
        if (result.success) {
          return result.data;
        }
        throw new Error(result.message || 'API error');
      })
    );
  }

  public register(body: RegisterRequest): Observable<any> {
    const url = `/api/Auth/register`;
    return this.http.post<WebApiResult<any>>(url, body).pipe(
      map(result => {
        if (result.success) {
          return result.data;
        }
        throw new Error(result.message || 'API error');
      })
    );
  }

  public currentUser(): Observable<any> {
    const url = `/api/Auth/currentUser`;
    // No request body is specified so we send an empty object.
    return this.http.post<WebApiResult<any>>(url, {}).pipe(
      map(result => {
        if (result.success) {
          return result.data;
        }
        throw new Error(result.message || 'API error');
      })
    );
  }
}

// 3. Role Service Proxy (for /Role endpoints)
@Injectable({
  providedIn: 'root'
})
export class RoleServiceProxy {
  constructor(private http: HttpClient) {}

  public getRole(id: string): Observable<Role> {
    const url = `/api/Role/GetRole`;
    const params = new HttpParams().set('id', id);
    return this.http.get<WebApiResult<Role>>(url, { params }).pipe(
      map(result => {
        if (result.success) {
          return result.data as Role;
        }
        throw new Error(result.message || 'API error');
      })
    );
  }

  public getAllRoles(): Observable<Role[]> {
    const url = `/api/Role/GetAllRoles`;
    return this.http.get<WebApiResult<Role[]>>(url).pipe(
      map(result => {
        if (result.success) {
          return result.data as Role[];
        }
        throw new Error(result.message || 'API error');
      })
    );
  }

  public createRole(role: Role): Observable<Role> {
    const url = `/api/Role/CreateRole`;
    return this.http.post<WebApiResult<Role>>(url, role).pipe(
      map(result => {
        if (result.success) {
          return result.data as Role;
        }
        throw new Error(result.message || 'API error');
      })
    );
  }

  public updateRole(role: Role): Observable<Role> {
    const url = `/api/Role/UpdateRole`;
    return this.http.put<WebApiResult<Role>>(url, role).pipe(
      map(result => {
        if (result.success) {
          return result.data as Role;
        }
        throw new Error(result.message || 'API error');
      })
    );
  }

  public deleteRole(body: DeleteRoleRequest): Observable<any> {
    const url = `/api/Role/DeleteRole`;
    return this.http.request<WebApiResult<any>>('delete', url, { body }).pipe(
      map(result => {
        if (result.success) {
          return result.data;
        }
        throw new Error(result.message || 'API error');
      })
    );
  }
}

// 4. Users Service Proxy (for /Users endpoints)
@Injectable({
  providedIn: 'root'
})
export class UsersServiceProxy {
  constructor(private http: HttpClient) {}

  public getUsers(): Observable<User[]> {
    const url = `/api/Users/GetUsers`;
    return this.http.get<WebApiResult<User[]>>(url).pipe(
      map(result => {
        if (result.success) {
          return result.data as User[];
        }
        throw new Error(result.message || 'API error');
      })
    );
  }

  public getUserById(id: string): Observable<User> {
    const url = `/api/Users/GetUserById`;
    const params = new HttpParams().set('id', id);
    return this.http.get<WebApiResult<User>>(url, { params }).pipe(
      map(result => {
        if (result.success) {
          return result.data as User;
        }
        throw new Error(result.message || 'API error');
      })
    );
  }

  public createUser(body: CreateUserRequest): Observable<any> {
    const url = `/api/Users/CreateUser`;
    return this.http.post<WebApiResult<any>>(url, body).pipe(
      map(result => {
        if (result.success) {
          return result.data;
        }
        throw new Error(result.message || 'API error');
      })
    );
  }

  public updateUser(body: UpdateUserRequest): Observable<any> {
    const url = `/api/Users/UpdateUser`;
    return this.http.put<WebApiResult<any>>(url, body).pipe(
      map(result => {
        if (result.success) {
          return result.data;
        }
        throw new Error(result.message || 'API error');
      })
    );
  }

  public deleteUser(body: DeleteUserRequest): Observable<any> {
    const url = `/api/Users/DeleteUser`;
    return this.http.request<WebApiResult<any>>('delete', url, { body }).pipe(
      map(result => {
        if (result.success) {
          return result.data;
        }
        throw new Error(result.message || 'API error');
      })
    );
  }
}

//#endregion
