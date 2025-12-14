import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BookingRequest, Movie, Ticket } from '../models/core.models';
import { MovieShowing } from '../models/core.models'; 

@Injectable({
  providedIn: 'root'
})
export class CinemaService {
  private apiUrl = 'https://localhost:7049/api'; 

  constructor(private http: HttpClient) {}


  getMovies(searchTerm?: string): Observable<MovieShowing[]> {
    let params = new HttpParams();
    if (searchTerm) {
      params = params.set('search', searchTerm);
    }
    return this.http.get<MovieShowing[]>(`${this.apiUrl}/movies`, { params });
  }

  bookTicket(req: BookingRequest): Observable<Ticket> {
    return this.http.post<Ticket>(`${this.apiUrl}/book`, req);
  }



  addMovie(formData: FormData): Observable<any> {
  return this.http.post(`${this.apiUrl}/admin/add-movie`, formData);
}

  getBookedSeats(movieId: number): Observable<{SeatRow: number, SeatNumber: number}[]> {
  return this.http.get<{SeatRow: number, SeatNumber: number}[]>(`${this.apiUrl}/movies/${movieId}/booked-seats`);
}
}