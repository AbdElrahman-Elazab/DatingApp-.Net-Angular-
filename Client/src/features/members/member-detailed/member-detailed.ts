import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { MemberService } from '../../../core/services/member-service';
import { filter, Observable } from 'rxjs';
import { Member } from '../../../types/members';
import { ActivatedRoute, NavigationEnd, Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AsyncPipe } from '@angular/common';
import { AgePipe } from "../../../core/pipes/age-pipe";
import { AccountService } from '../../../core/services/account-service';

@Component({
  selector: 'app-member-detailed',
  imports: [AsyncPipe, RouterLink, RouterLinkActive, RouterOutlet, AgePipe],
  templateUrl: './member-detailed.html',
  styleUrl: './member-detailed.css',
})
export class MemberDetailed implements OnInit {
  memberService = inject(MemberService);
  route = inject(ActivatedRoute);
  accountService = inject(AccountService);
  router = inject(Router);
  title=signal<string | undefined>('Profile');
  isCurrentUser=computed(()=> this.accountService.currentUser()?.id === this.route.snapshot.paramMap.get('id'));

  ngOnInit(): void {


    this.title.set(this.route.firstChild?.snapshot?.title)

    this.router.events.pipe(
      filter(event=>event instanceof NavigationEnd)
    ).subscribe({
      next:()=>{
    this.title.set(this.route.firstChild?.snapshot?.title)

      }
    })
  }

}
