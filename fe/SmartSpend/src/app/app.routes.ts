import { Routes } from '@angular/router';
import { Home } from './home/home';
import { Login } from './authentication/login/login';
import { Register } from './authentication/register/register';

export const routes: Routes = [
    {
        path: '',
        component: Home
    },
    {
        path: 'login',
        component: Login
    },
    {
        path: 'register',
        component: Register
    }
];
