import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login/login.component';
import { ProfileComponent } from './features/auth/profile/profile.component';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'profile', component: ProfileComponent },
  // { path: 'register', component: RegisterComponent }, // add when ready
];