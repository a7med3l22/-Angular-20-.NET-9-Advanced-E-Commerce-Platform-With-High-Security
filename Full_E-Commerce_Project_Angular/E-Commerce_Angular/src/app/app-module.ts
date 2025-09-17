import { NgModule, provideBrowserGlobalErrorListeners } from '@angular/core';
import { BrowserModule, provideClientHydration, withEventReplay } from '@angular/platform-browser';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { AppRoutingModule } from './app-routing-module';
import { App } from './app';
import { CoreModule } from './core/core-module';
import { ShopModule } from '../shop/shop-module';
import { SharedModule } from './shared/shared-module';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { ShopRoutingModule } from '../shop/shop-routing-module';
import { RouterModule } from '@angular/router';
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
// Import library module
import { NgxSpinnerModule } from "ngx-spinner";
import { loadingInterceptor } from './core/loading-interceptor';
import { AboutUs } from './about-us/about-us';
import { BestSeller } from './best-seller/best-seller';
@NgModule({
  declarations: [
    App,
    AboutUs,
    BestSeller
  ],
  imports: [
    FontAwesomeModule,
    SharedModule,
    BrowserModule,
    AppRoutingModule,
  
     BrowserAnimationsModule,
      NgxSpinnerModule.forRoot(),
      CoreModule

    
  ],
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideClientHydration(withEventReplay()),
    provideHttpClient(withFetch()) 
    // function withFetch(): HttpFeature<HttpFeatureKind.Fetch>;
    // provideHttpClient(...features: HttpFeature<HttpFeatureKind>[])
    ,  provideHttpClient(
    withInterceptors([loadingInterceptor]),
  )
  ],
  bootstrap: [App]
})
export class AppModule { 
}
