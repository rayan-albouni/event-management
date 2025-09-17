import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Event as EventModel, CreateEventDto, UpdateEventDto, Registration, CreateRegistrationDto } from '../models/event.models';
import { Auth } from './auth';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class EventService {
  private readonly http = inject(HttpClient);
  private readonly authService = inject(Auth);
  private readonly apiUrl = `${environment.apiUrl}`;


  private getHeaders(): HttpHeaders {
    const token = this.authService.getToken();
    let headers = new HttpHeaders({
      'Content-Type': 'application/json'
    });
    
    if (token) {
      headers = headers.set('Authorization', `Bearer ${token}`);
    }
    return headers;
  }

  getAllEvents(): Observable<EventModel[]> {
    return this.http.get<EventModel[]>(`${this.apiUrl}/events`, {
      headers: this.getHeaders()
    });
  }

  getEventById(id: string): Observable<EventModel> {
    return this.http.get<EventModel>(`${this.apiUrl}/events/${id}`, {
      headers: this.getHeaders()
    });
  }

  createEvent(eventData: CreateEventDto): Observable<EventModel> {
    return this.http.post<EventModel>(`${this.apiUrl}/events`, eventData, {
      headers: this.getHeaders()
    });
  }

  updateEvent(id: string, eventData: UpdateEventDto): Observable<EventModel> {
    return this.http.put<EventModel>(`${this.apiUrl}/events/${id}`, eventData, {
      headers: this.getHeaders()
    });
  }

  deleteEvent(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/events/${id}`, {
      headers: this.getHeaders()
    });
  }

  registerForEvent(eventId: string, registrationData: CreateRegistrationDto): Observable<Registration> {
    return this.http.post<Registration>(`${this.apiUrl}/events/${eventId}/registrations`, registrationData);
  }

  getEventRegistrations(eventId: string): Observable<Registration[]> {
    return this.http.get<Registration[]>(`${this.apiUrl}/events/${eventId}/registrations`, {
      headers: this.getHeaders()
    });
  }
}
