export interface IReturnedBasket{
basket:IReturnedBasketItem[]
}
export interface IReturnedBasketItem{
id:number;
name:string;
description:string;
oldPrice:number;
newPrice:number;
photosUrl:string[];
categoryName:string;
quantity:number;
MainPhotoIndex?:number;
}

////
export interface IAddBasket{
basket:IAddBasketItem[]
}
export interface IAddBasketItem{
id:number;
quantity:number;
name:string;          
description:string;                               
oldPrice:number;                                 
newPrice:number;               
photosUrl:string[];                 
categoryName:string;                 
}
///
export interface IError
{
    error:IErrorMessage;
}
interface IErrorMessage
{
    message:string,
    messages:string[];
}
export interface ISuccess
{
    message:string;
}
///
