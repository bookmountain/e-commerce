import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Pagination } from '../../shared/models/pagination';
import { Product } from '../../shared/models/products';
import { ShopParams } from '../../shared/models/shopParams';

@Injectable({
  providedIn: 'root',
})
export class ShopService {
  baseUrl = 'https://localhost:5002/api';
  private http = inject(HttpClient);
  types: string[] = [];
  brands: string[] = [];

  getProducts(shopParams: ShopParams) {
    const { brands, types, sort, search, pageSize, pageNumber } = shopParams;
    let params = new HttpParams();
    if (brands && brands.length > 0) {
      params = params.append('brands', brands.join(','));
    }
    if (types && types.length > 0) {
      params = params.append('types', types.join(','));
    }

    if (sort) {
      params = params.append('sort', sort);
    }

    if (search) {
      params = params.append('search', search);
    }

    params = params.append('pageSize', pageSize);
    params = params.append('pageIndex', pageNumber);

    return this.http.get<Pagination<Product>>(this.baseUrl + '/products', {
      params,
    });
  }

  getProduct(id: number) {
    return this.http.get<Product>(this.baseUrl + '/products/' + id);
  }

  getBrands() {
    if (this.brands && this.brands.length > 0) {
      return;
    }
    return this.http
      .get<string[]>(this.baseUrl + '/products/brands')
      .subscribe({
        next: (response) => (this.brands = response),
        error: (error) => console.log(error),
      });
  }

  getTypes() {
    if (this.types && this.types.length > 0) {
      return;
    }
    return this.http.get<string[]>(this.baseUrl + '/products/types').subscribe({
      next: (response) => (this.types = response),
      error: (error) => console.log(error),
    });
  }
}
