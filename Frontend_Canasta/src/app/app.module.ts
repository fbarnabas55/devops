import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomeComponent } from './pages/home/home.component';
import { NewGameComponent } from './pages/new-game/new-game.component';
import { GameBoardComponent } from './pages/game-board/game-board.component';
import { HttpClientModule, provideHttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { HeaderComponent } from './pages/header/header.component';
import { ConfigService } from './config.service';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    NewGameComponent,
    GameBoardComponent,
    HeaderComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule
  ],
  providers: [provideHttpClient()],
  bootstrap: [AppComponent]
})
export class AppModule { }
