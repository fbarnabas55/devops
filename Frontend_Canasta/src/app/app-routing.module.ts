import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home.component';
import { GameBoardComponent } from './pages/game-board/game-board.component';
import { NewGameComponent } from './pages/new-game/new-game.component';

const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'new-game', component: NewGameComponent },
  { path: 'game/:id', component: GameBoardComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
