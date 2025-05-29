import { CategoryDto } from './category.models';

export interface ExpenseDto {
    id: string;
    description: string | null;
    amount: number;
    date: string;
    isRecurring: boolean;
    recurringFrequency: string | null;
    nextRecurringDate: string | null;
    isShared: boolean;
    notes: string | null;
    category: CategoryDto;
    attachments: AttachmentDto[] | null;
}

export interface CreateExpenseCommand {
    userId: string;
    description: string | null;
    amount: number;
    date: string;
    categoryId: string;
    isRecurring: boolean;
    recurringFrequency: string | null;
    isShared: boolean;
    notes: string | null;
}

export interface UpdateExpenseCommand extends CreateExpenseCommand {
    id: string;
}

export interface AttachmentDto {
    id: string;
    fileName: string | null;
    filePath: string | null;
    contentType: string | null;
    fileSize: number;
}
