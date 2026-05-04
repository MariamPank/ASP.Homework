import { Component } from '@angular/core';
import { RouterModule } from "@angular/router";


@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterModule],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
// This name MUST match the name in main.ts exactly
export class AppComponent {
  title = '4Paws';
}
