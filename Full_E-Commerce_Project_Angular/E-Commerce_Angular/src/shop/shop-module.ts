import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ShopRoutingModule } from './shop-routing-module';
import { Shop } from './shop';
import { ProductDetails } from '../app/core/product-details/product-details';
import { SharedModule } from '../app/shared/shared-module';
import { FormsModule } from '@angular/forms';

@NgModule({
  declarations: [
    Shop
    
  ],
  imports: [
    CommonModule,
    ShopRoutingModule,
    SharedModule,
    FormsModule
  ],exports:[
    Shop
  ]
})
export class ShopModule { }
