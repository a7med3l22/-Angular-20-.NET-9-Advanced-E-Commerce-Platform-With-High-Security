export interface IDeliveryMethod {
  id: number;
  name: string;
  price: number;
  deliveryTimeInDays: number;
}
export interface IOrder{
//parameters
  deliveryMethodId?:number; //
  basketId?:string|null; 
}
export interface IOrderBody
{
// usersDeleviredAddresse?:OrderBody;
  street?: string, //
  city?: string, //
  state?: string, //
  zipCode?: string, //
  country?: string //
}
interface OrderBody
{
//body

  // firstName?: string, //
  // lastName?: string,  //
  // street?: string, //
  // city?: string, //
  // state?: string, //
  // zipCode?: string, //
  // country?: string //
}

export interface IDeliveryMethod
{
  name:string;
  price:number;
  deliveryTimeInDays:number;
  id:number;
}
export interface IPayment
{
  clientSecret:string;
}
export interface Order {
  orderId: number;
  orderItems: OrderItem[];
  deliveryAddress: DeliveryAddress;
  dateTime: string; // أو Date لو بتحوّلها بعدين
  paymentStatus: string;
  subTotal: number;
  total: number;
  deliveryMethod: DeliveryMethod;
}

export interface OrderItem {
  price: number;
  productId: number;
  productName: string;
  quantity: number;
  id: number;
}

export interface DeliveryAddress {
  firstName: string;
  lastName: string;
  street: string;
  city: string;
  state: string;
  zipCode: string;
  country: string;
}

export interface DeliveryMethod {
  name: string;
  price: number;
  deliveryTimeInDays: number;
  id: number;
}
