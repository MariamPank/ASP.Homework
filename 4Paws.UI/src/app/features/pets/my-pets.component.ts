import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { PetService } from '../../services/pet.service';
import { AuthService } from '../../services/auth.service';
import { Pet } from '../../models/feature.models';

@Component({
  selector: 'app-my-pets',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './my-pets.component.html',
  styleUrl: './my-pets.component.scss',
})
export class MyPetsComponent implements OnInit {
  private petService  = inject(PetService);
  private authService = inject(AuthService);
  private router      = inject(Router);
  private cdr         = inject(ChangeDetectorRef);

  pets: Pet[] = [];
  isLoading = true;
  errorMessage = '';

  // Add pet
  showAddForm = false;
  addForm = { petName: '', description: '' };
  addLoading = false;
  addError = '';

  // Edit pet
  editingPet: Pet | null = null;
  editForm = { petName: '', description: '' };
  editLoading = false;
  editError = '';

  // Delete
  deleteLoading: number | null = null;

  // Image upload
  imageLoading: number | null = null;
  imageError = '';

  readonly BASE_URL = 'http://localhost:5281';

  ngOnInit() { this.loadPets(); }

  loadPets() {
    this.petService.getMyPets().subscribe({
      next: (res) => {
        this.pets = res.value ?? [];
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.errorMessage = 'Failed to load pets.';
        this.isLoading = false;
        this.cdr.detectChanges();
      },
    });
  }

  // ── Add ────────────────────────────────────────────────────────────────

  toggleAddForm() {
    this.showAddForm = !this.showAddForm;
    this.addError = '';
    this.addForm = { petName: '', description: '' };
  }

  addPet() {
    this.addError = '';
    if (!this.addForm.petName.trim()) { this.addError = 'Pet name is required.'; return; }
    this.addLoading = true;
    this.petService.createPet(this.addForm).subscribe({
      next: () => {
        this.addLoading = false;
        this.showAddForm = false;
        this.loadPets();
      },
      error: (err) => {
        this.addLoading = false;
        this.addError = err.error?.message || 'Failed to add pet.';
        this.cdr.detectChanges();
      },
    });
  }

  // ── Edit ───────────────────────────────────────────────────────────────

  startEdit(pet: Pet) {
    this.editingPet = pet;
    this.editForm = { petName: pet.petName, description: pet.description };
    this.editError = '';
  }

  cancelEdit() { this.editingPet = null; }

  savePet() {
    if (!this.editingPet) return;
    this.editLoading = true;
    this.petService.updatePet(this.editingPet.id, this.editForm).subscribe({
      next: () => {
        this.editLoading = false;
        this.editingPet = null;
        this.loadPets();
      },
      error: (err) => {
        this.editLoading = false;
        this.editError = err.error?.message || 'Failed to update pet.';
        this.cdr.detectChanges();
      },
    });
  }

  // ── Delete ─────────────────────────────────────────────────────────────

  deletePet(pet: Pet) {
    if (!confirm(`Delete ${pet.petName}? This cannot be undone.`)) return;
    this.deleteLoading = pet.id;
    this.petService.deletePet(pet.id).subscribe({
      next: () => {
        this.deleteLoading = null;
        this.loadPets();
      },
      error: () => {
        this.deleteLoading = null;
        this.cdr.detectChanges();
      },
    });
  }

  // ── Image ──────────────────────────────────────────────────────────────

  onImageSelected(event: Event, pet: Pet) {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (!file) return;
    this.imageLoading = pet.id;
    this.imageError = '';
    this.petService.uploadImage(pet.id, file).subscribe({
      next: () => { this.imageLoading = null; this.loadPets(); },
      error: (err) => {
        this.imageLoading = null;
        this.imageError = err.error?.message || 'Upload failed.';
        this.cdr.detectChanges();
      },
    });
  }

  getImageUrl(pet: Pet): string {
    return pet.imageUrl ? `${this.BASE_URL}${pet.imageUrl}` : '';
  }

  getRatingLabel(r: number): string {
    return ['', 'Very Bad', 'Bad', 'Average', 'Good', 'Excellent'][r] ?? '—';
  }

  logout() { this.authService.logout(); this.router.navigate(['/login']); }
  goBack()  { this.router.navigate(['/owner-dashboard']); }
}