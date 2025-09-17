import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { IsAuth } from '../is-auth';
import { faL } from '@fortawesome/free-solid-svg-icons';
import { environment } from '../../baseUrl';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-logout',
  standalone: false,
  templateUrl: './logout.html',
  styleUrl: './logout.scss'
})
export class Logout implements OnInit{
/**
 *
 */
constructor(private http:HttpClient,private router:Router,private isAuth:IsAuth) {
 
  
}
  ngOnInit(): void {
    this.http.post(
      environment.baseUrl+"Account/logout",{}
    ).subscribe(
     _ => {
      this.isAuth._isAuth.next(false);
       Swal.fire({
        title:"Logged out successfully",
        icon: "success",
        draggable: true,
        showConfirmButton: false,
          timer: 1500
      
      });
      // لو عايز تعمل redirect بعد اللوج أوت
      this.router.navigate(['Account/login']);
    })
}
  }

