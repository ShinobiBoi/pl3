import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CinemaService } from '../services/cinema.service';
import { AuthService } from '../services/auth.service';
import { RouterLink, Router } from '@angular/router'; 


@Component({
  selector: 'app-admin-add-movie',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-add-movie.component.html',
  styleUrls: ['./admin-add-movie.component.css']
})
export class AdminAddMovieComponent {
  [x: string]: any;

    

  movieData = {
    title: '',
    description: '',
    showDate: '',
    hallNumber: 1,   
    basePrice: 100  
  };

  selectedFile: File | null = null;

  loading = signal(false);
  successMessage = signal<string | null>(null);

  constructor(private cinemaService: CinemaService,
        private authService: AuthService,    private router: Router,


  ) {}

  onFileSelected(event: any) {
    this.selectedFile = event.target.files[0];
  }

  onAddMovie() {
    if (!this.selectedFile) {
      alert("يرجى اختيار صورة للفيلم");
      return;
    }

    

    this.loading.set(true);
    this.successMessage.set(null);

    const formData = new FormData();

    formData.append("Title", this.movieData.title);
    formData.append("Description", this.movieData.description);
    formData.append("ShowDate", this.movieData.showDate);
    formData.append("HallNumber", this.movieData.hallNumber.toString()); 
    formData.append("BasePrice", this.movieData.basePrice.toString());   
    formData.append("Image", this.selectedFile);                        

    this.cinemaService.addMovie(formData).subscribe({
      next: () => {
        this.successMessage.set("تم إضافة الفيلم بنجاح!");
        this.loading.set(false);
        this.resetForm();
      },
      error: (err) => {
console.log(err);
  alert("حدث خطأ أثناء الإضافة: " + (err.error || err.message));
          this.loading.set(false);
      }
    });
  }
 onLogout(): void {
    this.authService.logout();
    this.router.navigate(['/']);
  }

  resetForm() {
    this.movieData = {
      title: '',
      description: '',
      showDate: '',
      hallNumber: 1,
      basePrice: 100
    };
    this.selectedFile = null;
  }
}
