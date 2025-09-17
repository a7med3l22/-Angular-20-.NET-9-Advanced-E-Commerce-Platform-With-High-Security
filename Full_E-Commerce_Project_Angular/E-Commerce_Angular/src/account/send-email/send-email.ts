import { HttpClient, HttpParams } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { IError } from '../../app/shared/Models/basket';
import { environment } from '../../baseUrl';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-send-email',
  standalone: false,
  templateUrl: './send-email.html',
  styleUrl: './send-email.scss'
})
export class SendEmail implements OnInit {
  /**
   *
   */


  
  constructor(private route: ActivatedRoute,private http:HttpClient,private router:Router) {
   
  }
  ngOnInit(): void {
debugger;
   // http://localhost:4200/Account/SendEmail?
   // email=justbewithgod@gmail.com
   // &token=CfDJ8ElH2e8ui%20hJohvSE1m7mWS%20dweUfwmhVbsRmfaF02wvnpWXc1qLwo%202wbNGqkI0cXHjhHgx1uy5fG%2FZkL5QYBetMgq2o2WblkXvKL9xY3pnuRkP9gSCIyXu9wDzbMlWT20%20wIvNlmpjUc4Ip%20MgRckOZiv8arCEeRP65%209G08dSd2HdlxJiyQp4hrA9c1dvAKYw32Yq3U1unev%20ZwZjDDjHcuK19fjPYOqD97ZD7EoCoNFOiuCoCKG9MvMhwiY9fg%3D%3D
    let email= this.route.snapshot.queryParams['email'];
   let token= this.route.snapshot.queryParams['token'];
    let queryParams=new HttpParams();
    queryParams = queryParams.set('email', email!);
    queryParams = queryParams.set('token', token!);
   this.http.get(environment.baseUrl+"Account/VerifyEmail",{params:queryParams}).subscribe(

    {
      next:_=>{
               Swal.fire({
                title:"Confirmed Successfully",
                icon: "success",
                draggable: true,
                showConfirmButton: false,
                  timer: 1500
              
              });
              // لو عايز تعمل redirect بعد اللوج أوت
              this.router.navigate(['Account/login']);
      },
      error:(err:IError)=>{
         Swal.fire({
                title:err.error.message,
                icon: "error",
                draggable: true,
                showConfirmButton: false,
                  timer: 1500
              
              });
       
              this.router.navigate(['Account/login']);
     
      }
    }
   );
  }
}
