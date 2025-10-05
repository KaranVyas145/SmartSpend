import { Component, inject, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  Validators,
  ReactiveFormsModule
} from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthenticationService } from '../../services/user/authentication-service';
import { UserLoginDto, UserRegisterDto } from '../../../models/user-models';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, RouterModule],
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class Register implements OnInit {

  // Reactive Form Group - strongly typed
  registerForm!: FormGroup;

  // Inject auth service
  authenticationService = inject(AuthenticationService);
  router = inject(Router);

  // UI state properties
  showPassword = false;     // ← Controls password visibility toggle
  showConfirmPassword = false; // ← Controls confirm password visibility toggles

  // Inject FormBuilder service for creating reactive forms
  constructor(private formBuilder: FormBuilder) {
    // ↑ FormBuilder makes creating forms easier than new FormGroup()
  }

  ngOnInit(): void {
    // Initialize reactive form with validation rules
    this.initForm();
  }

  initForm() {
    this.registerForm = this.formBuilder.group({
      // Username field with validation
      username: [
        '',  // ← Initial value
        [
          Validators.required,
          Validators.minLength(3)
        ]
      ],
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
      ],

      // Confirm Password field with validation
      confirmPassword: [
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
    return this.registerForm.get('email');
    // ↑ Returns the email FormControl for validation checking
  }

  get password() {
    return this.registerForm.get('password');
    // ↑ Returns the password FormControl for validation checking
  }

  get confirmPassword() {
    return this.registerForm.get('confirmPassword');
    // ↑ Returns the confirmPassword FormControl for validation checking
  }

  get username() {
    return this.registerForm.get('username');
    // ↑ Returns the username FormControl for validation checking
  }

  // Toggle password visibility
  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
    // ↑ Flips between true/false to show/hide password
  }

  // Toggle confirm password visibility
  toggleConfirmPasswordVisibility(): void {
    this.showConfirmPassword = !this.showConfirmPassword;
    // ↑ Flips between true/false to show/hide password
  }

  // Check password match
  checkPasswordMatch(): boolean {
    return this.registerForm.get('password')?.value === this.registerForm.get('confirmPassword')?.value;
    // ↑ Returns true if passwords match, false otherwise
  }

  // // Handle form submission
  // onLogin(): void {
  //   // Check if form is valid before proceeding
  //   if (this.registerForm.valid) {

  //     const formData = this.registerForm.value;

  //     const userLoginRequest: UserLoginDto = {
  //       Email: formData.email,
  //       Password: formData.password
  //     };

  //     this.authenticationService.login(userLoginRequest).subscribe({
  //       next: (response) => {
  //         console.log('Login successful', response);
  //         this.router.navigate(['/dashboard']); // Better route
  //       },
  //       error: (error) => {
  //         console.error('❌ Login failed:', error);
  //       }
  //     });

  //   }

  //   else {
  //     // Mark all fields as touched to show validation errors
  //     this.registerForm.markAllAsTouched();
  //     // ↑ This will display validation errors for invalid fields
  //     console.log('Form is invalid');
  //   }
  // }


  onRegister(): void {
    // Check if form is valid before proceeding
    if (this.registerForm.valid) {

      const formData = this.registerForm.value;

      const UserRegisterDto: UserRegisterDto = {
        Username: formData.username,
        Email: formData.email,
        Password: formData.password,
        ConfirmPassword: formData.confirmPassword
      };

      this.authenticationService.register(UserRegisterDto).subscribe({
        next: (response) => {
          console.log('Registration successful', response);
          this.router.navigate(['/login']); 
        },
        error: (error) => {
          console.error('❌ Registration failed:', error);
        }
      })

    };
  }

  // Helper method to check if a field has specific error
  hasError(fieldName: string, errorType: string): boolean {
    const field = this.registerForm.get(fieldName);
    return !!(field && field.hasError(errorType) && (field.dirty || field.touched));
    // ↑ Returns true if field has the specific error and user has interacted with it
  }

  // Helper method to get error message for a field
  getErrorMessage(fieldName: string): string {
    const field = this.registerForm.get(fieldName);

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
      if (this.checkPasswordMatch()) {
        return 'Passwords do not match';
      }
    }
    return '';
    // ↑ Return empty string if no errors
  }
}