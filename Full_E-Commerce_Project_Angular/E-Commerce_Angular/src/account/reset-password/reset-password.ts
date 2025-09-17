import { HttpClient, HttpParams } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { IError } from '../../app/shared/Models/basket';
import { environment } from '../../baseUrl';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-reset-password',
  standalone: false,
  templateUrl: './reset-password.html',
  styleUrl: './reset-password.scss'
})
export class ResetPassword implements OnInit{


  newPassword!:string;
  constructor(private route:ActivatedRoute,private http:HttpClient,private router:Router) {
    
  }
  ngOnInit(): void {
  
}  
changePassword()
{
  let email= this.route.snapshot.queryParams['email'];
   let token= this.route.snapshot.queryParams['token'];
    let queryParams=new HttpParams();
    queryParams = queryParams.set('email', email!);
    queryParams = queryParams.set('token', token!);
    queryParams = queryParams.set('newPassword', this.newPassword!);
   this.http.get(environment.baseUrl+"Account/resetPassword",{params:queryParams}).subscribe(

    {
      
      next:_=>{
        // اعمله هنا انه اتغير ب نجاح وحوله لصفحة ال لوجين

        Swal.fire({
  title: "Successfully Changed Password",
  icon: "success",
  draggable: true
});

this.router.navigateByUrl('Account/login');
      },
    error:(err:IError)=>{
               Swal.fire({
     title:err.error.message??err.error.messages,
     icon: "error",
     draggable: true
   });
        
      }
    }
   );  

}
}


