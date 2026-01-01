import * as signalR from '@microsoft/signalr';
import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class SignalRService {

  private hubConnection!: signalR.HubConnection;

  startConnection() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:7142/hubs/notification?userId=monem',
        { withCredentials: true }
      )
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start()
      .catch(err => console.error(err));
  }

  onMessage(callback: (msg: string) => void) {
    this.hubConnection.on('ReceiveMessage', callback);
  }
}
