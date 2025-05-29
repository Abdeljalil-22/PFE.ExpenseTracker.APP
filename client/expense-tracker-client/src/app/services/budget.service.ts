import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CreateBudgetCommand, UpdateBudgetCommand } from '../models/budget.models';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class BudgetService {
  private apiUrl = `${environment.apiUrl}/api/Budgets`;

  constructor(private http: HttpClient) {}

  getBudgets(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  getBudgetById(id: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  getBudgetByCategory(categoryId: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/category/${categoryId}`);
  }

  createBudget(command: CreateBudgetCommand): Observable<void> {
    return this.http.post<void>(this.apiUrl, command);
  }

  updateBudget(id: string, command: UpdateBudgetCommand): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, command);
  }

  deleteBudget(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
