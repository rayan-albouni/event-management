import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { EventService } from '../../services/event';
import { CreateEventDto } from '../../models/event.models';

@Component({
  selector: 'app-event-create',
  imports: [CommonModule, FormsModule],
  templateUrl: './event-create.html',
  styleUrl: './event-create.css'
})
export class EventCreate {
  private readonly eventService = inject(EventService);
  private readonly router = inject(Router);

  eventData: CreateEventDto = { name: '', description: '', location: '', startTime: '', endTime: '' };
  isLoading = false;
  errorMessage = '';

  onSubmit(): void {
    const { name, description, location, startTime, endTime } = this.eventData;
    
    if (!name || !description || !location || !startTime || !endTime) {
      this.errorMessage = 'Please fill in all fields';
      return;
    }

    const start = new Date(startTime);
    const end = new Date(endTime);
    
    if (start <= new Date() || end <= start) {
      this.errorMessage = 'Invalid dates - start must be future, end must be after start';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    this.eventService.createEvent(this.eventData).subscribe({
      next: () => this.router.navigate(['/events']),
      error: (error) => {
        this.errorMessage = error.error?.message || 'Failed to create event';
        this.isLoading = false;
      },
      complete: () => this.isLoading = false
    });
  }

  getMinDateTime = () => new Date().toISOString().slice(0, 16);
  cancel = () => this.router.navigate(['/events']);
}
