import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { loadStripe, Stripe, StripeCardElement } from '@stripe/stripe-js';
import { Payment } from '../../../payment';
import { environment } from '../../../baseUrl';
import { IMainAddress, ISetMainAddress } from '../../shared/Models/mainAddress';
import { IDeliveryMethod, IOrder, IOrderBody } from '../../shared/Models/order';
import { BasketService } from '../basket-service';
import { Router } from '@angular/router';
import Swal from 'sweetalert2';
import { OrderService } from '../../core/order/orderService';
import { firstValueFrom, timer } from 'rxjs';

@Component({
  selector: 'app-check-out',
  templateUrl: './check-out.html',
  styleUrls: ['./check-out.scss'],
  standalone: false
})
export class CheckOut implements OnInit {
  stripe: Stripe | null = null;
  cardElement!: StripeCardElement;
  clientSecret!: string;
  paymentError = '';
  deliveryMethods: IDeliveryMethod[] = [];
  order: IOrder = {};
  orderBody: IOrderBody = {};
  addressForm: FormGroup;

  constructor(private orderService:OrderService, private router: Router, private fb: FormBuilder, private http: HttpClient, private paymentService: Payment, private basket: BasketService) {
    this.addressForm = fb.group({
      street: ['', Validators.required],
      city: ['', Validators.required],
      state: ['', Validators.required],
      zipCode: ['', Validators.required],
      country: ['', Validators.required]
    });
  }

  async ngOnInit() {
    this.stripe = await loadStripe(environment.stripePublicKey);
    const elements = this.stripe!.elements();
    this.cardElement = elements.create('card');
    this.cardElement.mount('#card-element');
    // debugger;
    this.order.basketId = localStorage.getItem('basketId');



    this.http.get<IMainAddress>(environment.baseUrl + 'Order/AddressByUserId').subscribe(val => {
      this.addressForm.patchValue(val);
    });

    this.http.get<IDeliveryMethod[]>(environment.baseUrl + 'Order/GetAllDeliveryMethodsAsync').subscribe(
      val => this.deliveryMethods = val
    );
  }


  updateDeliveryMethods(deliveryMethodId: number)
  {
    debugger;
    //هجيب السعر من ال اي دي 
    this.http.get<IDeliveryMethod>(environment.baseUrl + 'Order/GetDeliveryById?id=' + deliveryMethodId).subscribe(


      val => {
        this.basket._deliveryPrice.next(val.price);
      }
    );
  }
  
  saveAsNewAddress() {
    if (this.addressForm.valid) {
      this.http.post<ISetMainAddress>(environment.baseUrl + 'Order/SaveNewMainAddress', this.addressForm.value)
        .subscribe(() => console.log('✅ Address saved'));
    }
  }

  saveAsDeliveryAddress() {
    if (this.addressForm.valid) {
      this.orderBody = { ...this.orderBody, ...this.addressForm.value };
      console.log(this.orderBody);

    }
  }



  async intentToPayment():Promise<void> {

    //حوّل الكود جوا intentToPayment() إلى Promise فعلي باستخدام firstValueFrom() من RxJS بدل subscribe. علشان ينتظر 


    const result=await firstValueFrom(

      this.paymentService.createOrUpdatePaymentIntent(this.order.basketId!, this.order.deliveryMethodId!, this.orderBody!)

    );
    this.clientSecret=result.clientSecret;


    
  }

  async submitPayment():Promise<void>  {



 

    ///////////////
     await this.intentToPayment(); // علشان متنفذش اللي بعدها غير لما الميثود دي تخلص اللي جواها 
    ///////////
    if (!this.stripe || !this.clientSecret) return;

    const result = await this.stripe.confirmCardPayment(this.clientSecret, {
      payment_method: {
        card: this.cardElement
      }
    });

    if (result.error) {

      this.paymentError = result.error.message ?? 'Payment failed.';


    } else if (result.paymentIntent?.status === 'succeeded') {




      //هعمله شكل جمالي وهروح لل هوم وافضي الواحد بتاعت الباسكت 
      this.basket._basketItems.next(null);
      this.basket._basket.next({basket:[]});
      localStorage.removeItem('basketId');

      Swal.fire({
        title: '✅ Payment successful!',
        icon: "success",
        draggable: true,
        showConfirmButton: false,
        timer: 1500

      });
    timer(2000).subscribe(_ => {
      this.router.navigateByUrl("/orders");
    });

    }
  }
}
