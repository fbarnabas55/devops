import { Component } from '@angular/core';
import { GameService } from '../../services/game.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-new-game',
  standalone: false,
  templateUrl: './new-game.component.html',
  styleUrl: './new-game.component.scss'
})
export class NewGameComponent {

  // Alapból 2 csapat
  teamNames = [
    { name: '' },
    { name: '' }
  ];

  errorMessage: string | null = null;

  constructor(
    private gameService: GameService,
    private router: Router
  ) {}

  get canAddTeam(): boolean {
    return this.teamNames.length < 4;
  }

  get canStartGame(): boolean {
    const nonEmpty = this.teamNames
      .map(t => t.name.trim())
      .filter(n => n.length > 0);

    return nonEmpty.length >= 2;
  }

  addTeam(): void {
    this.errorMessage = null;
    if (this.teamNames.length < 4) {
      this.teamNames.push({ name: '' });
    }
  }

  removeTeam(index: number): void {
    this.errorMessage = null;
    if (this.teamNames.length > 2) {
      this.teamNames.splice(index, 1);
    }
  }

  createGame(): void {
    this.errorMessage = null;

    const trimmed = this.teamNames.map(t => t.name.trim());
    const nonEmpty = trimmed.filter(n => n.length > 0);

    if (nonEmpty.length < 2) {
      this.errorMessage = 'Legalább 2 nem üres csapatnevet adj meg.';
      return;
    }

    if (nonEmpty.length > 4) {
      this.errorMessage = 'Legfeljebb 4 csapat lehet egy játékban.';
      return;
    }

    // duplikátum ellenőrzés
    const lower = nonEmpty.map(n => n.toLowerCase());
    const hasDuplicate = lower.some((n, idx) => lower.indexOf(n) !== idx);

    if (hasDuplicate) {
      this.errorMessage = 'A csapatnevek nem lehetnek duplikáltak.';
      return;
    }

    // Backend hívás
    this.gameService.createGame(nonEmpty).subscribe({
      next: game => this.router.navigate(['/game', game.id]),
      error: err => {
        console.error(err);
        this.errorMessage = 'Hiba történt a játék létrehozása közben.';
      }
    });
  }

}
