import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ExpenseDto, CreateExpenseCommand, UpdateExpenseCommand } from '../models/expense.models';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ExpenseService {
  private apiUrl = `${environment.apiUrl}/api/Expenses`;

  constructor(private http: HttpClient) {}

  getExpenses(params?: {
    userId?: string;
    categoryId?: string;
    startDate?: string;
    endDate?: string;
    isRecurring?: boolean;
  }): Observable<ExpenseDto[]> {
    let httpParams = new HttpParams();
    if (params) {
      Object.keys(params).forEach(key => {
        const value = params[key as keyof typeof params];
        if (value !== undefined) {
          httpParams = httpParams.set(key, value.toString());
        }
      });
    }
    return this.http.get<ExpenseDto[]>(this.apiUrl, { params: httpParams });
  }

  getExpenseById(id: string): Observable<ExpenseDto> {
    return this.http.get<ExpenseDto>(`${this.apiUrl}/${id}`);
  }

  createExpense(command: CreateExpenseCommand): Observable<void> {
    return this.http.post<void>(this.apiUrl, command);
  }

  updateExpense(id: string, command: UpdateExpenseCommand): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, command);
  }

  deleteExpense(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
