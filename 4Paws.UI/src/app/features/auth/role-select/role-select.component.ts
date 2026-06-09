import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-role-select',
  standalone: true,
  imports: [RouterModule],
  templateUrl: './role-select.component.html',
  styleUrl: './role-select.component.scss',
})


export class RoleSelectComponent {
  constructor(public router: Router) {}  // make public

  goToOwner()     { this.router.navigate(['/owner-dashboard']); }
  goToCaregiver() { this.router.navigate(['/caregiver-dashboard']); }
  goToListings()  { this.router.navigate(['/listings']); }
  goToProfile()   { this.router.navigate(['/my-profile']); }
}