import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EventService } from '../../services/event';
import { Auth } from '../../services/auth';
import { Event as EventModel, UpdateEventDto } from '../../models/event.models';

@Component({
  selector: 'app-event-edit',
  imports: [CommonModule, FormsModule],
  templateUrl: './event-edit.html',
  styleUrl: './event-edit.css'
})
export class EventEdit implements OnInit {
  private readonly eventService = inject(EventService);
  private readonly authService = inject(Auth);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly cdr = inject(ChangeDetectorRef);

  event: EventModel | null = null;
  isLoading = true;
  isSaving = false;
  errorMessage = '';
  successMessage = '';
  eventId = '';

  formData = {
    name: '',
    description: '',
    location: '',
    startTime: '',
    endTime: ''
  };

  ngOnInit(): void {
    
    const currentUser = this.authService.getCurrentUser();
    
    if (!currentUser || currentUser.role !== 'EventCreator') {
      this.router.navigate(['/events']);
      return;
    }

    this.eventId = this.route.snapshot.paramMap.get('id') || '';
    
    if (this.eventId) {
      this.loadEvent();
    } else {
      this.errorMessage = 'Invalid event ID';
      this.isLoading = false;
    }
  }

  loadEvent(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.eventService.getEventById(this.eventId).subscribe({
      next: (event) => {
        this.event = event;
        this.populateForm(event);
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('Error loading event:', error);
        this.errorMessage = `Failed to load event: ${error.status} ${error.statusText || error.message}`;
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  populateForm(event: EventModel): void {
    this.formData = {
      name: event.name,
      description: event.description,
      location: event.location,
      startTime: this.formatDateTimeForInput(event.startTime),
      endTime: this.formatDateTimeForInput(event.endTime)
    };
  }

  formatDateTimeForInput(dateTimeString: string): string {
    const date = new Date(dateTimeString);
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');
    return `${year}-${month}-${day}T${hours}:${minutes}`;
  }

  onSubmit(): void {
    if (!this.isValidForm()) {
      return;
    }

    this.isSaving = true;
    this.errorMessage = '';

    const updateData: UpdateEventDto = {
      name: this.formData.name,
      description: this.formData.description,
      location: this.formData.location,
      startTime: new Date(this.formData.startTime).toISOString(),
      endTime: new Date(this.formData.endTime).toISOString()
    };

    this.eventService.updateEvent(this.eventId, updateData).subscribe({
      next: () => {
        this.successMessage = 'Event updated successfully!';
        this.isSaving = false;
        this.cdr.detectChanges();
        setTimeout(() => {
          this.router.navigate(['/events']);
        }, 1500);
      },
      error: (error) => {
        console.error('Error updating event:', error);
        this.errorMessage = 'Failed to update event. Please try again.';
        this.isSaving = false;
        this.cdr.detectChanges();
      }
    });
  }

  isValidForm(): boolean {
    const { name, description, location, startTime, endTime } = this.formData;
    
    if (!name.trim() || !description.trim() || !location.trim() || !startTime || !endTime) {
      this.errorMessage = 'All fields are required.';
      return false;
    }

    const start = new Date(startTime);
    const end = new Date(endTime);

    if (start >= end) {
      this.errorMessage = 'End time must be after start time.';
      return false;
    }

    return true;
  }

  goBack(): void {
    this.router.navigate(['/events']);
  }
}