import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NavBar } from './nav-bar/nav-bar';
import { FaIconComponent } from "@fortawesome/angular-fontawesome";
import { Home } from './home/home';
import { ProductDetails } from './product-details/product-details';
import { AppRoutingModule } from "../app-routing-module";
import { RouterLink, RouterModule } from '@angular/router';
import { ShopRoutingModule } from '../../shop/shop-routing-module';
import { PaginationModule } from "ngx-bootstrap/pagination";
import { FormsModule } from '@angular/forms';
import { SharedModule } from "../shared/shared-module";
import { AddToCart } from '../basket/add-to-cart/add-to-cart';
import { CartItems } from '../basket/cart-items/cart-items';
import { MatBadgeModule } from '@angular/material/badge';



@NgModule({
  declarations: [
    NavBar,//
    Home,//
    ProductDetails

  ],
  imports: [
    CommonModule,
    FaIconComponent,
    RouterModule,
    AppRoutingModule,
    ShopRoutingModule,
    PaginationModule.forRoot(),
    SharedModule,FormsModule,MatBadgeModule
],
  exports:[NavBar,PaginationModule ]
})
export class CoreModule { }
