import { Routes } from '@angular/router';
import { QueueTicketComponent } from './pages/queue-ticket/queue-ticket.component';
import { QueueDisplayComponent } from './pages/queue-display/queue-display.component';
import { QueueClearComponent } from './pages/queue-clear/queue-clear.component';

export const routes: Routes = [
  { path: '', component: QueueTicketComponent },
  { path: 'queue', component: QueueTicketComponent },
  { path: 'display', component: QueueDisplayComponent },
  { path: 'clear', component: QueueClearComponent }
];
