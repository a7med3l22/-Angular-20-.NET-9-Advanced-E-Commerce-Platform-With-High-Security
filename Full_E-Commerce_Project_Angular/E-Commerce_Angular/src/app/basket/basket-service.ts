import { Injectable, OnInit } from '@angular/core';
import { BehaviorSubject, filter, map, Observable } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Guid } from 'guid-typescript';
import { IAddBasket, IAddBasketItem, IError, IReturnedBasket, IReturnedBasketItem } from '../shared/Models/basket';
import { environment } from '../../baseUrl';

@Injectable({
  providedIn: 'root'
})
export class BasketService {
  /**
   *
   */
  // deliveryPrice?:number;
  _deliveryPrice=new BehaviorSubject<number|undefined>(undefined);
  $deliveryPrice=this._deliveryPrice.asObservable();



  _basketItems=new BehaviorSubject<number|null>(null);
  $basketItems=this._basketItems.asObservable();

  callMe = true;

  error: string | null = null;

  // نفعت عاااش 

  //فهمك صح بنسبة ✅ 100% 💯
  // هام جدا جدا جدا [Very Gold Gold Gold]
  //لو ال  فريابول اكس بتبص علي رفرنس xxx وال ال فريابول واي بتبص علي الرفرنس اللي بتبص عليه ال اكس اللي هو xxx
  //ف لو الرفرنس xxx يحتوي علي فريابول بيبص علي رفرنس yyy وانا غيرت ال رفرنس yyy ل zzz
  //ف بالتالي لو جيت اجيب قيمة ال فريابول اللي كان بيشاور علي yyy وبقي بيشاور علي ال zzz من طريق ال فريابول y اللي هو بيبص علي رفرنس xxx اللي بقي يحتوي علي فريابول بيبص علي zzz ف ب التالي ال فريابول اللي بقي يبص علي ال zzz قيمته هتبقي ب القيمة اللي ف ال zzz اكيد 

  // وبالمثل لو معايا فريابول اكس بيبص علي رفرنس والرفرنس ده فيه فريابول فيه فاليو تايب 5 مثلا 
  // ولو انا عندي فريابول y بيبص علي الرفرنس اللي بيبص عليه ال فريابول اكس 
  // ف لو غيرت القيمة بتاعت ال فريابول اللي موجوده ف الرفرنس اللي بيبص عليه ال اكس وال واي
  // ف لو جيت اجيب قيمة ال فريابول ده من ال اكس او من ال واي هلاقيه ب اخر قيمة محطوطه طبعا لانهم بيبصوا علي نفس الرفرنس والفريابول اللي ف الرفرنس ده قيمته اتغيرت ف بالتالي لما اجي اجيب قيمته من ال اكس او ال واي الاقيها متغيره 

  #test: IReturnedBasket = { basket: [] }; // [Gold] طالما عملت نيكست للباسكت ف انا خليته يبص ع رفرنس غير اللي بيبص عليه ال تيست ف بكده ال تيست ملهوش اي لازمه لاني ف الاول لما كنت بخليهم يبصوا ع نفس الرفرنس ف ب التالي اي فريابول هيتغير جوه الرفرنس ده اقدر احس بيه طبعا لاني كنت هوصل لل فريابول ده عن طريق نفس الرفرنس 
  _basket = new BehaviorSubject<IReturnedBasket>(this.#test);
  basket$ = this._basket.asObservable();
  constructor(private http: HttpClient) {
    // متستخدمش ngOnInit ف ال سيرفس لانها مش بتستخدم مع السيرفس
  }
  getBasketIdFromLocal() {
    //Get Basket Id From Local 
    const basketId = localStorage.getItem('basketId');
    if (basketId) {
      this.http.get<IReturnedBasket>(
        environment.baseUrl + "BasketRedis?basketId=" + basketId
      ).pipe(
        map(
          val => {
            val.basket.forEach(x => x.MainPhotoIndex = 0)
            return val
          }
        )
      ).subscribe(

        {
          next: (val) => {
            
            this._basket.next(val);  // لازم اعملها اوبسيرفابول بقي علشان ال allPrice اللي ف ال cart-items.ts يتبعتلها اخر تحديث 
            // this.test.basket=val.basket; // حطها  فوق عن طريق ال نيكست افضل علشان لو حاجة عاملة سبسكرايب علي ال اوبسيرفابول تروحلها 

          },
          error: (error: IError) => {
            this.error = error.error.message;
          }
        }
      )
        ;
    }
  }
  convertReturnedBasketToAddBasket(returnedBasketItems: IReturnedBasketItem[]): IAddBasketItem[] {
    let addBasketItems: IAddBasketItem[] = returnedBasketItems.map(val => {
      let addBasketItem: IAddBasketItem = { id: val.id, quantity: val.quantity,categoryName:val.categoryName,description:val.description,name:val.name,newPrice:val.newPrice,oldPrice:val.oldPrice,photosUrl:val.photosUrl };
      return addBasketItem;
    })
    return addBasketItems;
  }
  removeItemFromBasket(itemId: number) {
    if (this._basket.value.basket.length == 1) {
      this._basket.next({ basket: [] });
      //      this.test.basket=[];

      const basketId = localStorage.getItem('basketId');
      this.http.delete(environment.baseUrl + "BasketRedis?basketId=" + basketId).subscribe(
        {
          error: (error: IError) => {
            this.error = error.error.message;
          }
        }
      );
      localStorage.removeItem('basketId');
      return;
    }
    let returnedBasketItems: IReturnedBasketItem[] = this._basket.value.basket.filter(Item => Item.id != itemId) // ده في حالة ال ريموف 

    let addBasketItems = this.convertReturnedBasketToAddBasket(returnedBasketItems);

    let basket: IAddBasket = { basket: addBasketItems }

    const basketId = localStorage.getItem('basketId');
    let queryParams = new HttpParams();
    queryParams = queryParams.set('basketId', basketId!);

    ////////////
    this.http.post<IReturnedBasket>(

      environment.baseUrl + "BasketRedis", basket, { params: queryParams }
    ).pipe(
      map(
        val => {
          val.basket.forEach(x => x.MainPhotoIndex = 0)
          return val
        }
      )
    ).subscribe(
      {
        next: (val) => {
          ;
          this._basket.next(val);  // لازم اعملها اوبسيرفابول بقي علشان ال allPrice اللي ف ال cart-items.ts يتبعتلها اخر تحديث 
          // this.test.basket=val.basket; // حطها  فوق عن طريق ال نيكست افضل علشان لو حاجة عاملة سبسكرايب علي ال اوبسيرفابول تروحلها 

        },
        error: (error: IError) => {
          this.error = error.error.message;
        }
      }

    );
  }
  changeItemQuantity(itemId: number, quantity: number) {
    // اجيب كل العناصر من ال اوبسيرفابول وبعدين اغير ف الكوانتتي بتاع العنصر وبعدين اضيفه ف الباك
    let items = this._basket.value.basket;
    let index = items.findIndex(i => i.id == itemId);
    items[index].quantity = quantity;

    const basketId = localStorage.getItem('basketId'); //sure that exist in local 

    let addBasketItems = this.convertReturnedBasketToAddBasket(items);



    let queryParams = new HttpParams();
    queryParams = queryParams.set('basketId', basketId!);

    let AddBasket: IAddBasket = { basket: addBasketItems }

    this.http.post<IReturnedBasket>(

      environment.baseUrl + "BasketRedis", AddBasket, { params: queryParams }
    ).pipe(
      map(
        val => {
          val.basket.forEach(x => x.MainPhotoIndex = 0)
          return val
        }
      )
    ).subscribe(
      {
        next: (val) => {
          this._basket.next(val);  // لازم اعملها اوبسيرفابول بقي علشان ال allPrice اللي ف ال cart-items.ts يتبعتلها اخر تحديث 
          // this.test.basket=val.basket; // حطها  فوق عن طريق ال نيكست افضل علشان لو حاجة عاملة سبسكرايب علي ال اوبسيرفابول تروحلها 
        },
        error: (error: IError) => {
          this.error = error.error.message;
        }
      }

    );

  }
  addItemToBasket(itemId: number, quantity: number, name: string,
    description: string,
    oldPrice: number,
    newPrice: number,
    photosUrl: string[],
    categoryName: string) {
    let addBasketItem: IAddBasketItem = { id: itemId, quantity: quantity ,categoryName:categoryName,description:description,name:name,newPrice:newPrice,oldPrice:oldPrice,photosUrl:photosUrl};
    if (this._basket.value.basket.length == 0) {
      //Empty Basket That Mean In My Logic That Dont Have Any BasketId In Local Or Redis
      const basketId = Guid.create().toString();; //Make It Guid Later
      localStorage.setItem('basketId', basketId);
      let queryParams = new HttpParams();
      queryParams = queryParams.set('basketId', basketId);

      let addBasketItems: IAddBasketItem[] = [addBasketItem]

      let addBasket: IAddBasket = { basket: addBasketItems };
      this.http.post<IReturnedBasket>(

        environment.baseUrl + "BasketRedis", addBasket, { params: queryParams }
      ).pipe(
        map(
          val => {
            val.basket.forEach(x => x.MainPhotoIndex = 0)
            return val
          }
        )
      ).subscribe(
        {
          next: (val) => {
            this._basket.next(val);  // لازم اعملها اوبسيرفابول بقي علشان ال allPrice اللي ف ال cart-items.ts يتبعتلها اخر تحديث 
            // this.test.basket=val.basket; // حطها  فوق عن طريق ال نيكست افضل علشان لو حاجة عاملة سبسكرايب علي ال اوبسيرفابول تروحلها 
          },
          error: (error: IError) => {
            this.error = error.error.message;
          }
        }

      );

    } else {
      //Have Basket
      // convert returnedBasket To Add Basket
      let listReturnedBasket = this._basket.value.basket;
      let listAddBasket = this.convertReturnedBasketToAddBasket(listReturnedBasket);


      const basketId = localStorage.getItem('basketId'); //sure that exist in local 


      // لو ال باسكت ايتم اللي هضيفه موجود ف ال باسكيت ايتيمز يبقي هاخد الكاونت بتاعت الباسكت ايتم اللي هضيفه وازودها علي الباسكيت ايتم اللي موجود ف ال باسكيت ايتيمز

      let index = listAddBasket.findIndex(item => item.id == itemId);
      if (index == -1) {
        listAddBasket.push(addBasketItem);
      } else {

        // listAddBasket[index].quantity += quantity;
        listAddBasket[index].quantity = quantity; // هخليها بتساوي ال كونتتي افضل علشان لو ضغط اكتر من مرة ب الغلط كده افضل 

      }

      let queryParams = new HttpParams();
      queryParams = queryParams.set('basketId', basketId!);
      let addBasket: IAddBasket = { basket: listAddBasket };
      this.http.post<IReturnedBasket>(

        environment.baseUrl + "BasketRedis", addBasket, { params: queryParams }
      ).pipe(
        map(
          val => {
            val.basket.forEach(x => x.MainPhotoIndex = 0)
            return val
          }
        )
      ).subscribe(
        {
          next: (val) => {
            this._basket.next(val);  // لازم اعملها اوبسيرفابول بقي علشان ال allPrice اللي ف ال cart-items.ts يتبعتلها اخر تحديث 
            // this.test.basket=val.basket; // حطها  فوق عن طريق ال نيكست افضل علشان لو حاجة عاملة سبسكرايب علي ال اوبسيرفابول تروحلها 
          },
          error: (err: IError) => {
            this.error = err.error.message;
          }
        }

      );

    }


  }
  
  totalBasketPrice(): Observable<number>
  {
   return   this.basket$.pipe(
        filter(
          Basket => Basket.basket.length > 0
        ),
        map(Basket => {
          let totalSum = Basket.basket.map(
            BasketItem => BasketItem.newPrice * BasketItem.quantity
          ).reduce((sum, current) => sum + current, 0);
          return +totalSum.toFixed(2);
        }
        ))
      
  }


}
