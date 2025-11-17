import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GameService } from '../../services/game.service';

@Component({
  selector: 'app-game-board',
  standalone: false,
  templateUrl: './game-board.component.html',
  styleUrl: './game-board.component.scss'
})
export class GameBoardComponent implements OnInit {

  gameId!: number;

  teams: any[] = []; // backendből jön
  rounds: any[] = []; // { roundNumber, scores[] }

  totals: { [teamId: number]: number } = {};
  isFinished = false;
  winningTeamName: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private gameService: GameService
  ) {}

  ngOnInit() {
    this.gameId = Number(this.route.snapshot.paramMap.get('id'));
    this.loadGame();
  }

  loadGame() {
    this.gameService.getGameDetails(this.gameId).subscribe(game => {
      this.teams = game.teams;
      this.rounds = game.rounds;
      this.isFinished = game.isFinished;
      this.winningTeamName = game.winningTeamName;
      this.calculateTotals();
    });
  }

  calculateTotals() {
    this.totals = {};
    for (let team of this.teams) {
      this.totals[team.id] = 0;
    }

    for (let round of this.rounds) {
      for (let score of round.scores) {
        this.totals[score.teamId] += score.score;
      }
    }
  }

  onScoreChange(round: any, teamId: number, value: string) {
    const score = Number(value) || 0;

    // módosítjuk a lokális adatot
    const s = round.scores.find((x: any) => x.teamId === teamId);
    s.score = score;
    this.calculateTotals();

    // automatikus mentés backend felé
    this.gameService.updateScore(this.gameId, round.roundNumber, teamId, score)
      .subscribe();
  }

  addRound() {
    const nextRound = this.rounds.length + 1;

    const newRound = {
      roundNumber: nextRound,
      scores: this.teams.map(t => ({
        teamId: t.id,
        score: 0
      }))
    };

    this.rounds.push(newRound);

    // mentés minden csapathoz (0 ponttal)
    for (let team of this.teams) {
      this.gameService.updateScore(this.gameId, nextRound, team.id, 0)
        .subscribe();
    }

    this.calculateTotals();
  }

}
