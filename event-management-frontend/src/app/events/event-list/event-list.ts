import { Component, inject, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subscription } from 'rxjs';
import { EventService } from '../../services/event';
import { Auth } from '../../services/auth';
import { Event as EventModel, CreateRegistrationDto } from '../../models/event.models';
import { User } from '../../models/auth.models';

@Component({
  selector: 'app-event-list',
  imports: [CommonModule, FormsModule],
  templateUrl: './event-list.html',
  styleUrl: './event-list.css'
})
export class EventList implements OnInit, OnDestroy {
  private readonly eventService = inject(EventService);
  private readonly authService = inject(Auth);
  private readonly router = inject(Router);
  private readonly cdr = inject(ChangeDetectorRef);
  private userSubscription?: Subscription;

  events: EventModel[] = [];
  currentUser: User | null = null;
  isLoading = true;
  errorMessage = '';
  showRegistrationModal = false;
  selectedEvent: EventModel | null = null;
  registrationData: CreateRegistrationDto = { name: '', phoneNumber: '', email: '' };

  ngOnInit(): void {
    this.userSubscription = this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
      this.cdr.detectChanges();
    });
    this.loadEvents();
  }

  ngOnDestroy(): void {
    this.userSubscription?.unsubscribe();
  }

  loadEvents(): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.eventService.getAllEvents().subscribe({
      next: (events) => {
        this.events = events;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (error) => {
        this.errorMessage = `Failed to load events: ${error.message || 'Unknown error'}`;
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  openRegistrationModal(event: EventModel): void {
    this.selectedEvent = event;
    this.registrationData = { name: '', phoneNumber: '', email: this.currentUser?.email || '' };
    this.showRegistrationModal = true;
  }

  closeRegistrationModal(): void {
    this.showRegistrationModal = false;
    this.selectedEvent = null;
  }

  registerForEvent(): void {
    if (!this.selectedEvent) return;
    this.eventService.registerForEvent(this.selectedEvent.id, this.registrationData).subscribe({
      next: () => {
        alert('Successfully registered!');
        this.closeRegistrationModal();
        this.loadEvents();
      },
      error: (error) => alert('Registration failed: ' + (error.error?.message || error.message))
    });
  }

  deleteEvent(event: EventModel): void {
    if (confirm(`Delete "${event.name}"?`)) {
      this.eventService.deleteEvent(event.id).subscribe({
        next: () => this.loadEvents(),
        error: (error) => {
          this.errorMessage = `Delete failed: ${error.message || 'Unknown error'}`;
          this.cdr.detectChanges();
        }
      });
    }
  }

  navigateToCreateEvent = () => this.router.navigate(['/events/create']);
  modifyEvent = (event: EventModel) => this.router.navigate(['/events', event.id, 'edit']);
  viewRegistrations = (event: EventModel) => this.router.navigate(['/events', event.id, 'registrations']);
  logout = () => { this.authService.logout(); this.router.navigate(['/login']); };
  isEventCreator = () => this.currentUser?.role === 'EventCreator';
  isEventParticipant = () => this.currentUser?.role === 'EventParticipant';
  formatDateTime = (dateString: string) => new Date(dateString).toLocaleDateString('en-US', {
    year: 'numeric', month: 'short', day: 'numeric', hour: '2-digit', minute: '2-digit'
  });
}
