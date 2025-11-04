import { NgModule, provideBrowserGlobalErrorListeners } from '@angular/core';
import { BrowserModule, provideClientHydration, withEventReplay } from '@angular/platform-browser';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { AppRoutingModule } from './app-routing-module';
import { App } from './app';
import { CoreModule } from './core/core-module';
import { SharedModule } from './shared/shared-module';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { ShopRoutingModule } from '../shop/shop-routing-module';
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { NgxSpinnerModule } from "ngx-spinner";
import { loadingInterceptor } from './core/loading-interceptor';
import { AboutUs } from './about-us/about-us';

@NgModule({
  declarations: [
    App,
    AboutUs
  ],
  imports: [
    FontAwesomeModule,
    SharedModule,
    BrowserModule,
    AppRoutingModule,
    ShopRoutingModule,
    BrowserAnimationsModule,
    NgxSpinnerModule.forRoot(),
    CoreModule
  ],
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideClientHydration(withEventReplay()),

    // ✅ provideHttpClient مرة واحدة
    provideHttpClient(
      withFetch(),
      withInterceptors([loadingInterceptor])
    )
  ],
  bootstrap: [App]
})
export class AppModule { }
