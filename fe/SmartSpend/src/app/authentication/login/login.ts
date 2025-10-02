import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';


@Component({
  selector: 'app-login',
  imports: [FormsModule, RouterModule],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login implements OnInit{

    // Form data properties
  loginData = {
    email: '',       // ← Stores user email input
    password: ''     // ← Stores user password input
  };

    // UI state properties
  showPassword = false;     // ← Controls password visibility toggle
  isLoading = false;       // ← Shows loading state during login
  ngOnInit(): void {
    
  }

    // Toggle password visibility
  togglePasswordVisibility() {
    this.showPassword = !this.showPassword;
    // ↑ Flips between true/false to show/hide password
  }


    onLogin() {
    this.isLoading = true;   // ← Start loading state
    
    console.log('Login attempt:', this.loginData);
    // ↑ Debug: see what user entered
    
    // TODO: Replace with actual authentication service
    setTimeout(() => {
      this.isLoading = false;  // ← Stop loading after "API call"
      // Navigate to dashboard or show error
    }, 2000);
  }

}
