export interface CreateBudgetCommand {
    userId: string;
    categoryId: string;
    amount: number;
    startDate: string;
    endDate: string;
    period: string | null;
}

export interface UpdateBudgetCommand extends CreateBudgetCommand {
    id: string;
}
