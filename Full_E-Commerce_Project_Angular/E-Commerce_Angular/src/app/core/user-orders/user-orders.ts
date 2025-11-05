import { HttpClient } from '@angular/common/http';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { environment } from '../../../baseUrl';
import { Order } from '../../shared/Models/order';
import { OrderService } from '../order/orderService';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-user-orders',
  standalone: false,
  templateUrl: './user-orders.html',
  styleUrl: './user-orders.scss'
})
export class UserOrders implements OnInit,OnDestroy{
  order:Order[]|null=null;
  subscription=new Subscription;
  constructor(private orderService:OrderService)
  {

  }
  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
  ngOnInit(): void {
   
    this.orderService.getOrders();
    this.subscription.add(
    this.orderService.$order.subscribe(
      val=>this.order=val
    ));
    
  }

}
