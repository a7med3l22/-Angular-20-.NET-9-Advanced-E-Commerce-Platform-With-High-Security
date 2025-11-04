import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {MatBadgeModule} from '@angular/material/badge';
import { BasketRoutingModule } from './basket-routing-module';
import { AddToCart } from './add-to-cart/add-to-cart';
import { CartItems } from './cart-items/cart-items';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CoreModule } from '../core/core-module';
import {MatStepperModule} from '@angular/material/stepper';
import { MatButtonModule, MatIconAnchor } from '@angular/material/button'; // Import MatButtonModule
import { MatFormFieldModule } from '@angular/material/form-field';
import {MatInputModule} from '@angular/material/input';
import { MatRadioButton, MatRadioGroup } from '@angular/material/radio';
import { MatCard } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { CheckOut } from './check-out/check-out';

@NgModule({
  declarations: [AddToCart,CartItems, CheckOut],
  imports: [
    CommonModule,
    BasketRoutingModule,
    FormsModule,
    MatBadgeModule,
    CoreModule, 
    MatButtonModule,
    MatStepperModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatRadioButton,
    MatRadioGroup,
    MatCard,
    MatIconModule
  ],
  exports:[
    AddToCart,CartItems,CheckOut
  ]
})
export class BasketModule { }
