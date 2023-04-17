import { Component } from '@angular/core';
import {LoginService} from "../services/login.service";

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  public email:string = "";
  public password:string = "";
  constructor(private login:LoginService) {
  }
  public submit(){
    this.login.register({email: this.email, password:this.password}).subscribe((response)=>{
      console.log(response)
    })
  }
}
