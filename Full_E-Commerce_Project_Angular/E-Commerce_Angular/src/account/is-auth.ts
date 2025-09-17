import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { faL } from '@fortawesome/free-solid-svg-icons';
import { BehaviorSubject } from 'rxjs';
import { ISuccess } from '../app/shared/Models/basket';
import { environment } from '../baseUrl';

@Injectable({
  providedIn: 'root'
})
export class IsAuth {

_isAuth=new BehaviorSubject(false);
isAuth=this._isAuth.asObservable();

  constructor(private http:HttpClient) {
  
  }

  getAuthFromBack()
  {
  this.http.get<boolean>(environment.baseUrl+"Account/isAuthorized").subscribe(
      val=>
      {
        debugger;
        this._isAuth.next(val)

      }
    )
  }
}
