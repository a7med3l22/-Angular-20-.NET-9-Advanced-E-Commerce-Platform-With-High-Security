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

import { MatBadgeModule } from '@angular/material/badge';
import { MatRadioModule } from '@angular/material/radio';
import { DeliveryMethod } from './delivery-method/delivery-method';
import { BasketModule } from "../basket/basket-module";
import { OrderSummary } from './order-summary/order-summary';
import { MatCardModule } from '@angular/material/card';


@NgModule({
  declarations: [
    NavBar,//
    Home,//
    ProductDetails,
    DeliveryMethod,
    OrderSummary

  ],
  imports: [
    MatCardModule,
    MatRadioModule, FormsModule,
    CommonModule,
    FaIconComponent,
    RouterModule,

    PaginationModule.forRoot(),
    SharedModule, FormsModule, MatBadgeModule

],
  exports:[NavBar,PaginationModule,OrderSummary ]
})
export class CoreModule { }
