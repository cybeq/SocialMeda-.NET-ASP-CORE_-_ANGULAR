import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";

interface ILoginData {
  email:string;
  password:string;
}

@Injectable({
  providedIn: 'root'
})
export class LoginService {


  constructor(private _http:HttpClient) { }
  public login(loginData:ILoginData){
    return this._http.post("/api/auth/login", loginData);
  }
  public register(loginData:ILoginData){
    return this._http.post("/api/auth/register", loginData);
  }
}
