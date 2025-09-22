import { computed, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class CartService {
  itemCount = computed(() => 0);
}
