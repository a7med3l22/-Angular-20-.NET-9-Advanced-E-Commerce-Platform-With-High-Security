export interface IGetAllProducts {
  allProductsCount: number
  totalReturnedProducts: number
  returnedPaginationProducts: number
  products: IProduct[]
  pageNumber:number;
  pageSize:number;
}
export interface IGetAllCatrgories {
  id:number
  name: string
  description: string
}
export interface IProduct {
  id: number
  name: string
  description: string
  oldPrice: number
  newPrice: number
  photosUrl: string[]
  categoryName: string
  categoryId: number
  MainPhotoIndex:number
}
export interface IMyException
{
   code:number;
   message:string;
}
export interface ISpecProducts
{
SortBy?:string;
CategoryId?:number;
Search?:string;
PageNumber?:number;
PageSize?:number;
}
export interface IPageNumber {

  pageNumber: number
}