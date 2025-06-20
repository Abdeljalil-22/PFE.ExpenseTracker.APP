import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ReportService {
  private apiUrl = `${environment.apiUrl}/api/Reports`;

  constructor(private http: HttpClient) {}

  getExpenseReport(startDate: string, endDate: string, format: string = 'PDF'): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/expenses`, {
      params: { startDate, endDate, format },
      responseType: 'blob'
    });
  }

  getBudgetReport(startDate: string, endDate: string, format: string = 'PDF'): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/budgets`, {
      params: { startDate, endDate, format },
      responseType: 'blob'
    });
  }

  getAnnualSummaryReport(year: number, format: string = 'PDF'): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/annual-summary`, {
      params: { year: year.toString(), format },
      responseType: 'blob'
    });
  }
}
