import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Pagination } from './pagination/pagination';
import { PaginationComponent } from "ngx-bootstrap/pagination";
import { FormsModule } from '@angular/forms';



@NgModule({
  declarations: [
    Pagination
  ],
  imports: [
    CommonModule,
    PaginationComponent,
    FormsModule
],
  exports:[Pagination]
})
export class SharedModule { }
