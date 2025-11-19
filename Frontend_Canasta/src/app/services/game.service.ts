import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface TeamDto {
  id: number;
  name: string;
  orderIndex: number;
}

export interface RoundScoreDto {
  teamId: number;
  score: number;
}

export interface RoundDto {
  roundNumber: number;
  scores: RoundScoreDto[];
}

export interface GameDetailsDto {
  id: number;
  isFinished: boolean;
  winningTeamId?: number | null;
  winningTeamName?: string | null;
  teams: TeamDto[];
  rounds: RoundDto[];
}

@Injectable({
  providedIn: 'root'
})
export class GameService {

  private baseUrl =  'https://localhost:7278/api/Games';

  constructor(private http: HttpClient) {}
  

   createGame(teamNames: string[]): Observable<GameDetailsDto> {
    return this.http.post<GameDetailsDto>(this.baseUrl, { teamNames });
  }

  getGames(): Observable<any[]> {
    return this.http.get<any[]>(this.baseUrl);
  }

  getGame(id: number): Observable<GameDetailsDto> {
    return this.http.get<GameDetailsDto>(`${this.baseUrl}/${id}`);
  }

  getRounds(gameId: number): Observable<RoundDto[]> {
    return this.http.get<RoundDto[]>(`${this.baseUrl}/${gameId}/Rounds`);
  }

  getGameDetails(id: number): Observable<any> {
    return this.http.get(`${this.baseUrl}/${id}`);
  }

  updateScore(
    gameId: number,
    roundNumber: number,
    teamId: number,
    score: number
  ): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/update-score`, {
      gameId,
      roundNumber,
      teamId,
      score
    });
  }

  addRound(gameId: number, roundNumber: number) {
    return this.http.post(`${this.baseUrl}/${gameId}/add-round`, roundNumber);
  }


  createRound(gameId: number): Observable<RoundDto> {
    return this.http.post<RoundDto>(`${this.baseUrl}/Rounds`, { gameId });
  }

  addScore(roundId: number, teamId: number, score: number): Observable<any> {
    return this.http.post(`${this.baseUrl}/RoundScores`, {
      roundId,
      teamId,
      score
    });
  }


  saveRound(gameId: number, roundNumber: number, scores: { teamId: number; score: number }[]) {
  return this.http.post(`${this.baseUrl}/save-round`, {
    gameId,
    roundNumber,
    scores
  });
}


}
