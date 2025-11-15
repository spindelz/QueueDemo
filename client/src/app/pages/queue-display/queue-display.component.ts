import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

interface QueueDetailResponse {
  id: number;
  number: string;
  qDate: string;
}

interface QueueDetail {
  queueNumber: string;
  date: string;
  time: string;
}

@Component({
  selector: 'app-queue-display',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './queue-display.component.html',
  styleUrls: ['./queue-display.component.css']
})
export class QueueDisplayComponent implements OnInit {
  queueDetailRes: QueueDetailResponse | null = null;
  queueDetail: QueueDetail | null = null;
  loading: boolean = true;
  error: string | null = null;

  constructor(private http: HttpClient, private router: Router) {}

  ngOnInit(): void {
    const tempQ = sessionStorage.getItem('tempQ');

    if (!tempQ) {
      this.error = 'ไม่พบหมายเลขคิว';
      this.loading = false;
      return;
    }

    this.fetchQueueDetail(tempQ);
  }

  fetchQueueDetail(qNum: string): void {
    const apiUrl = `${environment.apiUrl}/queue/detail?qNum=${qNum}`;

    this.http.get<QueueDetailResponse>(apiUrl).subscribe({
      next: (data) => {
        this.queueDetail = {
          queueNumber: data.number,
          date: data.qDate.split(' ')[0],
          time: data.qDate.split(' ')[1]
        };
        
        this.loading = false;
      },
      error: (err) => {
        console.error('Error fetching queue detail:', err);
        this.error = 'เกิดข้อผิดพลาดในการดึงข้อมูลคิว';
        this.loading = false;
      }
    });
  }

  goBackToTicketPage(): void {
    this.router.navigate(['/']);
  }

  goToClearPage(): void {
    this.router.navigate(['/clear']);
  }
}
