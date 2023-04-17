import { Component } from '@angular/core';
import {LoginService} from "../services/login.service";

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  public email:string = "";
  public password:string = "";

  constructor(private login:LoginService) {}

  public submit(){
    this.login.login({email:this.email, password:this.password}).subscribe((response)=>{
      console.log(response)
    })
  }
}
