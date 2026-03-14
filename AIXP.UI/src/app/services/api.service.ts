import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private http = inject(HttpClient);
  private baseUrl = 'http://localhost:5215';

  uploadDocument(file: File): Observable<any> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post(`${this.baseUrl}/document`, formData);
  }

  askQuestion(documentId: string, question: string): Observable<any> {
    return this.http.post(`${this.baseUrl}/document/${documentId}/ask`, {
      question,
    });
  }
}
