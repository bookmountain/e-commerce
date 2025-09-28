import { nanoid } from 'nanoid';

export type TCart = {
  id: string;
  items: CartItem[];
};

export type CartItem = {
  productId: number;
  productName: string;
  price: number;
  quantity: number;
  pictureUrl: string;
  brand: string;
  type: string;
};

export class Cart implements TCart {
  id = nanoid();
  items: CartItem[] = [];
}
