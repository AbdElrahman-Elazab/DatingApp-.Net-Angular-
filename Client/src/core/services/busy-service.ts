import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class BusyService {
  busyRequstedCount=signal(0);

  busy(){
this.busyRequstedCount.update(current=>current+1);
  }
  idle(){
this.busyRequstedCount.update(current=>Math.max(current-1,0));
  }
}
