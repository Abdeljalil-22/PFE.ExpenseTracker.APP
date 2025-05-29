import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { CategoryService } from '../../../services/category.service';
import { CategoryDto } from '../../../models/category.models';

@Component({
  selector: 'app-category-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule
  ],
  template: `
    <div class="category-list-container">
      <mat-card>
        <mat-card-header>
          <mat-card-title>Categories</mat-card-title>
          <div class="header-actions">
            <button mat-raised-button color="primary" routerLink="new">
              <mat-icon>add</mat-icon> New Category
            </button>
          </div>
        </mat-card-header>
        
        <mat-card-content>
          <table mat-table [dataSource]="categories" class="full-width">
            <ng-container matColumnDef="name">
              <th mat-header-cell *matHeaderCellDef>Name</th>
              <td mat-cell *matCellDef="let category">
                <div class="category-name">
                  <mat-icon [style.color]="category.color">{{category.icon || 'folder'}}</mat-icon>
                  <span>{{category.name}}</span>
                </div>
              </td>
            </ng-container>

            <ng-container matColumnDef="description">
              <th mat-header-cell *matHeaderCellDef>Description</th>
              <td mat-cell *matCellDef="let category">{{category.description}}</td>
            </ng-container>

            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef>Actions</th>
              <td mat-cell *matCellDef="let category">
                <button mat-icon-button [routerLink]="[category.id]">
                  <mat-icon>edit</mat-icon>
                </button>
                <button mat-icon-button color="warn" (click)="deleteCategory(category.id)" [disabled]="category.isDefault">
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
    .category-list-container {
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

    .category-name {
      display: flex;
      align-items: center;
      gap: 8px;
    }

    .mat-mdc-row:hover {
      background-color: rgba(0, 0, 0, 0.04);
    }
  `]
})
export class CategoryListComponent implements OnInit {
  categories: CategoryDto[] = [];
  displayedColumns = ['name', 'description', 'actions'];

  constructor(private categoryService: CategoryService) {}

  ngOnInit(): void {
    this.loadCategories();
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

  deleteCategory(id: string): void {
    if (confirm('Are you sure you want to delete this category?')) {
      this.categoryService.deleteCategory(id).subscribe({
        next: () => {
          this.categories = this.categories.filter(c => c.id !== id);
        },
        error: (error) => {
          console.error('Error deleting category:', error);
        }
      });
    }
  }
}
