import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidationErrors, Validators } from '@angular/forms';
import { IError, ISuccess } from '../../app/shared/Models/basket';
import { environment } from '../../baseUrl';
import Swal from 'sweetalert2';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  standalone: false,
  templateUrl: './register.html',
  styleUrl: './register.scss'
})
export class Register implements OnInit {
  registerForm!: FormGroup;
  constructor(private fb: FormBuilder,private http:HttpClient,private router:Router) {}

   ngOnInit(): void {
    this.registerForm = this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      userName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', Validators.required],

      // 👇 هنا الفورم جروب المتداخل
      userAddress: this.fb.group({
        street: [''],
        city: [''],
        state: [''],
        zipCode: [''],
        country: ['']
      })
    }, { validators: (control: AbstractControl) => {
      const password = control.get('password')?.value;
      const confirmPassword = control.get('confirmPassword')?.value;
      return password === confirmPassword ? null : { mismatch: true };
    }

    //validators دي الجديده و القديمة هي validator
   // بياخد داله  => (form: FormGroup<any>) => { mismatch: boolean; } | null
   // ف ممكن اعملها كده 
   // this.passwordMatchValidator
   // او كده
    // (form:AbstractControl)=>{
    //     return form.get('password')?.value === form.get('confirmPassword')?.value
    //   ? null : { mismatch: true };
    // }
    


    });
  }


  // passwordMatchValidator(form: AbstractControl) {
  //   return form.get('password')?.value === form.get('confirmPassword')?.value
  //     ? null : { mismatch: true };
  // }

  onSubmit() {
    if (this.registerForm.valid) {
      console.log("Form Data: ", this.registerForm.value);
      // هنا تبعت الداتا للـ API
      this.http.post<ISuccess> // الفاليو ف حالة النجاح
      (environment.baseUrl+"Account/register",this.registerForm.value).subscribe(
        {
          next:(val)=>{
          Swal.fire({
            title: "Check Your mail To Confirm Your Account",
            icon: "success",
            confirmButtonText: "Login After Confirm Your Account!",
                      }).then((result) => {
                        if (result.isConfirmed) {
                          this.router.navigateByUrl("Account/login");
                        }
                      });

          },
          error:(err:IError)=>{
            Swal.fire({
  title:err.error.message??err.error.messages,
  icon: "error",
  draggable: true
});
          }
        }
      )

    }
  }
}