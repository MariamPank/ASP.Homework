import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login/login.component';
import { RoleSelectComponent } from './features/auth/role-select/role-select.component';
import { OwnerDashboardComponent } from './features/auth/owner-dashboard/owner-dashboard.component';
import { CaregiverDashboardComponent } from './features/auth/caregiver-dashboard/caregiver-dashboard.component';
 
export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'profile', component: RoleSelectComponent },
  { path: 'owner-dashboard', component: OwnerDashboardComponent },
  { path: 'caregiver-dashboard', component: CaregiverDashboardComponent },
];