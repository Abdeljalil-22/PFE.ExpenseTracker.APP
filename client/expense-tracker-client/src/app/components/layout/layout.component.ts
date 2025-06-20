import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatToolbarModule,
    MatButtonModule,
    MatIconModule,
    MatSidenavModule,
    MatListModule
  ],
  template: `
    <mat-toolbar color="primary">
      <button mat-icon-button (click)="sidenav.toggle()">
        <mat-icon>menu</mat-icon>
      </button>
      <span>Expense Tracker</span>
      <span class="toolbar-spacer"></span>
      <button mat-icon-button [matMenuTriggerFor]="userMenu">
        <mat-icon>account_circle</mat-icon>
      </button>
    </mat-toolbar>

    <mat-sidenav-container>
      <mat-sidenav #sidenav mode="side" opened>
        <mat-nav-list>
          <a mat-list-item routerLink="/" routerLinkActive="active" [routerLinkActiveOptions]="{exact: true}">
            <mat-icon>dashboard</mat-icon>
            <span>Dashboard</span>
          </a>
          <a mat-list-item routerLink="/expenses" routerLinkActive="active">
            <mat-icon>receipt</mat-icon>
            <span>Expenses</span>
          </a>
          <a mat-list-item routerLink="/budgets" routerLinkActive="active">
            <mat-icon>account_balance_wallet</mat-icon>
            <span>Budgets</span>
          </a>
          <a mat-list-item routerLink="/categories" routerLinkActive="active">
            <mat-icon>category</mat-icon>
            <span>Categories</span>
          </a>
          <a mat-list-item routerLink="/goals" routerLinkActive="active">
            <mat-icon>flag</mat-icon>
            <span>Financial Goals</span>
          </a>
          <a mat-list-item routerLink="/reports" routerLinkActive="active">
            <mat-icon>bar_chart</mat-icon>
            <span>Reports</span>
          </a>
        </mat-nav-list>
      </mat-sidenav>

      <mat-sidenav-content>
        <div class="content">
          <router-outlet></router-outlet>
        </div>
      </mat-sidenav-content>
    </mat-sidenav-container>

    <mat-menu #userMenu="matMenu">
      <button mat-menu-item routerLink="/profile">
        <mat-icon>person</mat-icon>
        <span>Profile</span>
      </button>
      <button mat-menu-item (click)="logout()">
        <mat-icon>exit_to_app</mat-icon>
        <span>Logout</span>
      </button>
    </mat-menu>
  `,
  styles: [`
    :host {
      display: flex;
      flex-direction: column;
      height: 100vh;
    }

    .toolbar-spacer {
      flex: 1 1 auto;
    }

    mat-sidenav-container {
      flex: 1;
    }

    mat-sidenav {
      width: 250px;
    }

    .content {
      padding: 24px;
    }

    mat-nav-list {
      .mat-icon {
        margin-right: 16px;
      }
    }

    .active {
      background-color: rgba(0, 0, 0, 0.04);
    }
  `]
})
export class LayoutComponent {
  constructor(private authService: AuthService) {}

  logout() {
    this.authService.logout();
  }
}
