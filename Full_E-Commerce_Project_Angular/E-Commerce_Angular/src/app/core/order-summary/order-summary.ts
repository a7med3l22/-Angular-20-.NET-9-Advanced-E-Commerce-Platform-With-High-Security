import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { BasketService } from '../../basket/basket-service';

@Component({
  selector: 'app-order-summary',
  standalone: false,
  templateUrl: './order-summary.html',
  styleUrl: './order-summary.scss'
})
export class OrderSummary implements OnInit,OnDestroy{
  
  private _basketTotalPrice = 0;
  public get basketTotalPrice() {
    return this._basketTotalPrice;
  }
  public set basketTotalPrice(value) {
    this._basketTotalPrice = value;
  }
  DeliveryPrice?:number;
  subscription=new Subscription();
  constructor(private basket:BasketService){
   
  }
  ngOnDestroy(): void {
   this.subscription.unsubscribe();
  }
  ngOnInit(): void {
    
     this.subscription.add(
      
        this.basket.totalBasketPrice().subscribe
          (
          
            totalSum =>
              
              {
                this.basketTotalPrice = totalSum;
              }
          )
        
      );
       this.subscription.add(

        this.basket.$deliveryPrice.subscribe(

          val=>this.DeliveryPrice=val
        )

       );

  }

}
