import { HttpClient, HttpParams } from '@angular/common/http';
import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { environment } from '../../baseUrl';
import Swal from 'sweetalert2';
import { IError } from '../../app/shared/Models/basket';

@Component({
  selector: 'app-forget-password',
  standalone: false,
  templateUrl: './forget-password.html',
  styleUrl: './forget-password.scss'
})
export class ForgetPassword {
  email!:string;
   
    constructor(private route:ActivatedRoute,private http:HttpClient,private router:Router) {
    
  }
ngOnInit(): void {
    
  }

  sendEmail()
  {
let queryParams=new HttpParams();
    queryParams = queryParams.set('email', this.email!);
   this.http.get(environment.baseUrl+"Account/forgetPassword",{params:queryParams}).subscribe(

{
  next:  _=>{

debugger;
   
Swal.fire({
  title: "Check Your Email To Reset PassWord",
  icon: "success",
  confirmButtonText: "Login Now!",
            }).then((result) => {
              if (result.isConfirmed) {
                this.router.navigateByUrl("Account/login");
              }
            });
    }
    ,
   error: (err: IError) => {
            Swal.fire({
                icon: "error",
              title: err.error.message,
              confirmButtonText: "Register Now!",
            }).then((result) => {
              if (result.isConfirmed) {
                this.router.navigateByUrl("Account/register");
              }
            });
    }
}
    
  
   )  
  }
}
