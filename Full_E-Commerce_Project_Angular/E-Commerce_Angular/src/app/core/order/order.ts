import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IDeliveryMethod } from '../../shared/Models/order';
import { environment } from '../../../baseUrl';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class Order {
  ///Get All DeliveryMethod
  /**
   *
   */
  //https://localhost:7260/api/Order/GetAllDeliveryMethodsAsync
  
 _deliveryMethods=new BehaviorSubject<IDeliveryMethod[]>([]);
  $deliveryMethod=this._deliveryMethods.asObservable();
  constructor(http:HttpClient) {
    http.get<IDeliveryMethod[]>(environment.baseUrl+"Order/GetAllDeliveryMethodsAsync").subscribe(
      val=>{
              this._deliveryMethods.next(val);
      }

    );
  }
}
