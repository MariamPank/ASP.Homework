import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { VerifyEmailComponent } from './features/auth/verify-email/verify-email.component';
import { ForgotPasswordComponent } from './features/auth/forgot-password/forgot-password.component';
import { ResetPasswordComponent } from './features/auth/reset-password/reset-password.component';
import { RoleSelectComponent } from './features/auth/role-select/role-select.component';
import { OwnerDashboardComponent } from './features/auth/owner-dashboard/owner-dashboard.component';
import { CaregiverDashboardComponent } from './features/auth/caregiver-dashboard/caregiver-dashboard.component';
import { ListingsComponent } from './features/listings/listings.component';
import { ProfilePageComponent } from './features/profile/profile-page.component';
import { MyPetsComponent } from './features/pets/my-pets.component';
import { MyListingsComponent } from './features/listings/my-listings.component';
import { MyApplicationsComponent } from './features/applications/my-applications.component';
import { MyAgreementsComponent } from './features/agreements/my-agreements.component';
import { LeaveReviewComponent } from './features/reviews/leave-review.component';
import { PublicProfileComponent } from './features/reviews/public-profile.component';
import { authGuard } from './guards/auth.guard';
import { AdminDashboardComponent } from './features/admin/admin-dashboard.component';

export const routes: Routes = [
  { path: '',                       redirectTo: 'login', pathMatch: 'full' },
  { path: 'login',                  component: LoginComponent },
  { path: 'register',               component: RegisterComponent },
  { path: 'verify-email',           component: VerifyEmailComponent },
  { path: 'forgot-password',        component: ForgotPasswordComponent },
  { path: 'reset-password',         component: ResetPasswordComponent },
  { path: 'profile',                component: RoleSelectComponent },
  { path: 'my-profile',             component: ProfilePageComponent,    canActivate: [authGuard] },
  { path: 'owner-dashboard',        component: OwnerDashboardComponent, canActivate: [authGuard] },
  { path: 'caregiver-dashboard',    component: CaregiverDashboardComponent, canActivate: [authGuard] },
  { path: 'listings',               component: ListingsComponent },
  { path: 'my-listings',            component: MyListingsComponent,     canActivate: [authGuard] },
  { path: 'my-pets',                component: MyPetsComponent,         canActivate: [authGuard] },
  { path: 'my-applications',        component: MyApplicationsComponent, canActivate: [authGuard] },
  { path: 'my-agreements',          component: MyAgreementsComponent,   canActivate: [authGuard] },
  { path: 'leave-review',           component: LeaveReviewComponent,    canActivate: [authGuard] },
  { path: 'public-profile/:type/:id',      component: PublicProfileComponent},
  { path: 'admin', component: AdminDashboardComponent, canActivate: [authGuard] },

];