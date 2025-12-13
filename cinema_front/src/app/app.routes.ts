import { Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component'; 
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { BookingComponent } from './booking/booking.component';
import { AdminAddMovieComponent } from './admin-add-movie/admin-add-movie.component';

export const routes: Routes = [
  { path: '', component: HomeComponent }, 
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'admin', component: AdminAddMovieComponent },

   { path: 'book/:id', component: BookingComponent }, 

  { path: '**', redirectTo: '' } 
];