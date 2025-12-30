import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  constructor(private http: HttpClient) { }

  login() {
    return this.http.post(
      'https://localhost:7142/api/Login/login',
      { username: 'admin', password: '1234' },
      { withCredentials: true }
    );
  }

  getProfile() {
    return this.http.get(
      'https://localhost:7142/api/Login/profile',
      { withCredentials: true }
    );
  }

  logOut() {
    return this.http.get(
      'https://localhost:7142/api/Login/logout',
      { withCredentials: true }
    );
  }
}
