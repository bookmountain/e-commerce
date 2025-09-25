import { Component, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-test-error',
  imports: [],
  templateUrl: './test-error.component.html',
  styleUrl: './test-error.component.scss',
})
export class TestErrorComponent {
  baseUrl = 'https://localhost:5002/api/';
  private http = inject(HttpClient);
  validationErrors?: string[];

  get404Error() {
    this.http.get(this.baseUrl + ' buggy/notfound').subscribe({
      next: (response) => console.log(response),
      error: (error) => console.log(error),
    });
  }

  get400Error() {
    this.http.get(this.baseUrl + 'buggy/badrequest').subscribe({
      next: (response) => console.log(response),
      error: (error) => console.log(error),
    });
  }

  get401Error() {
    this.http.get(this.baseUrl + 'buggy/unauthorized').subscribe({
      next: (response) => console.log(response),
      error: (error) => console.log(error),
    });
  }

  get500Error() {
    this.http.get(this.baseUrl + 'buggy/internalerror').subscribe({
      next: (response) => console.log(response),
      error: (error) => console.log(error),
    });
  }

  get400ValidationError() {
    this.http.post(this.baseUrl + 'buggy/validationerror', {}).subscribe({
      next: (response) => console.log(response),
      error: (error) => (this.validationErrors = error),
    });
  }
}
