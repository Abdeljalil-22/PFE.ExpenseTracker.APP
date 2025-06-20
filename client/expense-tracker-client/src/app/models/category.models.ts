export interface CategoryDto {
    id: string;
    name: string | null;
    description: string | null;
    icon: string | null;
    color: string | null;
    isDefault: boolean;
}

export interface CreateCategoryCommand {
    userId: string;
    name: string | null;
    description: string | null;
    icon: string | null;
    color: string | null;
}

export interface UpdateCategoryCommand extends CreateCategoryCommand {
    id: string;
}
