import { Routes } from '@angular/router';
import { LoginComponent } from './components/auth/login.component';
import { RegisterComponent } from './components/auth/register.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { LayoutComponent } from './components/layout/layout.component';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  {
    path: '',
    component: LayoutComponent,
    canActivate: [authGuard],
    children: [
      { path: '', component: DashboardComponent },
      { 
        path: 'expenses',
        loadChildren: () => import('./components/expenses/expenses.routes').then(m => m.EXPENSE_ROUTES)
      },
      {
        path: 'budgets',
        loadChildren: () => import('./components/budgets/budgets.routes').then(m => m.BUDGET_ROUTES)
      },
      {
        path: 'categories',
        loadChildren: () => import('./components/categories/categories.routes').then(m => m.CATEGORY_ROUTES)
      },
      {
        path: 'goals',
        loadChildren: () => import('./components/goals/goals.routes').then(m => m.GOAL_ROUTES)
      },
      {
        path: 'reports',
        loadChildren: () => import('./components/reports/reports.routes').then(m => m.REPORT_ROUTES)
      }
    ]
  },
  { path: '**', redirectTo: '' }
];
