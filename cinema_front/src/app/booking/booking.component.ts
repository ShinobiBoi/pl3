
import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { CinemaService } from '../services/cinema.service';
import { AuthService } from '../services/auth.service';
import { BookingRequest, Ticket, MovieShowing } from '../models/core.models';
import { RouterLink, Router } from '@angular/router'; 


interface Seat {
  id: string; row: string; num: number;
  category: 'STANDARD' | 'GOLD' | 'PLATINUM';
  occupied: boolean;
}

@Component({
  selector: 'app-booking',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './booking.component.html',
  styleUrl: './booking.component.css'
})
export class BookingComponent implements OnInit {
  currentMovie = signal<MovieShowing | null>(null);
  loading = signal(true);
  rows = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H'];
  seats = signal<Seat[]>([]);
  selectedSeat = signal<Seat | null>(null);
  
  bookedSeatsFromDB = signal<{SeatRow: number, SeatNumber: number}[]>([]);

  constructor(
    private route: ActivatedRoute,
    private cinemaService: CinemaService,
    private authService: AuthService,
        private router: Router

  ) {}

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      const movieId = Number(id);
      this.loadMovieData(movieId);
      
      this.cinemaService.getBookedSeats(movieId).subscribe({
        next: (data) => {
          this.bookedSeatsFromDB.set(data);
          this.generateSeats(); 
        },
        error: () => this.generateSeats() 
      });
    }
  }

  loadMovieData(id: number) {
    this.cinemaService.getMovies().subscribe({
      next: (movies) => {
        const movie = movies.find(m => m.id === id);
        this.currentMovie.set(movie || null);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  generateSeats() {
    const generated: Seat[] = [];
    const booked = this.bookedSeatsFromDB(); 

    this.rows.forEach(row => {
      for (let i = 1; i <= 8; i++) {
        const currentRowNum = row.charCodeAt(0) - 64; 
        
        const isOccupied = booked.some(s => s.SeatRow === currentRowNum && s.SeatNumber === i);

        let cat: 'STANDARD' | 'GOLD' | 'PLATINUM' = 'STANDARD';
        if (['A', 'B'].includes(row)) cat = 'PLATINUM';
        else if (['C', 'D'].includes(row)) cat = 'GOLD';

        generated.push({ 
          id: `${row}${i}`, 
          row, 
          num: i, 
          category: cat, 
          occupied: isOccupied 
        });
      }
    });
    this.seats.set(generated);
  }


calculateTotalPrice(): number {
  const movie = this.currentMovie();
  const seat = this.selectedSeat();
  
  if (!movie || !seat) return 0;

  let multiplier = 1.0;
  if (seat.category === 'PLATINUM') multiplier = 2.0;
  else if (seat.category === 'GOLD') multiplier = 1.5;

  return movie.basePrice * multiplier;
}

  selectSeat(seat: Seat) { if (!seat.occupied) this.selectedSeat.set(seat); }

  confirmBooking() {
    const seat = this.selectedSeat();
    const movie = this.currentMovie();
    const user = this.authService.currentUser();

    if (!seat || !movie || !user) return;

    const bookingData: BookingRequest = {
      userId: Number(user?.id), 
      movieId: movie.id,
      seatRow: seat.row.charCodeAt(0) - 64,
      seatNumber: seat.num,
      category: seat.category
    };

    this.cinemaService.bookTicket(bookingData).subscribe({
      next: (ticket: Ticket) => this.downloadTicket(ticket),
      error: (err) => {
        console.error('API Error Details:', err.error);
        alert('عذراً: ' + err.error);
      }
    });
  }

  private downloadTicket(t: Ticket) {
    const movie = this.currentMovie();
    const seat = this.selectedSeat();
    const user = this.authService.currentUser();
    const ticketContent = `
    --- MOVIE MAKER OFFICIAL TICKET ---
    Ticket ID: ${t.id}
    Movie: ${movie?.title || t.movieId}
    Show Date: ${movie?.showDate ? new Date(movie.showDate).toLocaleString() : 'N/A'}
    Hall Number: ${movie?.hallNumber || 'N/A'}
    Seat: ${seat?.row}${seat?.num}  
    Category: ${t.ticketType}
    Price: ${t.price} EGP
    Date: ${new Date(t.bookingDate).toLocaleString()}
    -----------------------------------
    `;

    const blob = new Blob([ticketContent], { type: 'text/plain' });
    const a = document.createElement('a');
    a.href = URL.createObjectURL(blob);
    a.download = `Ticket_${t.id}.txt`;
    a.click();
  }
  
  onLogout(): void {
    this.authService.logout();
    this.router.navigate(['/']);
  }

}