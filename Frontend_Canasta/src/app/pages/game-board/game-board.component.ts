import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GameDetailsDto, GameService, RoundDto, TeamDto } from '../../services/game.service';
import { forkJoin } from 'rxjs';


@Component({
  selector: 'app-game-board',
  standalone: false,
  templateUrl: './game-board.component.html',
  styleUrl: './game-board.component.scss'
})
export class GameBoardComponent implements OnInit {

  gameId!: number;

  game: GameDetailsDto | null = null;
  teams: TeamDto[] = [];
  rounds: RoundDto[] = [];

  totals: { [teamId: number]: number } = {};

  newRoundScores: { [teamId: number]: number | null } = {};

  winner: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private gameService: GameService
  ) {}

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    this.gameId = Number(idParam);

    if (isNaN(this.gameId)) {
      console.error('Érvénytelen gameId a route-ban');
      return;
    }

    this.loadGame();
  }

  loadGame(): void {
    this.gameService.getGame(this.gameId).subscribe({
      next: (game) => {
        this.game = game;
        this.teams = game.teams;
        this.rounds = game.rounds ?? [];
        this.winner = game.winningTeamName ?? null;

        this.recalcTotals();
        this.resetNewRoundScores();
      },
      error: (err) => {
        console.error('Hiba a játék betöltésekor:', err);
      }
    });
  }

  private recalcTotals(): void {
    const totals: { [id: number]: number } = {};

    for (const t of this.teams) {
      totals[t.id] = 0;
    }

    for (const r of this.rounds) {
      for (const s of (r.scores || [])) {
        if (totals[s.teamId] === undefined) {
          totals[s.teamId] = 0;
        }
        totals[s.teamId] += s.score;
      }
    }

    this.totals = totals;
  }

  private resetNewRoundScores(): void {
    this.newRoundScores = {};
    for (const t of this.teams) {
      this.newRoundScores[t.id] = null;
    }
  }

  getScoreFor(round: RoundDto, team: TeamDto): number | null {
    const score = (round.scores || []).find(s => s.teamId === team.id);
    return score ? score.score : null;
  }

  

  addRound() {
  if (!this.teams.length) return;
  if (Object.values(this.newRoundScores).some(v => v == null || v === undefined)) {
    alert('Minden csapat pontját add meg, mielőtt mented a kört!');
    return;
  }

  const existingRoundNumbers = this.rounds.map(r => r.roundNumber);
  const nextRoundNumber = existingRoundNumbers.length
    ? Math.max(...existingRoundNumbers) + 1
    : 1;

  const scores = this.teams.map(t => ({
    teamId: t.id,
    score: Number(this.newRoundScores[t.id])
  }));

  this.gameService.saveRound(this.gameId, nextRoundNumber, scores)
    .subscribe({
      next: () => {
        this.loadGame();      
      },
      error: err => console.error(err)
    });
  }
}
