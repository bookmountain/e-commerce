import { Component, inject, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { Order } from '../../../shared/models/order';
import { OrderService } from '../../../core/services/order.service';

@Component({
  selector: 'app-order.ts',
  standalone: true,
  imports: [RouterLink, DatePipe, CurrencyPipe],
  templateUrl: './order.component.html',
  styleUrl: './order.component.scss',
})
export class OrderComponent implements OnInit {
  private orderService = inject(OrderService);
  orders: Order[] = [];

  ngOnInit(): void {
    this.orderService.getOrdersForUser().subscribe({
      next: (orders) => (this.orders = orders),
    });
  }
}
