import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { LoginRequest } from '../models/login-request.model';
import { RegisterRequest } from '../models/register-request.model';
import { Authresponse } from '../models/auth-response.model';

@Injectable({
  providedIn: 'root',
})
export class Auth {
  private readonly apiUrl = `${environment.apiUrl}/auth`;

  constructor(private http: HttpClient) {}

  login(request: LoginRequest): Observable<Authresponse> {
    return this.http.post<Authresponse>(`${this.apiUrl}/login`, request).pipe(
      tap((response) => {
        if (response.exito && response.token) {
          localStorage.setItem('token', response.token);
          localStorage.setItem('autUser', JSON.stringify(response));
        }
      })
    );
  }

  register(request: RegisterRequest): Observable<Authresponse> {
    return this.http.post<Authresponse>(`${this.apiUrl}/register`, request);
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('autUser');
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  getUser(): Authresponse | null {
    const userJson = localStorage.getItem('autUser');
    return userJson ? JSON.parse(userJson) : null;
  }

  getRole(): string | null {
    return this.getUser()?.rol || null;
  }
}
