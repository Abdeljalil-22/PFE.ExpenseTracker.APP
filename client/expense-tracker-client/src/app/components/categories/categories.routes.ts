import { Routes } from '@angular/router';

export const CATEGORY_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./category-list/category-list.component').then(m => m.CategoryListComponent)
  },
  {
    path: 'new',
    loadComponent: () => import('./category-form/category-form.component').then(m => m.CategoryFormComponent)
  },
  {
    path: ':id',
    loadComponent: () => import('./category-form/category-form.component').then(m => m.CategoryFormComponent)
  }
];
