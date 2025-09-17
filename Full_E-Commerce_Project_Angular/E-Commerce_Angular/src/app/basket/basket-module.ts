import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {MatBadgeModule} from '@angular/material/badge';
import { BasketRoutingModule } from './basket-routing-module';
import { AddToCart } from './add-to-cart/add-to-cart';
import { CartItems } from './cart-items/cart-items';
import { FormsModule } from '@angular/forms';
import { CheckOut } from './check-out/check-out';


@NgModule({
  declarations: [AddToCart,CartItems, CheckOut],
  imports: [
    CommonModule,
    BasketRoutingModule,
    FormsModule,MatBadgeModule
  ],
  exports:[
    AddToCart,CartItems
  ]
})
export class BasketModule { }
