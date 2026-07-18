import { HttpEvent, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { BusyService } from '../services/busy-service';
import { delay, finalize, of, tap } from 'rxjs';

const cache=new Map<string,HttpEvent<unknown>>();

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const busyRequstedCount=inject(BusyService);

  if(req.method ==='GET'){
    const cacheResponces =cache.get(req.url)
    if(cacheResponces)
      return of(cacheResponces);
  }

  busyRequstedCount.busy();

  return next(req).pipe(
    delay(500),
    tap((response=>cache.set(req.url,response))),
    finalize(()=>{
      busyRequstedCount.idle()
    })
  );
};
