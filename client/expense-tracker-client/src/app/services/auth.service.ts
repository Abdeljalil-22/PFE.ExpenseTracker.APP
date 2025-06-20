import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';
import { LoginCommand, RegisterCommand, AuthResponse } from '../models/auth.models';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUserSubject = new BehaviorSubject<string | null>(localStorage.getItem('userId'));
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient) {}

  login(command: LoginCommand): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/api/Auth/login`, command)
      .pipe(
        tap(response => {
          localStorage.setItem('token', response.token);
          localStorage.setItem('userId', response.userId);
          this.currentUserSubject.next(response.userId);
        })
      );
  }

  register(command: RegisterCommand): Observable<void> {
    return this.http.post<void>(`${environment.apiUrl}/api/Auth/register`, command);
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('userId');
    this.currentUserSubject.next(null);
  }

  isAuthenticated(): boolean {
    return !!localStorage.getItem('token');
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }
}
