import { Component, OnInit, signal } from '@angular/core';
import { AuthService } from './service/auth.service';
import { SignalRService } from './service/signal-r.service';

@Component({
  selector: 'app-root',
  imports: [],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App implements OnInit {
  message:string='';
  constructor(private authService: AuthService, private signalRService: SignalRService) { }
  ngOnInit(): void {
    this.signalRService.startConnection();

    this.signalRService.onMessage(message => {
      this.message= this.message + ' \n ' +message;
    });
  }

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
