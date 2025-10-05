import { HttpClient } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { Router } from '@angular/router';
import { User, UserLoginDto, UserLoginResponseDto, UserRegisterDto } from '../../../models/user-models';
import { environment } from '../../../environment';
import { catchError, map, Observable } from 'rxjs';
import { ApiResponse } from '../../../models/ApiResponse';

@Injectable({
  providedIn: 'root'
})



export class AuthenticationService {
  private http = inject(HttpClient);

  private _isAuthenticated = signal<boolean>(false);
  private _currentUser = signal<User | null>(null);
  private _isLoading = signal<boolean>(false);
  private _error = signal<string | null>(null);

  readonly isAuthenticated = computed(() => this._isAuthenticated());
  readonly currentUser = computed(() => this._currentUser());
  readonly isLoading = computed(() => this._isLoading());
  readonly error = computed(() => this._error());

  // 🗂️ localStorage keys
  private readonly TOKEN_KEY = 'smart_spends_token';
  private readonly USER_KEY = 'smart_spends_user';

  constructor() {
    // 🔍 Check if user was already logged in
    this.checkInitialAuthState();
  }
  private checkInitialAuthState(): void {
    try {
      const token = localStorage.getItem(this.TOKEN_KEY);
      const userJson = localStorage.getItem(this.USER_KEY);

      if (token && userJson) {
        const user = JSON.parse(userJson);

        // ✅ Restore authentication state
        this._isAuthenticated.set(true);
        this._currentUser.set(user);

        console.log('✅ User session restored:', user.email);
      }
    } catch (error) {
      console.error('❌ Error restoring session:', error);
      // this.clearAuthData();
    }
  }

  private StoreAuthData(token: string, user: UserLoginResponseDto): void {
    localStorage.setItem(this.TOKEN_KEY, token);
    localStorage.setItem(this.USER_KEY, JSON.stringify(user));
  }

  login(credentials: UserLoginDto): Observable<any> {
    this._isLoading.set(true);
    this._error.set(null);
    return this.http.post<any>(environment.apiUrl + "/api/Auth/login", credentials)
      .pipe(
        map((response: ApiResponse<UserLoginResponseDto>) => {
          if (response.statusCode === 200) {
            if (response.data) {
              this.StoreAuthData(response.data.Token, response.data);
              this._isAuthenticated.set(true);
              this._currentUser.set(response.data);
              this._isLoading.set(false);
              return response.data
            }
            else {
              throw new Error('Login failed');
            }
          }
          else {
            // Handle API success: false
            const errorMsg = response.message || 'Login failed';
            console.error('❌ Login failed:', errorMsg);
            throw new Error(errorMsg);
          }
        }),
        catchError((error) => {
          console.error('❌ Login failed:', error);
          this._isLoading.set(false);
          this._error.set(error.message);
          throw error;
        })

      );
  }

  register(userDetails: UserRegisterDto): Observable<any> {
    this._isLoading.set(true);
    this._error.set(null);

    return this.http.post<any>(environment.apiUrl+ '/api/Auth/register', userDetails).pipe(
      map((response: ApiResponse<any>) => {
        if (response.statusCode === 200) {
          this._isLoading.set(false);
          return response.data;
        } else {
          // Handle API success: false
          const errorMsg = response.message || 'Registration failed';
          console.error('❌ Registration failed:', errorMsg);
          throw new Error(errorMsg);
        }
      }),
      catchError((error) => {
        console.error('❌ Registration failed:', error);
        this._isLoading.set(false);
        this._error.set(error.message);
        throw error;
      })
    )
  }

}
