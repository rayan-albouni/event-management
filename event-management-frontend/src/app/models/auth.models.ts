export interface User {
  id: string;
  email: string;
  role: 'EventCreator' | 'EventParticipant';
  isActive: boolean;
  createdAt: string;
  updatedAt?: string;
}

export interface LoginDto {
  email: string;
  password: string;
}

export interface RegisterDto {
  email: string;
  password: string;
  role: 'EventCreator' | 'EventParticipant';
}

export interface AuthResponse {
  user: User;
  token: string;
}