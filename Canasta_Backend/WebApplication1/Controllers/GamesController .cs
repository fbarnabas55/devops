using Data;
using Entities.Dtos;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Canasta.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class GamesController : ControllerBase
    {
        private readonly KanastaDbContext _context;

        public GamesController(KanastaDbContext context)
        {
            _context = context;
        }

        // ------------------------------------------------------------
        // 1) Játék létrehozása
        // ------------------------------------------------------------
        [HttpPost]
        public async Task<ActionResult<GameDetailsDto>> CreateGame(CreateGameRequest request)
        {
            if (request.TeamNames.Count < 2 || request.TeamNames.Count > 4)
                return BadRequest("2–4 csapat szükséges.");

            var game = new Game();

            int index = 0;
            foreach (var name in request.TeamNames)
            {
                game.Teams.Add(new Team
                {
                    Name = name.Trim(),
                    OrderIndex = index++,
                    Game = game
                });
            }

            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            return await GetGameDetails(game.Id);
        }

        // ------------------------------------------------------------
        // 2) Összes játék listázása (home page)
        // ------------------------------------------------------------
        [HttpGet]
        public async Task<ActionResult<List<GameListItemDto>>> GetGames()
        {
            var games = await _context.Games
                .Include(g => g.Teams)
                .ToListAsync();

            var list = games.Select(g => new GameListItemDto
            {
                Id = g.Id,
                TeamNames = g.Teams.OrderBy(t => t.OrderIndex).Select(t => t.Name).ToList(),
                IsFinished = g.IsFinished,
                StatusText = g.IsFinished
                    ? $"Nyertes: {g.WinningTeam?.Name}"
                    : "A játék még tart"
            })
            .ToList();

            return list;
        }

        // ------------------------------------------------------------
        // 3) Játék részletes adatainak lekérése
        // ------------------------------------------------------------
        [HttpGet("{id}")]
        public async Task<ActionResult<GameDetailsDto>> GetGameDetails(int id)
        {
            var game = await _context.Games
                .Include(g => g.Teams)
                .Include(g => g.Rounds)
                    .ThenInclude(r => r.Scores)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game == null)
                return NotFound();

            var dto = new GameDetailsDto
            {
                Id = game.Id,
                IsFinished = game.IsFinished,
                WinningTeamId = game.WinningTeamId,
                WinningTeamName = game.WinningTeam?.Name,

                Teams = game.Teams
                    .OrderBy(t => t.OrderIndex)
                    .Select(t => new TeamDto
                    {
                        Id = t.Id,
                        Name = t.Name,
                        OrderIndex = t.OrderIndex
                    }).ToList(),

                Rounds = game.Rounds
                    .OrderBy(r => r.RoundNumber)
                    .Select(r => new RoundDto
                    {
                        RoundNumber = r.RoundNumber,
                        Scores = r.Scores.Select(s => new RoundScoreDto
                        {
                            TeamId = s.TeamId,
                            Score = s.Score
                        }).ToList()
                    })
                    .ToList()
            };

            return dto;
        }

        // ------------------------------------------------------------
        // 4) Pont frissítés (kör + csapat pont)
        // ------------------------------------------------------------
        [HttpPost("update-score")]
        public async Task<ActionResult> UpdateScore(UpdateScoreRequest request)
        {
            var game = await _context.Games
                .Include(g => g.Teams)
                .Include(g => g.Rounds)
                    .ThenInclude(r => r.Scores)
                .FirstOrDefaultAsync(g => g.Id == request.GameId);

            if (game == null)
                return NotFound("Game not found.");

            var round = game.Rounds
                .FirstOrDefault(r => r.RoundNumber == request.RoundNumber);

            if (round == null)
            {
                round = new Round
                {
                    GameId = game.Id,
                    Game = game,
                    RoundNumber = request.RoundNumber
                };
                _context.Rounds.Add(round);
            }

            var score = round.Scores
                .FirstOrDefault(s => s.TeamId == request.TeamId);

            if (score == null)
            {
                score = new RoundScore
                {
                    Round = round,
                    RoundId = round.Id,
                    TeamId = request.TeamId,
                    Score = request.Score
                };
                _context.RoundScores.Add(score);
            }
            else
            {
                score.Score = request.Score;
            }

            // Játék vége logika
            var totals = game.Teams.ToDictionary(t => t.Id, t => 0);

            foreach (var r in game.Rounds)
                foreach (var s in r.Scores)
                    totals[s.TeamId] += s.Score;

            var winner = totals.FirstOrDefault(x => x.Value >= 10000);

            if (winner.Value >= 10000)
            {
                game.IsFinished = true;
                game.WinningTeamId = winner.Key;
            }

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}