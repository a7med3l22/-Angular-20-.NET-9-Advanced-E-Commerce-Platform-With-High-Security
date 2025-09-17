import { HttpClient } from '@angular/common/http';
import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BehaviorSubject, filter, map, Observable, Subscription } from 'rxjs';
import { IProduct } from '../../shared/Models/Products';
import { ShopService } from '../../../shop/shop-service';
import { Location } from '@angular/common';
import { PageChangedEvent } from 'ngx-bootstrap/pagination';
import { environment } from '../../../baseUrl';

@Component({
  selector: 'app-product-details',
  standalone: false,
  templateUrl: './product-details.html',
  styleUrl: './product-details.scss'
})
export class ProductDetails implements OnInit, OnDestroy {
  /////////عاوز لما اضغط ع الصفحة التانية يجيبلي اول عنصر ف الصفحة التانية ولما اضغط ع الاولي .....
  /**
   *
   */
  // عاوز اعمل ال اي دي اوبسيرافول بحيث كل م يتغير اعرف وعاوز القيمة الابتدائية بتاعته ب صفر   
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
  constructor(private router: Router, private location: Location, public filteredProducts: ShopService, private route: ActivatedRoute, private http: HttpClient) {
    this._id = new BehaviorSubject<number>(+this.route.snapshot.paramMap.get('id')!);
    this.id = this._id.asObservable();
  }
  ngOnDestroy(): void {
    //[Gold][Gold][Gold][Gold][Gold][Gold][Gold][Gold][Gold][Gold][Gold][Gold][Gold][Gold][Gold][Gold][Gold][Gold]
    ///////// الدوخه اللي انا كنت فيها دي كلها اني مكنتش عامل ان سبسكرايب لما ال كمبوننت يتعمله ديستوري !!
    // ف اللي كان بيحصل هو ان لما الكمبوننت ب امبليمنت اون ديستوري  مش بيحرر الكمبوننت من الذاكرة لاني معملتش ان سبسكرايب ف بالتالي بيفضل موجود ف الذاكرة الكمبوننت ده 
    //  ف علشان كده لما قيمة جديده بتتحط بيبش القمة الجديدة دي ف كل الكمبوننتس اللي عاملين عليه سبسكرايب !!
    // ف مصيبه سودا اني معملش ان سبسكرايب !!
    // ;
    this.subscription.unsubscribe();
  }
  ngOnInit(): void {
    ;;;;;;;;;;;
    console.log(this.filteredProducts.showIcons);
    // كده ضمنت ان ال idsو product موجوده ومتحدثه مع اي تغييير 
    // ;
      this.filteredProducts.showIcons =false;

this.filteredProducts.allowToGetAllProduct=false;
//مش عاوزها تظهر الازرار لما اعمل ريلود بس 
;
    this.subscription.add(
      this.filteredProducts.getAllProducts.pipe(
        // هو اول م يعرف ان getAllProducts معمولها سبسكرايب هيروحلها حتي لو قعدت اتنقل بين الكمبونات المختلفة 
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
      this.filteredProducts.showIcons = true;


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




this.filteredProducts.allowToGetAllProduct=true;





  }
  next() {
    // هخليه يرجعلي ليسته من ال اي ديز بتاع البرودكتات اللي راجعه  
    if (this.min) {
      this.min = false
    }
    let idIndex = this.ids!.findIndex(id => id == this._id.value);

    if (idIndex + 2 == this.ids?.length) {
      this.max = true;
    }
    if (idIndex + 1 == this.ids?.length) {
      // اخرج وكده وصلت للماكس

      return;
    }

    let nextId = this.ids?.at(idIndex + 1)
    this.location.go('shop/productDetails/' + nextId) // علشان يغيره فوق بس من غير م يحصل اي حاجة تاني

    this._id.next(nextId!)







  }
  previous() {
    if (this.max) {
      this.max = false
    }
    // ;
    let idIndex = this.ids!.findIndex(id => id == this._id.value);
    if (idIndex == 1) {
      this.min = true;
    }
    if (idIndex == 0) {
      // اخرج وكده وصلت للميني
      return;
    }
    let previousId = this.ids?.at(idIndex - 1)
    this.location.go('shop/productDetails/' + previousId) // علشان يغيره فوق بس من غير م يحصل اي حاجة تاني



    this._id.next(previousId!)











  }
  back() {
    debugger;
    this.router.navigateByUrl("/shop");

  }

  checkTest()
  {
    ;
    
    console.log(this.filteredProducts._test.value.id);//Full Except 5 
    this.filteredProducts.__test.id=66;
    console.log(this.filteredProducts._test.value.id);//Full Except 66
    //Gold Except
  }
  //   pageChanged(event:PageChangedEvent)
  // {
  //   ;

  //      this.filteredProducts.GetAllProducts({PageNumber:event.page,Search:this.filteredProducts.shopParams?.Search,SortBy:this.filteredProducts.shopParams?.SortBy,CategoryId:this.filteredProducts.shopParams?.CategoryId});

  //       this._id.next(0);


  //     }
}
