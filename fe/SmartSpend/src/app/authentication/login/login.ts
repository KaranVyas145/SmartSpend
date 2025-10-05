import { Component, inject, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  Validators,
  ReactiveFormsModule
} from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthenticationService } from '../../services/user/authentication-service';
import { UserLoginDto } from '../../../models/user-models';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, RouterModule], // ← Updated for reactive forms
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login implements OnInit {

  // Reactive Form Group - strongly typed
  loginForm!: FormGroup;

  // Inject auth service
  authenticationService = inject(AuthenticationService);
  router = inject(Router);

  // UI state properties
  showPassword = false;     // ← Controls password visibility toggle

  // Inject FormBuilder service for creating reactive forms
  constructor(private formBuilder: FormBuilder) {
    // ↑ FormBuilder makes creating forms easier than new FormGroup()
  }

  ngOnInit(): void {
    // Initialize reactive form with validation rules
    this.initForm();
  }

  initForm() {
    this.loginForm = this.formBuilder.group({
      // Email field with multiple validators
      email: [
        '',  // ← Initial value (empty string)
        [
          Validators.required,        // ← Required field
          Validators.email           // ← Must be valid email format
        ]
      ],
      // Password field with validation
      password: [
        '',  // ← Initial value
        [
          Validators.required,       // ← Required field
          Validators.minLength(6)    // ← Minimum 6 characters
        ]
      ]
    });
  }

  // Getter methods for easy access to form controls in template
  get email() {
    return this.loginForm.get('email');
    // ↑ Returns the email FormControl for validation checking
  }

  get password() {
    return this.loginForm.get('password');
    // ↑ Returns the password FormControl for validation checking
  }

  // Toggle password visibility
  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
    // ↑ Flips between true/false to show/hide password
  }

  // Handle form submission
  onLogin(): void {
    // Check if form is valid before proceeding
    if (this.loginForm.valid) {

      const formData = this.loginForm.value;

      const userLoginRequest: UserLoginDto = {
        Email: formData.email,
        Password: formData.password
      };

      this.authenticationService.login(userLoginRequest).subscribe({
        next: (response) => {
          console.log('Login successful', response);
          this.router.navigate(['/dashboard']); // Better route
        },
        error: (error) => {
          console.error('❌ Login failed:', error);
        }
      });

    }

    else {
      // Mark all fields as touched to show validation errors
      this.loginForm.markAllAsTouched();
      // ↑ This will display validation errors for invalid fields
      console.log('Form is invalid');
    }
  }

  // Helper method to check if a field has specific error
  hasError(fieldName: string, errorType: string): boolean {
    const field = this.loginForm.get(fieldName);
    return !!(field && field.hasError(errorType) && (field.dirty || field.touched));
    // ↑ Returns true if field has the specific error and user has interacted with it
  }

  // Helper method to get error message for a field
  getErrorMessage(fieldName: string): string {
    const field = this.loginForm.get(fieldName);

    if (field && field.errors && (field.dirty || field.touched)) {
      // Check for specific error types and return appropriate messages
      if (field.errors['required']) {
        return `${fieldName.charAt(0).toUpperCase() + fieldName.slice(1)} is required`;
      }
      if (field.errors['email']) {
        return 'Please enter a valid email address';
      }
      if (field.errors['minlength']) {
        const requiredLength = field.errors['minlength'].requiredLength;
        return `Password must be at least ${requiredLength} characters long`;
      }
    }
    return '';
    // ↑ Return empty string if no errors
  }
}
