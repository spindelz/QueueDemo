import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { environment } from '../../../environments/environment';

interface QueueReceiveResponse {
  queueNumber: string;
}

@Component({
  selector: 'app-queue-ticket',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './queue-ticket.component.html',
  styleUrls: ['./queue-ticket.component.css']
})
export class QueueTicketComponent {
  loading: boolean = false;
  errorMessage: string | null = null;

  constructor(private router: Router, private http: HttpClient) {}

  getQueue(): void {
    this.loading = true;
    this.errorMessage = null;

    const apiUrl = `${environment.apiUrl}/queue/receive`;

    this.http.post<QueueReceiveResponse>(apiUrl, {}).subscribe({
      next: (response) => {
        
        sessionStorage.setItem('tempQ', response.queueNumber);

        this.router.navigate(['/display']);
        this.loading = false;
      },
      error: (error: HttpErrorResponse) => {
        this.loading = false;

        if (error.status === 0) {
          this.errorMessage = 'ไม่สามารถเชื่อมต่อกับเซิร์ฟเวอร์ได้ กรุณาตรวจสอบการเชื่อมต่อ';
        } else if (error.status >= 400 && error.status < 500) {
          this.errorMessage = 'เกิดข้อผิดพลาดในการรับบัตรคิว กรุณาลองใหม่อีกครั้ง';
        } else if (error.status >= 500) {
          this.errorMessage = 'เซิร์ฟเวอร์มีปัญหา กรุณาลองใหม่ภายหลัง';
        } else {
          this.errorMessage = 'เกิดข้อผิดพลาด กรุณาลองใหม่อีกครั้ง';
        }

        console.error('Error receiving queue:', error);
      }
    });
  }

  clearQueue(): void {
    this.errorMessage = null;
    this.router.navigate(['/clear']);
  }
}
