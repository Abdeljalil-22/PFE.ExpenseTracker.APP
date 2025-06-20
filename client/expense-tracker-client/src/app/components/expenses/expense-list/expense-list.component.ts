import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatTooltipModule } from '@angular/material/tooltip';
import { ExpenseService } from '../../../services/expense.service';
import { ExpenseDto } from '../../../models/expense.models';

@Component({
  selector: 'app-expense-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatTooltipModule
  ],
  template: `
    <div class="expense-list-container">
      <mat-card>
        <mat-card-header>
          <mat-card-title>Expenses</mat-card-title>
          <div class="header-actions">
            <button mat-raised-button color="primary" routerLink="new">
              <mat-icon>add</mat-icon> New Expense
            </button>
          </div>
        </mat-card-header>
        
        <mat-card-content>
          <table mat-table [dataSource]="expenses" class="full-width">
            <ng-container matColumnDef="date">
              <th mat-header-cell *matHeaderCellDef>Date</th>
              <td mat-cell *matCellDef="let expense">{{expense.date | date}}</td>
            </ng-container>

            <ng-container matColumnDef="description">
              <th mat-header-cell *matHeaderCellDef>Description</th>
              <td mat-cell *matCellDef="let expense">{{expense.description}}</td>
            </ng-container>

            <ng-container matColumnDef="category">
              <th mat-header-cell *matHeaderCellDef>Category</th>
              <td mat-cell *matCellDef="let expense">{{expense.category?.name}}</td>
            </ng-container>

            <ng-container matColumnDef="amount">
              <th mat-header-cell *matHeaderCellDef>Amount</th>
              <td mat-cell *matCellDef="let expense">{{expense.amount | currency}}</td>
            </ng-container>

            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef>Actions</th>
              <td mat-cell *matCellDef="let expense">
                <button mat-icon-button [routerLink]="[expense.id]" matTooltip="Edit">
                  <mat-icon>edit</mat-icon>
                </button>
                <button mat-icon-button color="warn" (click)="deleteExpense(expense.id)" matTooltip="Delete">
                  <mat-icon>delete</mat-icon>
                </button>
              </td>
            </ng-container>

            <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
            <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
          </table>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .expense-list-container {
      padding: 24px;
      max-width: 1200px;
      margin: 0 auto;
    }

    .header-actions {
      margin-left: auto;
    }

    .full-width {
      width: 100%;
    }

    .mat-mdc-row:hover {
      background-color: rgba(0, 0, 0, 0.04);
    }
  `]
})
export class ExpenseListComponent implements OnInit {
  expenses: ExpenseDto[] = [];
  displayedColumns = ['date', 'description', 'category', 'amount', 'actions'];

  constructor(private expenseService: ExpenseService) {}

  ngOnInit(): void {
    this.loadExpenses();
  }

  loadExpenses(): void {
    this.expenseService.getExpenses().subscribe({
      next: (expenses) => {
        this.expenses = expenses;
      },
      error: (error) => {
        console.error('Error loading expenses:', error);
      }
    });
  }

  deleteExpense(id: string): void {
    if (confirm('Are you sure you want to delete this expense?')) {
      this.expenseService.deleteExpense(id).subscribe({
        next: () => {
          this.expenses = this.expenses.filter(e => e.id !== id);
        },
        error: (error) => {
          console.error('Error deleting expense:', error);
        }
      });
    }
  }
}
