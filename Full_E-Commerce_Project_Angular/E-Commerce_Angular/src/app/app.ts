import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import {  IGetAllProducts,IMyException,IProduct } from './shared/Models/Products';
import { faCoffee, faEye } from '@fortawesome/free-solid-svg-icons';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.html',
  standalone: false,
  styleUrl: './app.scss'
})
export class App {


  protected title = 'E-Commerce_Angular';


}
