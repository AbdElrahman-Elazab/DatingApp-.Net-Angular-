import { HttpClient } from '@angular/common/http';
import { Component, inject, signal, Signal } from '@angular/core';

@Component({
  selector: 'app-test-errors',
  imports: [],
  templateUrl: './test-errors.html',
  styleUrl: './test-errors.css',
})
export class TestErrors {
  httpClient=inject(HttpClient);
  baseUrl ="https://localhost:5001/api"
  validationErrors=signal<string[]>([]);

  get404Error(){
    this.httpClient.get(this.baseUrl+"/buggy/not-found").subscribe({
      next:response=>console.log(response),
      error:error=>console.log(error)
    })
  }

  get400Error(){
    this.httpClient.get(this.baseUrl+"/buggy/bad-request").subscribe({
      next:response=>console.log(response),
      error:error=>console.log(error)
    })
  }

  get500Error(){
    this.httpClient.get(this.baseUrl+"/buggy/server-error").subscribe({
      next:response=>console.log(response),
      error:error=>console.log(error)
    })
  }


  get401Error(){
    this.httpClient.get(this.baseUrl+"/buggy/auth").subscribe({
      next:response=>console.log(response),
      error:error=>console.log(error)
    })
  }


  get400ValidationError(){
    this.httpClient.post(this.baseUrl+"/Account/register",{}).subscribe({
      next:response=>console.log(response),
      error:error=>{
        console.log(error);
        this.validationErrors.set(error)
      }
    })
  }
}
