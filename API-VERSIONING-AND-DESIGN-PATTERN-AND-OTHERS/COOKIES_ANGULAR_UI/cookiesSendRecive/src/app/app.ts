import { Component, OnInit, signal } from '@angular/core';
import { AuthService } from './service/auth.service';

@Component({
  selector: 'app-root',
  imports: [],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {

  constructor(private authService: AuthService) { }

  protected readonly title = signal('cookiesSendRecive');

  login() {
    this.authService.login().subscribe(res => {
      console.log(res);
    },
      err => {
        console.log(err);
      });
  }

  getProfile() {
    this.authService.getProfile().subscribe(res => {
      console.log(res);
    },
      err => {
        console.log(err);
      });
  }

  logOut() {
    this.authService.logOut().subscribe(res => {
      console.log(res);
    },
      err => {
        console.log(err);
      });
  }
}
