import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ApiService } from './services/api.service';

@Component({
  selector: 'app-root',
  imports: [FormsModule],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {
  private api = inject(ApiService);
  private cdr = inject(ChangeDetectorRef);

  // Document state
  documentId: string | null = null;
  documentName: string = '';
  pageCount: number = 0;
  chunkCount: number = 0;
  uploading: boolean = false;

  // Question state
  question: string = '';
  asking: boolean = false;

  // Result state
  answer: string = '';
  sourceChunks: number[] = [];
  error: string = '';

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];

    if (!file) return;

    this.uploading = true;
    this.error = '';

    this.api.uploadDocument(file).subscribe({
      next: (response) => {
        this.documentId = response.id;
        this.documentName = response.name;
        this.pageCount = response.pageCount;
        this.chunkCount = response.chunkCount;
        this.uploading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.error = 'Failed to upload document. Is the API running?';
        this.uploading = false;
        this.cdr.detectChanges();
      },
    });
  }

  askQuestion(): void {
    if (!this.documentId || !this.question.trim()) return;

    this.asking = true;
    this.answer = '';
    this.sourceChunks = [];
    this.error = '';

    this.api.askQuestion(this.documentId, this.question).subscribe({
      next: (response) => {
        this.answer = response.answer;
        this.sourceChunks = response.sourceChunks.map((c: any) => c.chunkNumber);
        this.asking = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.error = 'Failed to get answer. Is the API running?';
        this.asking = false;
        this.cdr.detectChanges();
      },
    });
  }

  reset(): void {
    this.documentId = null;
    this.documentName = '';
    this.pageCount = 0;
    this.chunkCount = 0;
    this.question = '';
    this.answer = '';
    this.sourceChunks = [];
    this.error = '';
    this.cdr.detectChanges();
  }
}
