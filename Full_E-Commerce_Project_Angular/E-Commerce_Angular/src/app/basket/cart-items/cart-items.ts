import { Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { BasketService } from '../basket-service';
import { filter, map, Subscription } from 'rxjs';
import { Router } from '@angular/router';
import { IReturnedBasket } from '../../shared/Models/basket';

@Component({
  selector: 'app-cart-items',
  standalone: false,
  templateUrl: './cart-items.html',
  styleUrl: './cart-items.scss'
})
export class CartItems {
  /**
   *
   */
  basketItems: IReturnedBasket | null = null;
  subscription = new Subscription;
  allPrice: number | null = null;
  constructor(private router: Router, private basket: BasketService) {
    // this.basketItems=basket.Basket; // كده بيبص ع الرفرنس اللي احنا بنغير فيه 
    // this.basketItems=basket._basket.value; // كده بيبص ع الرفرنس اللي احنا بنغير فيه  // ده او اللي فوق عادي 

    /////////////////////////////
    // [Gooooooooold] // 
    // لو عندي ال اكس بيساوي 5 وخليت ال واي تساوي اللي بيساوي ال اكس اللي هي 5 ف لو غيرت ف الاكس ال واي مش هيحس 
    // وايضا لو ال اكس خليتها بيساوي خمسة وخليت ال اكس بعد شوية بتساوي ال سبعه ف ال اكس هتحس ب التغيير طبعا 
    // لو خليت ال اكس بتساوي القيمة اللي موجوده ف رفرنس xxxxx وبعد شوية غيرت القيمة اللي ف ال رفرنس xxxxx ف ال اكس مش هتحس ب التغيير الفوري لازم تحسبها تاني وتعدي عليها يعني 
    // انما لو خليت ال اكس بتساوي القيمة اللي موجوده ف رفرنس xxxxx وبعد شوية عملت رفرنس جديد yyyyy فيه قيمة تانية ف اكيد مش هيشوف لقيمة التانية اللي اتحطت ف الرفرنس الجديد ده خالص 
    // انما لو عملت بايندج ف ال html علي قيمة اكس مثلا وال اكس دي اتغيرت بعينها ف اكيد هيشوف التغيير ده 
    // دي الخلاصه الشاملة Gold
    ///////////////////////////
  }
  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }


  ngOnInit(): void {

    // this.test = this.basket.test; // مبقاش ليه لازمة ووضحت ف ال #test in basket-service.ts
   

    // this.basketItems=this.basket._basket.value  //[Gold] طبعا ده كمان ملهوش لازمة لاني كده خليت this.basketItems تبص ع رفرنس معين ولما اعمل نيكست لل _basket هتبص ع رفرنس تاني خالص ف بالتالي اي تغيير هيحصل بعد كده مش هيتم ف الرفرنس المعين ده طبعا 
    

        this.subscription.add(
     this.basket.basket$.subscribe(
      val=>
        this.basketItems=val
    )
        );




    this.subscription.add(
      this.basket.basket$.pipe(
        filter(
          Basket => Basket.basket.length > 0
        ),
        map(Basket => {
          let totalSum = Basket.basket.map(
            BasketItem => BasketItem.newPrice * BasketItem.quantity
          ).reduce((sum, current) => sum + current, 0);
          return +totalSum.toFixed(2);
        }
        )).subscribe
        (
          totalSum => this.allPrice = totalSum
        ));




if(this.basket.callMe)
{
  this.basket.getBasketIdFromLocal();
}




  }
  removeItem(id: number) {
    console.log(this.basket)
    this.basket.removeItemFromBasket(id);
  }
  changeQuantity(itemId:number,quantity: number) {
    debugger;
    this.basket.changeItemQuantity(itemId, quantity);
  }
  backToShop() {
    this.router.navigateByUrl("/shop");

  }
  decreaseQuantity(itemId:number,quantity:number)
  {
      if(quantity>1)
      {
        quantity-=1;
        this.basket.changeItemQuantity(itemId, quantity);
      }

  }
  increaseQuantity(itemId:number,quantity:number)
  {
    quantity+=1;
  this.basket.changeItemQuantity(itemId, quantity);
  }

}
