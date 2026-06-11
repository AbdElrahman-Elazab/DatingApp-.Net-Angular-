import { HttpClient } from '@angular/common/http';
import { Conditional } from '@angular/compiler';
import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { lastValueFrom } from 'rxjs';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit{

  protected readonly title = signal('Client');
  http:HttpClient=inject(HttpClient);
  members:any =signal<any>([])

  async ngOnInit() {
   this.members.set(await this.getMembers())
  }

 async getMembers(){
    try {
return lastValueFrom(this.http.get('https://localhost:5001/api/members'))
    } catch (error) {
console.log(error)
throw error;
    }
  }

}
