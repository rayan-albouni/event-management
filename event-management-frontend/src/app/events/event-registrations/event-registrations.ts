import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { EventService } from '../../services/event';
import { Auth } from '../../services/auth';
import { Event as EventModel, Registration } from '../../models/event.models';
import { User } from '../../models/auth.models';

@Component({
  selector: 'app-event-registrations',
  imports: [CommonModule],
  templateUrl: './event-registrations.html',
  styleUrl: './event-registrations.css'
})
export class EventRegistrations implements OnInit {
  private readonly eventService = inject(EventService);
  private readonly authService = inject(Auth);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly cdr = inject(ChangeDetectorRef);

  event: EventModel | null = null;
  registrations: Registration[] = [];
  currentUser: User | null = null;
  isLoading = true;
  errorMessage = '';
  eventId = '';

  ngOnInit(): void {
    this.currentUser = this.authService.getCurrentUser();
    
    if (this.currentUser?.role !== 'EventCreator') {
      this.router.navigate(['/events']);
      return;
    }

    this.eventId = this.route.snapshot.paramMap.get('id') || '';
    if (this.eventId) {
      this.loadEventAndRegistrations();
    }
  }

  loadEventAndRegistrations(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.eventService.getEventById(this.eventId).subscribe({
      next: (event) => {
        this.event = event;
        this.loadRegistrations();
      },
      error: (error) => {
        this.errorMessage = `Failed to load event: ${error.message || error.statusText || 'Unknown error'}`;
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  loadRegistrations(): void {
    this.eventService.getEventRegistrations(this.eventId).subscribe({
      next: (registrations) => {
        this.registrations = registrations;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (error) => {
        this.errorMessage = `Failed to load registrations: ${error.message || error.statusText || 'Unknown error'}`;
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/events']);
  }

  exportRegistrations(): void {
    if (!this.registrations.length) return;

    const csvContent = this.generateCSV();
    const blob = new Blob([csvContent], { type: 'text/csv' });
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = `${this.event?.name}-registrations.csv`;
    link.click();
    window.URL.revokeObjectURL(url);
  }

  private generateCSV(): string {
    const headers = ['Name', 'Email', 'Phone Number', 'Registration Date'];
    const csvHeaders = headers.join(',');
    
    const csvRows = this.registrations.map(reg => [
      reg.name,
      reg.email,
      reg.phoneNumber,
      new Date(reg.createdAt).toLocaleDateString()
    ].join(','));

    return [csvHeaders, ...csvRows].join('\n');
  }

  formatDateTime(dateString: string): string {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  trackByRegistrationId(index: number, registration: Registration): string {
    return registration.id;
  }
}