import { HttpClient, HttpParams } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { IError } from '../../app/shared/Models/basket';
import { environment } from '../../baseUrl';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-is-valid-pass-link',
  standalone: false,
  templateUrl: './is-valid-pass-link.html',
  styleUrl: './is-valid-pass-link.scss'
})
export class IsValidPassLink implements OnInit {
  /**
   *
   */

  constructor(private route: ActivatedRoute, private http: HttpClient, private router: Router) {

  }
  ngOnInit(): void {
    let email = this.route.snapshot.queryParams['email'];
    let token = this.route.snapshot.queryParams['token'];
    let queryParams = new HttpParams();
    queryParams = queryParams.set('email', email!);
    queryParams = queryParams.set('token', token!);
    this.http.get(environment.baseUrl + "Account/IsValidPassLink", { params: queryParams }).subscribe(

      {
        next: _ => {
          this.router.navigateByUrl("Account/resetPassword?email=" + email + "&token=" + token);
        },
        error: (err: IError) => {
          Swal.fire({
              icon: "error",
            title: err.error.message,
            confirmButtonText: "Enter Your Email To Send Link Again",
          }).then((result) => {
            if (result.isConfirmed) {
              this.router.navigateByUrl("Account/forgetPassword");
            }
          });
        }
      }
    );

  }
}
