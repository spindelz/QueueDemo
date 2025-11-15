import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-queue-clear',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './queue-clear.component.html',
  styleUrls: ['./queue-clear.component.css']
})
export class QueueClearComponent implements OnInit {
  currentQueueNumber: string = 'Q00';
  loading: boolean = false;
  error: string | null = null;
  success: string | null = null;

  constructor(private http: HttpClient, private router: Router) {}

  ngOnInit(): void {
    const tempQ = sessionStorage.getItem('tempQ');
    if (tempQ) {
      this.currentQueueNumber = tempQ;
    }
  }

  clearQueue(): void {
    this.loading = true;
    this.error = null;
    this.success = null;

    const apiUrl = `${environment.apiUrl}/queue/clear`;

    this.http.post(apiUrl, {}).subscribe({
      next: () => {
        this.success = 'ล้างคิวสำเร็จ';
        this.currentQueueNumber = '00';
        sessionStorage.removeItem('tempQ');
        this.loading = false;
      },
      error: (err) => {
        console.error('Error clearing queue:', err);
        this.error = 'เกิดข้อผิดพลาดในการล้างคิว';
        this.loading = false;
      }
    });
  }

  goBackToTicketPage(): void {
    this.router.navigate(['/']);
  }
}
