import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CartItems } from './cart-items/cart-items';
import { AddToCart } from './add-to-cart/add-to-cart';
import { CheckOut } from './check-out/check-out';

const routes: Routes = [
{ path: '', component: CartItems },
  { path: 'cartItems', component: CartItems },
  { path: 'addToCart/:id', component: AddToCart },
  { path: 'checkOut', component: CheckOut }

];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class BasketRoutingModule { }
