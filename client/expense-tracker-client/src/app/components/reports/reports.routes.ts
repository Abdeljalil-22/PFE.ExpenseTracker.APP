import { Routes } from '@angular/router';

export const REPORT_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./report-dashboard/report-dashboard.component').then(m => m.ReportDashboardComponent)
  },
  {
    path: 'expenses',
    loadComponent: () => import('./expense-report/expense-report.component').then(m => m.ExpenseReportComponent)
  },
  {
    path: 'budgets',
    loadComponent: () => import('./budget-report/budget-report.component').then(m => m.BudgetReportComponent)
  },
  {
    path: 'annual-summary',
    loadComponent: () => import('./annual-summary/annual-summary.component').then(m => m.AnnualSummaryComponent)
  }
];
