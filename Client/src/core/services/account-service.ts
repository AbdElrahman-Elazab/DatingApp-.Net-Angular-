import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { RegisterCreds, User } from '../../types/user';
import { tap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  baseUrl='https://localhost:5001/api/';
  http=inject(HttpClient)
   currentUser=signal<User|null>(null);

   register(creds:RegisterCreds){
return this.http.post<User>(this.baseUrl+'account/register',creds).pipe(
    tap(user=>{
      if(user){
        this.setCurrentUser(user)

      }
    })
  );
   }

  login(creds:any){
  return this.http.post<User>(this.baseUrl+'account/login',creds).pipe(
    tap(user=>{
      if(user){
          this.setCurrentUser(user)
      }
    })
  );
  }

  setCurrentUser(user:User){
    localStorage.setItem('user',JSON.stringify(user))
    this.currentUser.set(user);

  }

  logOut(){
    localStorage.removeItem('user')
    this.currentUser.set(null);
  }
}
