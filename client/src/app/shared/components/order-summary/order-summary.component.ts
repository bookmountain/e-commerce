import { Component, inject } from '@angular/core';
import { CurrencyPipe } from '@angular/common';
import { CartService } from '../../../core/services/cart.service';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { RouterLink } from '@angular/router';
import { MatButton } from '@angular/material/button';
import { MatInput } from '@angular/material/input';

@Component({
  selector: 'app-order-summary',
  imports: [
    CurrencyPipe,
    MatFormField,
    MatLabel,
    RouterLink,
    MatButton,
    MatInput,
  ],
  templateUrl: './order-summary.component.html',
  styleUrl: './order-summary.component.scss',
})
export class OrderSummaryComponent {
  cartService = inject(CartService);
}
