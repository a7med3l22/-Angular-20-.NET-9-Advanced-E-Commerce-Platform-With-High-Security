import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import path from 'path';
import { Shop } from './shop';
import { Home } from '../app/core/home/home';
import { ProductDetails } from '../app/core/product-details/product-details';


const routes: Routes = 
[
{
  path:'',component:Shop,
  // طالما انا مش عامل شيلد من كمبوننت ل كمبوننت مش هيعرف ان ده ابوة و ب التالي لما اروح من الاب لل ابن الاب هيتعمله ديستوري ..

},
{
    path:'productDetails/:id',component:ProductDetails
},
{
      path:'**',component:Home

}

];
@NgModule({
  imports: [
    RouterModule.forChild(routes), // لأانه شايلد لل رووت مش رووت
    CommonModule
  ],
  exports: [RouterModule]

})
export class ShopRoutingModule { }
