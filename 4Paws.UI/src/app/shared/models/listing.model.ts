export interface PetListing {
  id: string;
  name: string;
  breed: string;
  age: number;
  description: string;
  imageUrl?: string;
  ownerId: string;
}
