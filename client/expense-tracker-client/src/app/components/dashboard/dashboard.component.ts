import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    RouterModule
  ],
  template: `
    <div class="dashboard-container">
      <div class="dashboard-header">
        <h1>Dashboard</h1>
      </div>
      
      <div class="dashboard-grid">
        <mat-card class="dashboard-card" routerLink="/expenses">
          <mat-card-header>
            <mat-card-title>Expenses</mat-card-title>
          </mat-card-header>
          <mat-card-content>
            <mat-icon>receipt</mat-icon>
            <p>Manage your expenses</p>
          </mat-card-content>
        </mat-card>

        <mat-card class="dashboard-card" routerLink="/budgets">
          <mat-card-header>
            <mat-card-title>Budgets</mat-card-title>
          </mat-card-header>
          <mat-card-content>
            <mat-icon>account_balance_wallet</mat-icon>
            <p>Track your budgets</p>
          </mat-card-content>
        </mat-card>

        <mat-card class="dashboard-card" routerLink="/categories">
          <mat-card-header>
            <mat-card-title>Categories</mat-card-title>
          </mat-card-header>
          <mat-card-content>
            <mat-icon>category</mat-icon>
            <p>Manage expense categories</p>
          </mat-card-content>
        </mat-card>

        <mat-card class="dashboard-card" routerLink="/goals">
          <mat-card-header>
            <mat-card-title>Financial Goals</mat-card-title>
          </mat-card-header>
          <mat-card-content>
            <mat-icon>flag</mat-icon>
            <p>Set and track goals</p>
          </mat-card-content>
        </mat-card>

        <mat-card class="dashboard-card" routerLink="/reports">
          <mat-card-header>
            <mat-card-title>Reports</mat-card-title>
          </mat-card-header>
          <mat-card-content>
            <mat-icon>bar_chart</mat-icon>
            <p>View financial reports</p>
          </mat-card-content>
        </mat-card>
      </div>
    </div>
  `,
  styles: [`
    .dashboard-container {
      padding: 24px;
      max-width: 1200px;
      margin: 0 auto;
    }

    .dashboard-header {
      margin-bottom: 24px;
    }

    .dashboard-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
      gap: 24px;
    }

    .dashboard-card {
      cursor: pointer;
      transition: transform 0.2s;

      &:hover {
        transform: translateY(-4px);
      }

      mat-card-content {
        display: flex;
        flex-direction: column;
        align-items: center;
        padding: 24px;
        text-align: center;

        mat-icon {
          font-size: 48px;
          height: 48px;
          width: 48px;
          margin-bottom: 16px;
        }
      }
    }
  `]
})
export class DashboardComponent implements OnInit {
  constructor() {}

  ngOnInit(): void {}
}
