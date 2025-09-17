import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Auth } from '../../services/auth';
import { RegisterDto } from '../../models/auth.models';

@Component({
  selector: 'app-register',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class Register {
  private readonly authService = inject(Auth);
  private readonly router = inject(Router);

  userData: RegisterDto = { email: '', password: '', role: 'EventParticipant' };
  confirmPassword = '';
  isLoading = false;
  errorMessage = '';

  onSubmit(): void {
    if (!this.userData.email || !this.userData.password || !this.confirmPassword) {
      this.errorMessage = 'Please fill in all fields';
      return;
    }

    if (this.userData.password !== this.confirmPassword) {
      this.errorMessage = 'Passwords do not match';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    this.authService.register(this.userData).subscribe({
      next: () => this.router.navigate(['/login']),
      error: (error) => {
        this.errorMessage = error.error?.message || 'Registration failed';
        this.isLoading = false;
      },
      complete: () => this.isLoading = false
    });
  }
}
