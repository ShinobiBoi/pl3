import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { RegisterRequest } from '../models/core.models';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css' 
})
export class RegisterComponent implements OnInit {
  
  registerData: RegisterRequest = {
    username: '',
    password: '',
    email: ''
  };

  loading = signal(false);
  errorMessage = signal('');
  successMessage = signal('');
  
  confirmPassword = ''; 

  constructor(
    private authService: AuthService,
    private router: Router 
  ) {}
  
  ngOnInit(): void {
    if (this.authService.currentUser()) {
      this.router.navigate(['/']); 
    }
  }

  onRegister(): void {
    this.errorMessage.set('');
    this.successMessage.set('');
    this.loading.set(true);

    if (this.registerData.password !== this.confirmPassword) {
      this.errorMessage.set('كلمة المرور وتأكيد كلمة المرور غير متطابقين.');
      this.loading.set(false);
      return;
    }
    
    if (!this.registerData.username || !this.registerData.password || !this.registerData.email) {
      this.errorMessage.set('الرجاء إدخال اسم المستخدم وكلمة المرور و البريد.');
      this.loading.set(false);
      return;
    }

    this.authService.register(this.registerData).subscribe({
      next: () => {
        this.successMessage.set('تم التسجيل بنجاح! سيتم توجيهك لصفحة تسجيل الدخول.');
    
        this.router.navigate(['/login']);
      },
      error: (err) => {
        
      this.loading.set(false);        
   
      if (err.status === 409) {
        this.errorMessage.set('عذراً، اسم المستخدم موجود بالفعل. اختر اسماً آخر.');
      } else {
        this.errorMessage.set('حدث خطأ أثناء التسجيل. حاول مرة أخرى لاحقاً.');
      }
      console.error('Registration Error:', err);       
       
      },
      complete: () => {
        this.loading.set(false);
      }
    });
  }
}