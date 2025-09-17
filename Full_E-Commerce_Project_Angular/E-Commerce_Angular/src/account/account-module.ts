import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AccountRoutingModule } from './account-routing-module';
import { SendEmail } from './send-email/send-email';
import { IsValidPassLink } from './is-valid-pass-link/is-valid-pass-link';
import { ResetPassword } from './reset-password/reset-password';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ForgetPassword } from './forget-password/forget-password';
import { Register } from './register/register';
import { Login } from './login/login';
import { Logout } from './logout/logout';


@NgModule({
  declarations: [
    SendEmail,
    IsValidPassLink,
    ResetPassword,
    ForgetPassword,
    Register,
    Login,
    Logout
  ],
  imports: [
    CommonModule,
    AccountRoutingModule,
    FormsModule,ReactiveFormsModule
  ]
})
export class AccountModule { }
