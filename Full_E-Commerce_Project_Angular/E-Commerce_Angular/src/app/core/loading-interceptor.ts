import { HttpInterceptorFn, HttpResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';
import { finalize, tap } from 'rxjs';

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  
  //INJECT  NgxSpinnerService //  constructor(private spinner: NgxSpinnerService) {


  req=req.clone({withCredentials: true}); // علشان يبعت الكوكي
  //gold
   // كده غيرت الرفرنس بتاعه وخليته يشاور ع رفرنس جديد فيه الخاصية دي اما لو كنت عملت كده فقط req.clone({withCredentials: true}) مش كده  req=req.clone({withCredentials: true})  كده ملهاش اي لازمة وال كلون اللي عملته ممسكوتهوش ف فريابول ف بكده الرفرنس الجديد اللي اتعمل ملهوش اي لازمة لاني هيتمسح م الميموري بعد السطر ده لاني محفظتهوش ف فريابول يعيش طول الفترة اللي الفريابول فيها ف البلوك بتاعه !!
    let spinner = inject(NgxSpinnerService);

    spinner.show();

  //start
 
  return next(req).pipe(
//End
//export function tap<T>(observerOrNext?: Partial<TapObserver<T>> | ((value: T) => void)): 
// MonoTypeOperatorFunction<T>;

  //   tap(
  //       {
          
  // // next?: ((value: T) => void) | null,
  // // error?: ((error: any) => void) | null,
  // // complete?: (() => void) | null
  // next:_=>{spinner.hide();},
  // error:_=>{spinner.hide();},
  // complete:()=>{spinner.hide();}
          
  //       }),

  //   )
  //export function finalize<T>(callback: () => void): MonoTypeOperatorFunction<T> {
  finalize(
    ()=>{
spinner.hide();
    }
  )
  )

}


// constructor(private spinner: NgxSpinnerService) {
    
//   }
// ngOnInit() {
//     /** spinner starts on init */
//     this.spinner.show();

//     setTimeout(() => {
//       /** spinner ends after 5 seconds */
//       this.spinner.hide();
//     }, 5000);
//   }























// //type HttpInterceptorFn = (req: HttpRequest<unknown>, next: HttpHandlerFn) => Observable<HttpEvent<unknown>>;
// export const test: HttpInterceptorFn =(x,y)=>
// {
 
 
//   //type HttpHandlerFn = (req: HttpRequest<unknown>) => Observable<HttpEvent<unknown>>;
//   return y(x);

// }
// type xx=()=>number;

// class x 
// {
//   xxxx:xx=()=>3;
//   x:number=this.xxxx();
// }