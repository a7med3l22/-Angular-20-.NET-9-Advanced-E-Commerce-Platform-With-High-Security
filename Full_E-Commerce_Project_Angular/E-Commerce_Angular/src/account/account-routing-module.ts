import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SendEmail } from './send-email/send-email';
import { Home } from '../app/core/home/home';
import { ResetPassword } from './reset-password/reset-password';
import { IsValidPassLink } from './is-valid-pass-link/is-valid-pass-link';
import { ForgetPassword } from './forget-password/forget-password';
import { Register } from './register/register';
import { Login } from './login/login';
import { Logout } from './logout/logout';

const routes: Routes = [
{path:'SendEmail',component:SendEmail},
{path:'resetPassword',component:ResetPassword},
{path:'isValidPassLink',component:IsValidPassLink},
{path:'forgetPassword',component:ForgetPassword},
{path:'register',component:Register},
{path:'login',component:Login},
{path:'logout',component:Logout},
{ path:'**',component:Home}
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AccountRoutingModule { }
