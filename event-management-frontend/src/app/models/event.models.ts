export interface Event {
  id: string;
  name: string;
  description: string;
  location: string;
  startTime: string;
  endTime: string;
  creatorId: string;
  registrationCount: number;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateEventDto {
  name: string;
  description: string;
  location: string;
  startTime: string;
  endTime: string;
}

export interface UpdateEventDto {
  name?: string;
  description?: string;
  location?: string;
  startTime?: string;
  endTime?: string;
}

export interface Registration {
  id: string;
  name: string;
  phoneNumber: string;
  email: string;
  eventId: string;
  createdAt: string;
}

export interface CreateRegistrationDto {
  name: string;
  phoneNumber: string;
  email: string;
}