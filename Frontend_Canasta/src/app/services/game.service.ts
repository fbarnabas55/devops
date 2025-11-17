import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class GameService {

  private baseUrl =  'https://localhost:7278/api/Games';

  constructor(private http: HttpClient) {}

  createGame(teamNames: string[]): Observable<any> {
    return this.http.post(this.baseUrl, { teamNames });
  }

  getGames(): Observable<any[]> {
    return this.http.get<any[]>(this.baseUrl);
  }

  getGameDetails(id: number): Observable<any> {
    return this.http.get(`${this.baseUrl}/${id}`);
  }

  updateScore(
    gameId: number,
    roundNumber: number,
    teamId: number,
    score: number
  ): Observable<any> {
    return this.http.post(`${this.baseUrl}/update-score`, {
      gameId,
      roundNumber,
      teamId,
      score
    });
  }

}
