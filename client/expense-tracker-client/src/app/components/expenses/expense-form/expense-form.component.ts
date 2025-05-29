import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatCardModule } from '@angular/material/card';
import { ExpenseService } from '../../../services/expense.service';
import { CategoryService } from '../../../services/category.service';
import { CategoryDto } from '../../../models/category.models';

@Component({
  selector: 'app-expense-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatCheckboxModule,
    MatCardModule
  ],
  template: `
    <div class="expense-form-container">
      <mat-card>
        <mat-card-header>
          <mat-card-title>{{isEditing ? 'Edit' : 'New'}} Expense</mat-card-title>
        </mat-card-header>
        
        <mat-card-content>
          <form [formGroup]="expenseForm" (ngSubmit)="onSubmit()">
            <mat-form-field appearance="fill">
              <mat-label>Description</mat-label>
              <input matInput formControlName="description" required>
            </mat-form-field>

            <mat-form-field appearance="fill">
              <mat-label>Amount</mat-label>
              <input matInput type="number" formControlName="amount" required>
            </mat-form-field>

            <mat-form-field appearance="fill">
              <mat-label>Date</mat-label>
              <input matInput [matDatepicker]="picker" formControlName="date" required>
              <mat-datepicker-toggle matSuffix [for]="picker"></mat-datepicker-toggle>
              <mat-datepicker #picker></mat-datepicker>
            </mat-form-field>

            <mat-form-field appearance="fill">
              <mat-label>Category</mat-label>
              <mat-select formControlName="categoryId" required>
                <mat-option *ngFor="let category of categories" [value]="category.id">
                  {{category.name}}
                </mat-option>
              </mat-select>
            </mat-form-field>

            <mat-checkbox formControlName="isRecurring">Recurring Expense</mat-checkbox>

            <mat-form-field appearance="fill" *ngIf="expenseForm.get('isRecurring')?.value">
              <mat-label>Recurring Frequency</mat-label>
              <mat-select formControlName="recurringFrequency">
                <mat-option value="daily">Daily</mat-option>
                <mat-option value="weekly">Weekly</mat-option>
                <mat-option value="monthly">Monthly</mat-option>
                <mat-option value="yearly">Yearly</mat-option>
              </mat-select>
            </mat-form-field>

            <mat-checkbox formControlName="isShared">Shared Expense</mat-checkbox>

            <mat-form-field appearance="fill">
              <mat-label>Notes</mat-label>
              <textarea matInput formControlName="notes" rows="4"></textarea>
            </mat-form-field>

            <div class="form-actions">
              <button mat-button type="button" routerLink="/expenses">Cancel</button>
              <button mat-raised-button color="primary" type="submit" [disabled]="!expenseForm.valid">
                {{isEditing ? 'Update' : 'Create'}} Expense
              </button>
            </div>
          </form>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .expense-form-container {
      padding: 24px;
      max-width: 600px;
      margin: 0 auto;
    }

    form {
      display: flex;
      flex-direction: column;
      gap: 16px;
    }

    .form-actions {
      display: flex;
      gap: 16px;
      justify-content: flex-end;
      margin-top: 16px;
    }
  `]
})
export class ExpenseFormComponent implements OnInit {
  expenseForm: FormGroup;
  categories: CategoryDto[] = [];
  isEditing = false;
  
  constructor(
    private fb: FormBuilder,
    private expenseService: ExpenseService,
    private categoryService: CategoryService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.expenseForm = this.fb.group({
      description: ['', Validators.required],
      amount: ['', [Validators.required, Validators.min(0)]],
      date: ['', Validators.required],
      categoryId: ['', Validators.required],
      isRecurring: [false],
      recurringFrequency: [''],
      isShared: [false],
      notes: ['']
    });
  }

  ngOnInit(): void {
    this.loadCategories();
    
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditing = true;
      this.loadExpense(id);
    }
  }

  loadCategories(): void {
    this.categoryService.getCategories().subscribe({
      next: (categories) => {
        this.categories = categories;
      },
      error: (error) => {
        console.error('Error loading categories:', error);
      }
    });
  }

  loadExpense(id: string): void {
    this.expenseService.getExpenseById(id).subscribe({
      next: (expense) => {
        this.expenseForm.patchValue({
          description: expense.description,
          amount: expense.amount,
          date: new Date(expense.date),
          categoryId: expense.category.id,
          isRecurring: expense.isRecurring,
          recurringFrequency: expense.recurringFrequency,
          isShared: expense.isShared,
          notes: expense.notes
        });
      },
      error: (error) => {
        console.error('Error loading expense:', error);
      }
    });
  }

  onSubmit(): void {
    if (this.expenseForm.valid) {
      const expense = {
        ...this.expenseForm.value,
        date: new Date(this.expenseForm.value.date).toISOString()
      };

      const request = this.isEditing ?
        this.expenseService.updateExpense(this.route.snapshot.paramMap.get('id')!, expense) :
        this.expenseService.createExpense(expense);

      request.subscribe({
        next: () => {
          this.router.navigate(['/expenses']);
        },
        error: (error) => {
          console.error('Error saving expense:', error);
        }
      });
    }
  }
}
