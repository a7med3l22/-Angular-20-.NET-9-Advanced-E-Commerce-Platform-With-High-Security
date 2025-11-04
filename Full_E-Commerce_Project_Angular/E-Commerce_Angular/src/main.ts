import { platformBrowser } from '@angular/platform-browser';
import { AppModule } from './app/app-module';

navigator.serviceWorker.register('firebase-messaging-sw.js')
  .then((registration) => {
    console.log('✅ Service Worker Registered:', registration);
  })
  .catch((err) => console.error('❌ SW registration failed:', err));

platformBrowser().bootstrapModule(AppModule, {
  ngZoneEventCoalescing: true,
})
  .catch(err => console.error(err));
