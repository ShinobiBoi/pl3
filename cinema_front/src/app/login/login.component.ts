import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink , ActivatedRoute } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { LoginRequest} from '../models/core.models';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent implements OnInit {
  
  loginData: LoginRequest = {
    username: '',
    password: ''
  };

  loading = signal(false);
  errorMessage = signal('');
  returnUrl: string = '/';
registerData: any;

  constructor(
    private authService: AuthService,
    private router: Router ,
    private route: ActivatedRoute
  ) {}
  
  // ngOnInit(): void {
  //   if (this.authService.currentUser()) {
  //     this.returnUrl = this.route.snapshot.queryParams['returnTo'] || '/';
      
  //   if (this.authService.currentUser()) {
  //     this.router.navigateByUrl(this.returnUrl); 
  //   }
  //   }
  // }

  ngOnInit(): void {
  const user = this.authService.currentUser();
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';

  if (user) {
    
    this.router.navigateByUrl(this.returnUrl);
  }
}

  onLogin(): void {
    this.errorMessage.set('');
    this.loading.set(true);

    if (!this.loginData.username || !this.loginData.password) {
      this.errorMessage.set('الرجاء إدخال اسم المستخدم وكلمة المرور.');
      this.loading.set(false);
      return;
    }

    this.authService.login(this.loginData).subscribe({
      next: (user) => {
        // if (user.role === 'Admin') {
        //   this.router.navigate(['/admin']); 
        // } else {
       
        //   this.router.navigate(['/']); 
        // }
              let redirectUrl: string;

            if (user.role === 'Admin') {
                redirectUrl = '/admin'; 
            } 
            else if (this.returnUrl !== '/') {
                redirectUrl = this.returnUrl; 
            } 
            else {
                redirectUrl = '/'; 
            }
    this.router.navigateByUrl(redirectUrl);

      },
      error: (err) => {
        console.error('Login failed:', err);
        this.errorMessage.set('فشل تسجيل الدخول. يرجى التحقق من البيانات.');
        this.loading.set(false);
      },
      complete: () => {
        this.loading.set(false);
      }
    });
  }
}