import { HttpClient } from '@angular/common/http';
import { Conditional } from '@angular/compiler';
import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { lastValueFrom } from 'rxjs';
import { Nav } from "../layout/nav/nav";
import { AccountService } from '../core/services/account-service';
import { Home } from "../features/home/home";

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Nav, Home],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit{

  protected readonly title = signal('Client');
  http:HttpClient=inject(HttpClient);
  accountService=inject(AccountService)
  members:any =signal<any>([])

  async ngOnInit() {
   this.members.set(await this.getMembers())
   this.setCurrentUser()
  }
setCurrentUser(){
  const userString=localStorage.getItem('user')
  if(!userString) return;
    const user=JSON.parse(userString);
    this.accountService.currentUser.set(user);

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
