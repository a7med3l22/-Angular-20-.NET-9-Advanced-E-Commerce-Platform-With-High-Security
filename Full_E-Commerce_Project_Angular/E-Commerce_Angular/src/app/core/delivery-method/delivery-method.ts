import { Component, OnDestroy, OnInit } from '@angular/core';
import { OrderService } from '../order/orderService';
import { IDeliveryMethod } from '../../shared/Models/order';
import { BehaviorSubject, Observable, Subscription } from 'rxjs';
import { BasketService } from '../../basket/basket-service';

@Component({
  selector: 'app-delivery-method',
  standalone: false,
  templateUrl: './delivery-method.html',
  styleUrl: './delivery-method.scss'
})
export class DeliveryMethod implements OnInit,OnDestroy{

  subscription=new Subscription();
  // selectedDeliveryMethodId?: number;
    _selectedDeliveryMethodId!:BehaviorSubject<number | undefined>;
    $selectedDeliveryMethodId!:Observable<number | undefined>;
    deliveryMethods:IDeliveryMethod[]=[];

  constructor(private order:OrderService,private basket:BasketService) {
       
    
  }
  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
  ngOnInit(): void {
       this._selectedDeliveryMethodId=new BehaviorSubject<number|undefined>(this.basket._deliveryPrice.value);
       this.$selectedDeliveryMethodId=this._selectedDeliveryMethodId.asObservable();
    this.subscription.add(
 this.order.$deliveryMethod.subscribe(
      val=>this.deliveryMethods=val
    ));  
  
    this.subscription.add(
      this.$selectedDeliveryMethodId.subscribe(
          val=>
          {
debugger;
            this.basket._deliveryPrice.next(val)
          }
      )

    );
  
  }
  changeDeliveryMethodPrice(price:number)
  {
    debugger;
    this._selectedDeliveryMethodId.next(price);
  }
}