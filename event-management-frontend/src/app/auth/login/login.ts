import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Auth } from '../../services/auth';
import { LoginDto } from '../../models/auth.models';

@Component({
  selector: 'app-login',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login {
  private readonly authService = inject(Auth);
  private readonly router = inject(Router);

  credentials: LoginDto = { email: '', password: '' };
  isLoading = false;
  errorMessage = '';

  onSubmit(): void {
    if (!this.credentials.email || !this.credentials.password) {
      this.errorMessage = 'Please fill in all fields';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    this.authService.login(this.credentials).subscribe({
      next: () => this.router.navigate(['/events']),
      error: (error) => {
        this.errorMessage = error.error?.message || 'Login failed';
        this.isLoading = false;
      },
      complete: () => this.isLoading = false
    });
  }
}
