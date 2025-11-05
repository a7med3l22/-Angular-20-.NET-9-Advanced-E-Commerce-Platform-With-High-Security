import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IDeliveryMethod, Order } from '../../shared/Models/order';
import { environment } from '../../../baseUrl';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  ///Get All DeliveryMethod
  /**
   *
   */
  //https://localhost:7260/api/Order/GetAllDeliveryMethodsAsync

  _order=new BehaviorSubject<Order[]|null>(null);
  $order=this._order.asObservable();
 _deliveryMethods=new BehaviorSubject<IDeliveryMethod[]>([]);
  $deliveryMethod=this._deliveryMethods.asObservable();
  constructor(private http:HttpClient) {
    http.get<IDeliveryMethod[]>(environment.baseUrl+"Order/GetAllDeliveryMethodsAsync").subscribe(
      val=>{
              this._deliveryMethods.next(val);
      }

    );
  }
    getOrders()
    {

         this.http.get<Order[]>(environment.baseUrl+"Order/GetAllOrdersForUserAsync").subscribe(
                val=>{
                    this._order.next(val);
                }
        
            );
    }
  }

