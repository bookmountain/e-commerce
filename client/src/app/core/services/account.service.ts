import { computed, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  currentUser = computed(() => null);
}
