

import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms'; 
import { RouterLink, Router } from '@angular/router'; 
import { MovieShowing } from '../../models/core.models'; 
import { CinemaService } from '../../services/cinema.service';
import { AuthService } from '../../services/auth.service'; 

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink], 
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit {
  movies = signal<MovieShowing[]>([]);
  loading = signal<boolean>(true);
  searchTerm = '';

  topMovies = computed(() => this.movies().slice(0, 3));
  apiBase = "https://localhost:7049";

  constructor(
    private cinemaService: CinemaService,
    private router: Router,
    public authService: AuthService 
  ) {}

  ngOnInit(): void {
    this.getMovies();
  }

  handleBookingAction(movieId: number): void {
    if (this.authService.currentUser()) {
      this.router.navigate(['/book', movieId]);
    } else {
      this.warnAndRedirectToLogin(movieId);
    }
  }

  warnAndRedirectToLogin(movieId: number): void {
    alert('يجب تسجيل الدخول أولاً للمتابعة لصفحة الحجز.'); 
      this.router.navigate(['/login'], { queryParams: { returnUrl: `/book/${movieId}` } });

   
  }

  onLogout(): void {
    this.authService.logout();
    this.router.navigate(['/']);
  }

  getMovies(search?: string): void {
    this.loading.set(true);
    this.cinemaService.getMovies(search).subscribe({
      next: (data) => {
        this.movies.set(data);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  onSearch(): void {
    this.getMovies(this.searchTerm);
  }
}