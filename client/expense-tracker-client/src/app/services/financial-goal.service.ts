import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { 
  CreateFinancialGoalCommand, 
  UpdateFinancialGoalCommand,
  AddGoalContributionCommand 
} from '../models/financial-goal.models';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class FinancialGoalService {
  private apiUrl = `${environment.apiUrl}/api/FinancialGoals`;

  constructor(private http: HttpClient) {}

  getFinancialGoals(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  getFinancialGoalById(id: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  createFinancialGoal(command: CreateFinancialGoalCommand): Observable<void> {
    return this.http.post<void>(this.apiUrl, command);
  }

  updateFinancialGoal(id: string, command: UpdateFinancialGoalCommand): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, command);
  }

  deleteFinancialGoal(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  addContribution(goalId: string, command: AddGoalContributionCommand): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${goalId}/contributions`, command);
  }
}
