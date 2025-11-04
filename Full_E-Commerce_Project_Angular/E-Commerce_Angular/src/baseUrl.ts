export const environment= {
  production: false,
  baseUrl: 'https://localhost:7260/api/',
  stripePublicKey:'pk_test_51RZSpvQZUUK4xO8RThwZU38ywAKVXS0oWfMW4XrUp9MNlU5jEG9RSlHTFMlZ1y8nkEFxgNocBDweMc4sMLIOyAiY00X7PYryd9'

};
/*

C# → string Reference Type (بس immutable).

TypeScript → string Primitive Type (Value Type) (برضه immutable). =>طبيعي لانها فاليو تايب وبتتخزن ف الاستاك 

let a = "hello";
let b = a;
a = "world";

console.log(b); // hello

//Gold

string a = "hello";
string b = a;
a = "world";

Console.WriteLine(b); // hello
--- 
م ده طبيعي 
وبردو 
var P =new Product  ();
var c =P;
var P=new Category();
Console.WriteLine(typeof(c)) // Product;

*/