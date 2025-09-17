import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { IError, ISuccess } from '../../app/shared/Models/basket';
import { IsAuth } from '../is-auth';
import { environment } from '../../baseUrl';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-login',
  standalone: false,
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class Login implements OnInit {
  loginForm!: FormGroup;

  constructor(private fb: FormBuilder, private http: HttpClient, private router: Router,private isAuth:IsAuth) {}

  ngOnInit(): void {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
      saveMe: [false]
    });
  }

  onSubmit(): void {
    debugger;
    if (this.loginForm.invalid)
    {
      return;
    }

    const loginDto = this.loginForm.value;

    this.http.post<ISuccess>(environment.baseUrl+'Account/login', loginDto).subscribe({
      next: (res) => {
              this.isAuth._isAuth.next(true);


              Swal.fire({
  title: res.message,
  icon: "success",
  draggable: true,
  showConfirmButton: false,
    timer: 1500

});
      
        this.router.navigate(['/shop']);
      },
      error: (err:IError) => {
        debugger;
        // Show error from backend
                      Swal.fire({
  title: err.error?.message,
  icon: "error",
  draggable: true
});
      
      }
    });
  }
}