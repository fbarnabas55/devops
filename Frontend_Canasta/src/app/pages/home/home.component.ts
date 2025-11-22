import { Component, OnInit } from '@angular/core';
import { GameService } from '../../services/game.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  standalone: false,
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit {

  games: any[] = [];

  constructor(private gameService: GameService, private router: Router) {}

  ngOnInit() {
    this.loadGames();
  }

  loadGames() {
    this.gameService.getGames().subscribe(data => {
      this.games = data;
    });
  }

  deleteGame(id: number) {
    if (!confirm('Biztosan törlöd ezt a játékot?')) return;

    this.gameService.deleteGame(id).subscribe({
      next: () => {
        this.games = this.games.filter(g => g.id !== id);
      },
      error: err => console.error('Törlés hiba:', err)
    }); 
  }


  newGame() {
    this.router.navigate(['/new-game']);
  }

  openGame(id: number) {
    this.router.navigate(['/game', id]);
  }
}
