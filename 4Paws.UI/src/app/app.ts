import { Component } from '@angular/core';
import { PetListComponent } from './features/listings/pet-list/pet-list.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [PetListComponent],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
// This name MUST match the name in main.ts exactly
export class AppComponent {
  title = '4Paws';
}
