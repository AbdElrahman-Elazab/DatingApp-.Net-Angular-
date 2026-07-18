import { Component, inject, signal, Signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../../core/services/account-service';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { ToastService } from '../../core/services/toast-service';
import { themes } from '../theme';
import { BusyService } from '../../core/services/busy-service';

@Component({
  selector: 'app-nav',
  imports: [FormsModule, RouterLink, RouterLinkActive],
  templateUrl: './nav.html',
  styleUrl: './nav.css',
})
export class Nav {
  protected accountService = inject(AccountService);
  protected router = inject(Router);
  protected busyService = inject(BusyService);
  protected toast=inject(ToastService)
  protected creds: any = {};
  selectedTheme=signal<string>(localStorage.getItem('theme')|| 'ligth')
  themes=themes

  handleSelectTheme(theme:string){
    this.selectedTheme.set(theme);
    localStorage.setItem('theme',theme);
    document.documentElement.setAttribute('data-theme',theme)
    const elem=document.activeElement as HTMLDivElement
    if(elem) elem.blur()
  }

  login() {
    this.accountService.login(this.creds).subscribe({
      next: (result) => {
        this.router.navigateByUrl('/members');
        this.toast.success('Login is successfully')
        this.creds = {};
      },

      error: (error) => {this.toast.error(error.message,3000)},
    });
  }

  logout() {
    this.accountService.currentUser.set(null);
    this.router.navigateByUrl('/');
  }
}
