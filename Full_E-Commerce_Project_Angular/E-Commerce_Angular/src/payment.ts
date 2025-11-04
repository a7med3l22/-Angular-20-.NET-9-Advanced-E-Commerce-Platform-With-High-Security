import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from './baseUrl';
import { IOrder, IOrderBody } from './app/shared/Models/order';

@Injectable({
  providedIn: 'root'
})
export class Payment {
  constructor(private http: HttpClient) {}

  createOrUpdatePaymentIntent(basketId:string,deliveryMethodId:number,orderBody:IOrderBody) {
  return this.http.post<{ clientSecret: string, paymentIntentId: string }>(



  // `${environment.baseUrl}Payment/CreateOrUpdatePayment?deliveryMethodId=${deliveryMethodId}&basketId=${basketId}`
  
  environment.baseUrl+"Payment/CreateOrUpdatePayment?deliveryMethodId="+deliveryMethodId+"&basketId="+basketId
  ,
  orderBody
);

  }
}
