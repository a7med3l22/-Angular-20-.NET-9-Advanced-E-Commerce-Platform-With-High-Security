import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IGetAllCatrgories, IGetAllProducts, IPageNumber, ISpecProducts } from '../app/shared/Models/Products';
import { BehaviorSubject, filter, map } from 'rxjs';
import Swal from 'sweetalert2'
import { environment } from '../baseUrl';

@Injectable({
  providedIn: 'root'
})
export class ShopService {
  allowToGetAllProduct = true;
  showIcons = true;
  //  pageIsChanged=true;
 // عاوز لما اجي ابحث او اغير ف الكاتيجوري ميروحش يجيب كل ال برودكتس من ال onChange بتاع ال الارقام
  __test: { id: number } = { id: 5 };
  _test = new BehaviorSubject<{ id: number }>(this.__test); // كده ال تيست قيميتها رفرنس تايب ف بتشاور علي __test ف لو غيرت ف الرفرنس تايب __test اللي هي بتشاور عليه هتحس ب التغيير طبعا 

pageIsChanged=true;

  _sortOptionService = new BehaviorSubject<string>('name_asc');
  SortOptionService = this._sortOptionService.asObservable();

  _searchService = new BehaviorSubject<string>('');
  SearchService = this._searchService.asObservable();

  _categoryIdService = new BehaviorSubject<number | undefined>(undefined);
  categoryIdService = this._categoryIdService.asObservable();


  // pageNumberSameRef:IPageNumber={pageNumber:1} // خليته يبص ع ده واي تغيير هيبقي ف ده علشان يحس بيه لاني غيرت ف نفس الرفرنس اللي هو بيبص عليه

  _pageNumber = new BehaviorSubject<number>(1); // عاوز اعمل نيكست ل نفس الرفرنس واغير ف نفس الرفرنس 
  pageNumber = this._pageNumber.asObservable();


  shopParams: ISpecProducts | null = null;
  getApi = true;
  _getAllProducts = new BehaviorSubject<IGetAllProducts | null>(null);
  getAllProducts = this._getAllProducts.asObservable();

  private _errorMessage = new BehaviorSubject<string | null>(null);
  errorMessage = this._errorMessage.asObservable();

  _getAllCategories = new BehaviorSubject<IGetAllCatrgories[] | null>(null);
  getAllCategories = this._getAllCategories.asObservable();

 

  constructor(private http: HttpClient) {
  }

  GetAllProducts(specProducts: ISpecProducts | null = null) {
    if (!this.allowToGetAllProduct) {
      ;
      return; // جميل جدا علشان ميروحش لل باك لو الشرط ده اتحقق
    }


    ;
    if (this.getApi == false) {
      return;

    }
    let queryParams = new HttpParams
    if (specProducts) {
      //This class is immutable; all mutation operations return a new instance.
      if (specProducts.SortBy) {
        queryParams = queryParams.append("SortBy", specProducts.SortBy);
      }
      if (specProducts.CategoryId) {
        queryParams = queryParams.append("CategoryId", specProducts.CategoryId);
      }
      if (specProducts.PageNumber) {
        queryParams = queryParams.append("PageNumber", specProducts.PageNumber);
      }
      if (specProducts.PageSize) {
        queryParams = queryParams.append("PageSize", specProducts.PageSize);
      }
      if (specProducts.Search) {
        queryParams = queryParams.append("Search", specProducts.Search);
      }
    }

    this.http.get<IGetAllProducts>(environment.baseUrl+ "Product/AllProducts", { params: queryParams })
      .pipe(
        map(

          val => {
            val.products.forEach(prd => prd.MainPhotoIndex = 0)
            return val;
            // وبكده خليتها ب تساوي ال صفر عملت كده علشان هييجي ب ان ديفايند لانه مش مستقبله من الباك وانا اصلا حطيت الخاصيه دي علشان يبقي لكل برودكت فيه مين فوتو اندكس وكل مين فوتو يبقي مستقل يعني بيشاور ع رفرنس غير التاني 
          }

        )
      ).

      subscribe(

        {
          next: (val) => {
            ;//////////Yes
            this._getAllProducts.next(val);

            // Swal.fire({
            //   icon: "success",
            //   title: "Getting Products Successfully",
            //   showConfirmButton: false,
            //   timer: 1500
            // });

          },
          error: (err) => {
            if (err.error.message) {
              this._errorMessage.next(err.error.message);
            }
          }
        }

      )
  }
  GetAllCatrgories() {
    this.http.get<IGetAllCatrgories[]>(environment.baseUrl+ "Category").subscribe(

      {
        next: (val) => {

          this._getAllCategories.next(val);
        },
        error: (err) => {
          if (err.error.message) {
            this._errorMessage.next(err.error.message);
          }
        }
      })
  }
  // getPrpductById(id:number){
  //   ;
  //   this.http.get(environment.baseUrl+"api/Product/"+id)

  // }
}
