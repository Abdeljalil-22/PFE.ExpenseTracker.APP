export interface CreateFinancialGoalCommand {
    userId: string;
    name: string | null;
    description: string | null;
    targetAmount: number;
    startDate: string;
    targetDate: string;
}

export interface UpdateFinancialGoalCommand extends CreateFinancialGoalCommand {
    id: string;
}

export interface AddGoalContributionCommand {
    financialGoalId: string;
    userId: string;
    amount: number;
    date: string;
    notes: string | null;
}
