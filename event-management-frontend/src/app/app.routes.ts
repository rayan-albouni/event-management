import { Routes } from '@angular/router';
import { Login } from './auth/login/login';
import { Register } from './auth/register/register';
import { EventList } from './events/event-list/event-list';
import { EventCreate } from './events/event-create/event-create';
import { EventEdit } from './events/event-edit/event-edit';
import { EventRegistrations } from './events/event-registrations/event-registrations';

export const routes: Routes = [
  { path: '', redirectTo: '/events', pathMatch: 'full' },
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  { path: 'events', component: EventList },
  { path: 'events/create', component: EventCreate },
  { path: 'events/:id/edit', component: EventEdit },
  { path: 'events/:id/registrations', component: EventRegistrations },
  { path: '**', redirectTo: '/events' }
];
