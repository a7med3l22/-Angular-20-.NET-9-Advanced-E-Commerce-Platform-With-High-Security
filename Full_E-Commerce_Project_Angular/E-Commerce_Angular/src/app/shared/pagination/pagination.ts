import { Component, Input, input, OnDestroy, OnInit } from '@angular/core';
import { ShopService } from '../../../shop/shop-service';
import { PageChangedEvent } from 'ngx-bootstrap/pagination';
import { BehaviorSubject, Observable, Subscription } from 'rxjs';
import { faL } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-pagination',
  standalone: false,
  templateUrl: './pagination.html',
  styleUrl: './pagination.scss'
})
export class Pagination implements OnInit,OnDestroy {

  // الملحوظه الوحيده اني لما بعمل ديتيلز بيجيبلي اول برودكت 
  /**
   *
   */
  @Input() _id!: BehaviorSubject<number>;
  pageSize!: number;
  pageNumber:BehaviorSubject<number>;
  // _pageNumber=new BehaviorSubject<number>(1);  // حلو انا عاوزة يجيب اخر قيمة محفوظه ف السيرفس انما مش عاوز اعمل سبسكرايب عليها ف كل مرة تتغير لا انا عاوز اخد القيمة مرة واحده ك ديفولت بس علشان كده هساويها ب الفاليو ع طول بعتها تحت ف ال اون اينيت 
  // pageNumber=this._pageNumber.asObservable(); 
  totalReturnedProducts!: number;
  subscribe=new Subscription;
  constructor(private shopService: ShopService) {
    this.pageNumber=
    shopService._pageNumber; // كده انا خليته يبص ع نفس الرفرنس اللي ف السيرفس -- 
    
    // المشكلة ان لما الفاليو تتغير مش بيحس ب التغيير الفوري لانها فاليو تايب ف مش بيعرف اخر قيمة غير لو اتساوي بيها وعدي عليها ف الحل اني اخليه يبص علي رفرنس تايب بحيث اني لو غيرت الرفرنس ده يبقي عارف لاتغيير لانه هيكون بيبص عليه ف نفذ
  // عو كان بيبص ع رفرنس معين بس انا عملت بيكست ل رفرنس تاني مغيرتش ف نفس الرفرنس الاصلي ف لا لازم اخليه يبص ع رفرنس واحد واغير ف نفس الرفرنس ده ف نفذ وخليه يساوي رفرنس واحد والتغيير يبقي ف نفس الرفرنس ده  
  // طبعا حاجة مهمة جدا لو عندي حاجة رفرنس تايب وحاجة تانيه رفرنس تايب والاتنين بيبصوا ع نفس الرفرنس ف اي تغيير ف الرفرنس ده التاني هيحس بيه لانهم بيبصوا ع رفرنس واحد والتغيير حصل ف الرفرنس الواحد ده .. بس هما الاتنين لازم يبقوا رفرنس تايب اكيد ف نفذ واكيد طالما مخلتش الرفرنس تايب ده يساوي رفرنس تايب تاني يبقي انا كده تمام ف لازم اغير  فنفس الرفرنس ولازم يبصوا ع نفس الرفرنس مش اخليهم يبصوا ع نفس الرفرنس وبعد كده اخلي واحد منهم يبص ع رفرنس جديد اكيد لا  
  }
  ngOnDestroy(): void {
    // اللي لاحظته دلوقتي من سلولكها اني لما بروح من باجينيشن ل باجينيشن ف كمبوننت تاني باخد ال ديفولت فاليو من جديد تقريبا علشان انا عملت ان سبسكرايب ف مسح الكمبونتس ف لما بدخله تاني بيعيد انشاءه م الاول 
    // ف انا مش عاوزه يعيد انشاءه م الاول ويحتفظ ب القيم ف ب التالي مش هعمل ان سبسكرايب لل بيدج نامبر بس ده خطر اكيد فيه حل اني اخلي القيمة الابتدائية مش بواحد اخليها ب اخر قيمة ف الكمبوننت اللي قبله 
    // ف اسلم حل اني اعمل سيرفس لل بيدج نامبر وابعتلها قيمة مع كل تغيير وسبسكرايب عليها هنا  
    this.subscribe.unsubscribe();
  }
  ngOnInit(): void {
    
    // this._pageNumber.next(this.shopService._pageNumber.value); // وبكده تم المطلوب 
    //  this.shopService.GetAllProducts({ PageNumber:  this._pageNumber.value, Search: this.shopService.shopParams?.Search, SortBy: this.shopService.shopParams?.SortBy, CategoryId: this.shopService.shopParams?.CategoryId });
// لازم طبعا اجيب ال برودكت ب ال بيدج نمبر اللي انا عملتلها نيكست دي
    // ;
    this.subscribe.add(

      this.shopService.getAllProducts.subscribe(
        val => {
          this.pageSize = val?.pageSize!;
          // this._pageNumber.next(val?.pageNumber!);
          this.totalReturnedProducts = val?.totalReturnedProducts!;
        }
      )
    );

    // this.subscribe.add(
    // this.pageNumber.subscribe(
    //   val => {
    //     //  if (this._id) // علشان مش هيتبعتلها قيمة الا من خلال ال اب اللي هو برودكت ديتيلز بس 
    //     // {
    //     //   ;
    //     //   // this._id.next(0);
    //     // }
    //     // ;
    //     this.shopService.GetAllProducts({ PageNumber: val, Search: this.shopService.shopParams?.Search, SortBy: this.shopService.shopParams?.SortBy, CategoryId: this.shopService.shopParams?.CategoryId });

    
    //   }

    // ));
  }

  //   pageChanged(pageChangedEvent:PageChangedEvent)
  //   {
  //         this.pageNumber=pageChangedEvent.page;
  //          this.shopService.GetAllProducts({PageNumber:this.pageNumber,Search:this.shopService.shopParams?.Search,SortBy:this.shopService.shopParams?.SortBy,CategoryId:this.shopService.shopParams?.CategoryId});
  // // ;
  // if(this._id) // علشان مش هيتبعتلها قيمة الا من خلال ال اب اللي هو برودكت ديتيلز بس 
  // {

  //   this._id.next(0);
  // }
  //  }
pageChanged(pageNumber:number)
{
  
  debugger;
  // this.shopService.pageNumberSameRef.pageNumber=pageNumber;
  this.shopService._pageNumber.next(pageNumber);
  // ;
  // this._pageNumber.next(pageNumber);

  if(this.shopService.pageIsChanged)
  {
  this.shopService.GetAllProducts({ PageNumber: pageNumber, Search: this.shopService.shopParams?.Search, SortBy: this.shopService.shopParams?.SortBy, CategoryId: this.shopService.shopParams?.CategoryId });
  }
    this.shopService.pageIsChanged=true;
  
  if(this._id) // علشان مش هيتبعتلها قيمة الا من خلال ال اب اللي هو برودكت ديتيلز بس 
  {

    this._id.next(0);
  }
}

}
