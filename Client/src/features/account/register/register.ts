import { Component, EventEmitter, inject, OnInit, Output, output, signal } from '@angular/core';
import { RegisterCreds, User } from '../../../types/user';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { AccountService } from '../../../core/services/account-service';
import { JsonPipe } from '@angular/common';
import { TextInput } from "../../../shared/text-input/text-input";
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, JsonPipe, TextInput],
  templateUrl: './register.html',
  styleUrl: './register.css',

})
export class Register  {

protected accountService=inject(AccountService)
protected fb=inject(FormBuilder)
protected router=inject(Router)
cancelRegister =output<boolean>();
protected credentialsForm :FormGroup =new FormGroup({});
protected profileForm: FormGroup;
currentStep=signal(1);
validationErrors=signal<string[]>([]);


constructor(){

this.credentialsForm=this.fb.group ({
    email:['',[Validators.required,Validators.email]],
    displayName:['',[Validators.required]],
    password:['',[Validators.required,Validators.minLength(4),Validators.maxLength(8)]],
    confirmPassword:['',[Validators.required,this.matchValues('password')]]
  })


  this.profileForm = this.fb.group({
  gender: ['', Validators.required],
  dateOfBirth: ['', Validators.required],
  city: ['', Validators.required],
  country: ['', Validators.required],
})
   this.credentialsForm.controls['password'].valueChanges.subscribe(()=>{
    this.credentialsForm.controls['confirmPassword'].updateValueAndValidity();
  })
}


matchValues(matchTo:string):ValidatorFn{
  return (control:AbstractControl): ValidationErrors|null =>{
  const parent=control.parent;
  if(!parent)return null;
  const matchValue=parent.get(matchTo)?.value;
  return matchValue===control.value?null:{passwordMismatch:true}
  }

}

nextStep(){
if(this.credentialsForm.valid){
  this.currentStep.update(prevStep=>prevStep+1)
}
}

prevStep(){

  this.currentStep.update(prevStep=>prevStep-1)

}

getMaxDate() {
  const today = new Date();
  today.setFullYear(today.getFullYear() - 18);
  return today.toISOString().split('T')[0];
}
 register(){
  if(this.credentialsForm.valid && this.profileForm.valid){
    const formdata={...this.credentialsForm.value,...this.profileForm.value}
this.accountService.register(formdata).subscribe({
  next: response=>{
this.router.navigateByUrl('/members');
 this.cancel()
  },
  error: error=>{
this.validationErrors.set(error);
  }
})  }

}



cancel(){
this.cancelRegister.emit(false);
}
}
