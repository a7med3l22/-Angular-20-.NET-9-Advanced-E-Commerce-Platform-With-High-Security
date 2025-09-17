import { Component, ElementRef, OnDestroy, OnInit, Type, ViewChild, viewChild } from '@angular/core';
import { ShopService } from './shop-service';
import { IGetAllCatrgories, IGetAllProducts, IProduct } from '../app/shared/Models/Products';
import { BehaviorSubject, Subscription } from 'rxjs';
import { faL } from '@fortawesome/free-solid-svg-icons';
import { FormControl } from '@angular/forms';
import { PageChangedEvent } from 'ngx-bootstrap/pagination';

@Component({
  selector: 'app-shop',
  standalone: false,
  templateUrl: './shop.html',
  styleUrl: './shop.scss'
})
export class Shop implements OnInit,OnDestroy{
  //[Gold]
  // x:Type<IGetAllCatrgories>=Shop;
  /*
    x:Type<IGetAllCatrgories>
    يعني ال x بتاخد كلاس بحيث ان الكلاس ده لازم يكون فيه كل ال خصائص اللي موجوده ف ال IGetAllCatrgories
    وممكن اعمل ع كلاس ال Shop implements IGetAllCatrgories 
    علشان يساعدني انه اكون متاكد ان كل الخصائص موجوده 

    ولو عملت 
    y:Type<any>
    معناها انه بياخد كلاس بحيث ان الكلاس دي مش متحدد ان يكون فيه بروبيرتس معينه وممكن تاخد كلاس مفهوش بروبيرتس خالص كمان عادي 
  */
  subscription=new Subscription;
  getAllProducts!:IGetAllProducts|null;
  getAllCatrgories!:IGetAllCatrgories[]|null;
  // sortOption: string|null ='name_asc';


  ////////////
  // عاوز القيم الابتدائية بتاعتخم  ب اللي موجوده ف ال سيرفس
   _sortOption=new BehaviorSubject<string>('name_asc');
  SortOption=this._sortOption.asObservable();

     _search=new BehaviorSubject<string>('');
    Search=this._search.asObservable();

  categoryId?:number;

///////////


  constructor(private shopService:ShopService) {
    this._sortOption.next(this.shopService._sortOptionService.value); // بحيث ان اول م يتم انشاء الكونستراكتور ياخد اخر قيمة ف ال سيرفس 
    this._search.next(this.shopService._searchService.value);
     this.categoryId=this.shopService._categoryIdService.value;
;
  }
  // id: number=4;
  // name: string="";
  // description: string="";

// لو فيه قيم ف ال شوب شيرفس يبقي مش عاوز اعمل حاجة ف اخليهم ب ترو لانها مش هتبقي اول مرة دخول ليها 


  ngOnDestroy(): void {
  this.subscription.unsubscribe();
  }



  ngOnInit(): void {
    debugger;
// search,sorting,catId 

//NotSearch,NotSort,NotGet => هفلتر بيهم بحيث انه لو ترو ميعملش سبسكرايب 
  
if(this.shopService._getAllCategories.value==null)
{
  this.shopService.GetAllCatrgories();
}



    this.subscription.add(
    this.Search.subscribe(

     val=>
      {
       
  this.shopService._searchService.next(val);
      //PageNumber:this.shopService._pageNumber.value // كل م ييجي هنا هنساوي ال بيدج نامبر ب اخر قيمة موجوده ف ال بيدج نامبر اللي ف ال سيرفس ولكن القيمة هتتغير بس لما ييجي هنا مش مع كل تغيير لا كل م ييجي هنا بس 
          this.shopService.shopParams={PageNumber:this.shopService._pageNumber.value,Search:this._search.value,SortBy:this._sortOption.value,CategoryId:this.categoryId};
          
          this.shopService.GetAllProducts(this.shopService.shopParams)
        
        
    
      }
       
        
    ));
    ;
this.shopService.allowToGetAllProduct=false;
      this.subscription.add(
    this.SortOption.subscribe(
  val=>
       
  {
  this.shopService._sortOptionService.next(val);

          this.shopService.shopParams={PageNumber:this.shopService._pageNumber.value,Search:this._search.value,SortBy:this._sortOption.value,CategoryId:this.categoryId};

          this.shopService.GetAllProducts(this.shopService.shopParams)
      }       
    ));
    
    this.subscription.add(
  this.shopService.getAllProducts.subscribe(
    
      val=>{
        this.getAllProducts=val

      }
    )
    );
   this.subscription.add(

     this.shopService.getAllCategories.subscribe(
      val=>
        {this.getAllCatrgories=val


        }
      )
    );
    this.shopService.allowToGetAllProduct=true;

  }









GetPrdByCat(catId?:number)
{
    this.shopService.pageIsChanged=false;
;
  this.shopService._categoryIdService.next(catId);
  this.categoryId=catId;
  {
          this.shopService.shopParams={PageNumber:this.shopService._pageNumber.value,Search:this._search.value,SortBy:this._sortOption.value,CategoryId:this.categoryId};

          this.shopService.GetAllProducts(this.shopService.shopParams)
      }    //  console.log(this.categoryId);
// عاوز لما ال كاتيجوري يتغير ميروحش يجيب ال برودكتش من ال بيدج اتجينش
    }


    search(search:string)
    {
  this.shopService.pageIsChanged=false;

this._search.next(search);
    }
ResetFilters()
{
 // عاوز امسح كل حاجة من ال UI واغير كل القيم اللي موجوده من غير م يروح لل API
 // عاوز اغير القيم بتاع ال اوبسيرفابول من غير م اروح لل api
//  console.log(this.categoryId);
//  console.log(this._search.value);
//  console.log(this._sortOption.value);
//  console.log("------after reset---");
 
 this.shopService.getApi=false; // علشان ميروحش لل api 
 this._search.next('');
 this._sortOption.next('name_asc');
 this.categoryId=undefined
 this.shopService.getApi=true;
  this.shopService.pageIsChanged=false;
 this.shopService.GetAllProducts(); 
 ;
//  this.shopService.pageNumberSameRef={pageNumber:1};
//  this.shopService._pageNumber.next(this.shopService.pageNumberSameRef);
this.shopService._pageNumber.next(1); // كده غيرت ف نفس الرفرنس وكده متأكد اني لما اعمل ريسيت هيرجع للصفحة واحد -- برافو عليك حصل اللي المتوقع منك فعلا اللي فاهم بيريح فعلا وبيستريح
this.shopService.shopParams={PageNumber:1,Search:this._search.value,SortBy:this._sortOption.value,CategoryId:this.categoryId};


  this.shopService._searchService.next('');
  this.shopService._categoryIdService.next(undefined);
  this.shopService._sortOptionService.next('name_asc'); // وبكده كل القيم بقيت متحدثه

//  console.log(this.categoryId);
//  console.log(this._search.value);
//  console.log(this._sortOption.value);
 // كده ناقص بقي ال فورم كونترول 
}
// pageChanged(event:PageChangedEvent)
// {
//   ;
//   {

//         this.shopService.shopParams={PageNumber:event.page,Search:this._search.value,SortBy:this._sortOption.value,CategoryId:this.categoryId};
//         this.shopService.GetAllProducts(this.shopService.shopParams)
//       }}
}
// رقم الصفحة-1)*حجم الصفحة)+1 )   ) ) )
//الي
// رقم الصفحة-1)*حجم الصفحة)+عدد عناصر الصفحة )  ) ) ) 
