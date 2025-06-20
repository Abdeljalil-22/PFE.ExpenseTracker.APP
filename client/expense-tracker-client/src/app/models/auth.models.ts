export interface LoginCommand {
    email: string | null;
    password: string | null;
}

export interface RegisterCommand {
    email: string | null;
    userName: string | null;
    password: string | null;
    firstName: string | null;
    lastName: string | null;
}

export interface AuthResponse {
    token: string;
    userId: string;
}
