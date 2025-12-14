import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { LoginRequest, RegisterRequest, User } from '../models/core.models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'https://localhost:7049/api/auth'; 
  
  currentUser = signal<User | null>(null);

  constructor(private http: HttpClient) {}

  register(req: RegisterRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, req);
  }

  login(req: LoginRequest): Observable<User> {
    return this.http.post<User>(`${this.apiUrl}/login`, req).pipe(
      tap(user => {
        this.currentUser.set(user);
        localStorage.setItem('user', JSON.stringify(user));
      })
    );
  }

  logout() {
    this.currentUser.set(null);
    localStorage.removeItem('user');
  }
}

