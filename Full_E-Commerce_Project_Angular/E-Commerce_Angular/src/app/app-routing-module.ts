import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import path from 'path';
import { Shop } from '../shop/shop';
import { Home } from './core/home/home';
import { AboutUs } from './about-us/about-us';
import { DeliveryMethod } from './core/delivery-method/delivery-method';
import { UserOrders } from './core/user-orders/user-orders';

const routes: Routes = 
[
{path:'Account',
      loadChildren:()=>import('../account/account-module').then(m=>m.AccountModule)
},


  //[Gold]
  // x:Type<IGetAllCatrgories>=Shop;
  /*
    x:Type<IGetAllCatrgories>
    يعني ال x بتاخد كلاس بحيث ان الكلاس ده لازم يكون فيه كل ال خصائص اللي موجوده ف ال IGetAllCatrgories
    وممكن اعمل ع كلاس ال Shop implements IGetAllCatrgories 
    علشان يساعدني انه اكون متاكد ان كل الخصائص موجوده 
    -------
    ولو عملت 
    component?:Type<any>
    معناها انه بياخد كلاس بحيث ان الكلاس دي مش متحدد ان يكون فيه بروبيرتس معينه وممكن تاخد كلاس مفهوش بروبيرتس خالص كمان عادي 
  */
  {path:'shop',
    loadChildren:()=>import('../shop/shop-module').then(m=>m.ShopModule)
   //[Gold]
   // يعني اول م اروح لل باس شوب هيروح لل ShopRoutingModule وهيعمل لود لل كمبوننت اللي فيها باس فاضي ''
   // انما لو عملت shop/...  ف يروح لل ShopRoutingModule وهيحمل الكمبوننت اللي ... ولو ملقهاش لو انا عامل ** جوه ال ShopRoutingModule  هيحمل الكمبوننت دي 
  },
  {path:'basket',
    loadChildren:()=>import('../app/basket/basket-module').then(m=>m.BasketModule)
    //[Gold]// هنا لما اعمل ف الباس basket/ هيروح لل موديول باسكت وبعدين يطابق الراوت اللي ف ال راوت بتاعت الموديول ويحمل الكمبوننت الللي بيطابق الراوت ده
   //[Gold]//
   // يعني اول م اروح لل باس شوب هيروح لل ShopRoutingModule وهيعمل لود لل كمبوننت اللي فيها باس فاضي ''
   // انما لو عملت shop/...  ف يروح لل ShopRoutingModule وهيحمل الكمبوننت اللي ... ولو ملقهاش لو انا عامل ** جوه ال ShopRoutingModule  هيحمل الكمبوننت دي 
  },
  {path:'home',component:Home},

  //لما تحب تعمل Lazy Loading بتستخدم loadChildren مش loadComponent، لأن loadComponent مخصص للـ Standalone Component فقط.
    {path:'aboutUs',
      component:AboutUs
     },
 {path:'deliveryMethod',

         component:DeliveryMethod

  },
  {path:'orders',

         component:UserOrders

  },
  {path:'**',redirectTo:'home'}


];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
