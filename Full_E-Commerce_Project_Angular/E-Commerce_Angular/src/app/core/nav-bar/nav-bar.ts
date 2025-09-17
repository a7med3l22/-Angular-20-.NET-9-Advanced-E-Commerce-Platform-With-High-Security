import { Component, OnDestroy, OnInit } from '@angular/core';
import { BasketService } from '../../basket/basket-service';
import { filter, map, Observable, Subscription } from 'rxjs';
import { IsAuth } from '../../../account/is-auth';


@Component({
  selector: 'app-nav-bar',
  standalone: false,
  templateUrl: './nav-bar.html',
  styleUrl: './nav-bar.scss'
})

export class NavBar implements OnInit, OnDestroy {
  /**
   *
   */
  subscribe = new Subscription;
  basketItems: number | null = null;
  isAuthorize!:boolean;
  constructor(private basket: BasketService,private isAuth:IsAuth) {

  }
  ngOnDestroy(): void {
    this.subscribe.unsubscribe()
  }
  ngOnInit(): void {
    this.isAuth.getAuthFromBack()

 this.subscribe.add(
this.isAuth.isAuth.subscribe(
  val=>this.isAuthorize=val
));
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

    // this.test = this.basket.test; // مبقاش ليه لازمة ووضحت ف ال #test in basket-service.ts
   
    if(this.basket.callMe)
    {
      // روح جيب العناصر من ال باك 
      this.basket.getBasketIdFromLocal(); // الحمدلله
    }

      this.subscribe.add(
     this.basket.basket$.pipe(
      map(val=>
      {
        if(val.basket.length>0)
        {
          return val.basket.length;
        }
        return null;
      }
      )
     ).subscribe(
      val=>this.basketItems=val
     )
    );
    this.basket.callMe=false;
  }

}
