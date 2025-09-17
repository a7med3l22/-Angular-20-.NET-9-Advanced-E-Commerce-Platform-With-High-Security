import { HttpClient } from '@angular/common/http';
import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BehaviorSubject, filter, map, Observable, Subscription } from 'rxjs';
import { IProduct } from '../../shared/Models/Products';
import { ShopService } from '../../../shop/shop-service';
import { Location } from '@angular/common';
import { BasketService } from '../basket-service';
import { environment } from '../../../baseUrl';

@Component({
  selector: 'app-add-to-cart',
  standalone: false,
  templateUrl: './add-to-cart.html',
  styleUrl: './add-to-cart.scss'
})
export class AddToCart implements OnInit, OnDestroy {
  /////////عاوز لما اضغط ع الصفحة التانية يجيبلي اول عنصر ف الصفحة التانية ولما اضغط ع الاولي .....
  /**
   *
   */
  // عاوز اعمل ال اي دي اوبسيرافول بحيث كل م يتغير اعرف وعاوز القيمة الابتدائية بتاعته ب صفر   
  itemCount:number=1;
  _id: BehaviorSubject<number>; // عاوز ادخل القيمة دي لل ابن بنفس الريفرانس طبعا  بحيث انه يعدل عليها لما الباجينيجن بتتغير علشان كده هعملها انبون  
  id: Observable<number>;
  product!: IProduct;
  max = false;
  min = false;
  pageNumber!: number;
  pageSize!: number;
  totalReturnedProducts!: number;
  ids!: number[];
  subscription = new Subscription();
  test:number=0;
  constructor(private basket:BasketService,private router: Router, private location: Location, private filteredProducts: ShopService, private route: ActivatedRoute, private http: HttpClient) {
    this._id = new BehaviorSubject<number>(+this.route.snapshot.paramMap.get('id')!);
    this.id = this._id.asObservable();
  ;
  }
  ngOnDestroy(): void {

    this.subscription.unsubscribe();
  }
  ngOnInit(): void {
 

    this.subscription.add(
      this.filteredProducts.getAllProducts.pipe(
        filter(

          val => {
            if (val == null) {

              this.filteredProducts.GetAllProducts();
              return false;
            } else if (val.products.length == 0) {
              return false;


            } else {
              return true;
            }

          }
        ))
        .subscribe(
          val => {


            this.pageNumber = val?.pageNumber!;
            this.pageSize = val?.pageSize!;
            this.totalReturnedProducts = val?.totalReturnedProducts!;
            this.ids = val?.products.map(p => p.id)!;
            if(this._id.value==0)
            {
              this._id.next(this.ids[0]);
            }

            let idIndex = this.ids!.findIndex(id => id == this._id.value);
            if (idIndex == 0) {
              this.min = true;
              this.max = false;
            } else if (
              (idIndex == (val?.products.length! - 1))
            ) {
              this.max = true;
              this.min = false;
            }
            if (val?.products.length == 1) {
              this.min = true;
              this.max = true;
            }
          }
          // <3
        ) // لازم اعملها علشان يروح يجيب القيم

    );


    this.subscription.add(

      this.id.subscribe( // كده لما يعمل سبسكرايب ع ال اي دي هتبقي اخر قيمة موجوده مش 0 بل القيمة اللي جات من ال يو ار ال علشان انا غيرتها ف الخطوة اللي فوق 

        id => {
          if (id == 0) {
            return;
          }
          // عاوزة لما يعمل نيكست يجيبله ال برودكت بتاع ال اي دي اللي عليه الدور ف ال برودكتات اللي راجعه من الفلتر !
          // ;
          this.http.get<IProduct>(  environment.baseUrl+"Product/" + id)

            .pipe(
              map(
                val => {

                  val.MainPhotoIndex = 0; // علشان اخليها 0 بدل انديفايند
                  return val;
                }
              )
            )
            .subscribe(
              val => {
                // ;
                this.product = val
              }
            )
        }
      )

    );









  }
addToCart()
{
  debugger;
this.basket.addItemToBasket(this.product.id,this.itemCount);
}
  backToShop()
  {
        this.router.navigateByUrl("/shop");

  }
}

