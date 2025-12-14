
export interface User {
  id: number; 
  username: string;
  role: string; 
  token?: string; 
}

export interface MovieShowing {
  id: number; 
  title: string;
  description: string;
  imageUrl: string; 
  showDate: string; 
  hallNumber: number; 
  basePrice: number;
}

export interface RegisterRequest {
  username: string;
  password: string;
  email: string;
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface Movie {
  id?: number; 
  title: string;
  description: string;
  showDate: string;
  basePrice: number;
  hallNumber?: number;
  posterUrl?: string; 
}

export interface BookingRequest {
  userId: number;
  movieId: number;
  seatRow: number;
  seatNumber: number;
  category: 'PLATINUM' | 'GOLD' | 'STANDARD'; 
}

export interface Ticket {
  id: string;
  userId: number;
  movieId: number;
  seatRow: number;
  seatNumber: number;
  ticketType: string;
  price: number;
  isPaid: boolean;
  bookingDate: Date;
}
