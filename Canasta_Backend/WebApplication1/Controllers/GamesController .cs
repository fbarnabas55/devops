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

        // POST: api/games
        [HttpPost]
        public async Task<ActionResult<GameDetailsDto>> CreateGame(CreateGameRequest request)
        {
            if (request.TeamNames == null || request.TeamNames.Count < 2 || request.TeamNames.Count > 4)
            {
                return BadRequest("Legalább 2 és legfeljebb 4 csapatot lehet megadni.");
            }

            var game = new Game();

            int index = 0;
            foreach (var name in request.TeamNames)
            {
                if (string.IsNullOrWhiteSpace(name))
                    return BadRequest("A csapatnevek nem lehetnek üresek.");

                game.Teams.Add(new Team
                {
                    Name = name.Trim(),
                    OrderIndex = index++
                });
            }

            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            return await GetGameDetails(game.Id);
        }

        // GET: api/games
        [HttpGet]
        public async Task<ActionResult<List<GameListItemDto>>> GetGames()
        {
            var games = await _context.Games
                .Include(g => g.Teams)
                .Include(g => g.Rounds)
                    .ThenInclude(r => r.Scores)
                .OrderByDescending(g => g.CreatedAt)
                .ToListAsync();

            var result = games.Select(g =>
            {
                var teamNames = g.Teams
                    .OrderBy(t => t.OrderIndex)
                    .Select(t => t.Name)
                    .ToList();

                // Összpontszám számolása
                var scoresByTeam = g.Teams.ToDictionary(t => t.Id, t => 0);
                foreach (var round in g.Rounds)
                {
                    foreach (var score in round.Scores)
                    {
                        if (scoresByTeam.ContainsKey(score.TeamId))
                        {
                            scoresByTeam[score.TeamId] += score.Score;
                        }
                    }
                }

                var maxScore = scoresByTeam.Count > 0 ? scoresByTeam.Values.Max() : 0;
                bool isFinished = maxScore >= 10000;

                string statusText = isFinished
                    ? "A játék véget ért"
                    : "Még tart a játék";

                return new GameListItemDto
                {
                    Id = g.Id,
                    TeamNames = teamNames,
                    IsFinished = isFinished,
                    StatusText = statusText
                };
            }).ToList();

            return result;
        }

        // GET: api/games/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GameDetailsDto>> GetGameDetails(int id)
        {
            var game = await _context.Games
                .Include(g => g.Teams)
                .Include(g => g.Rounds)
                    .ThenInclude(r => r.Scores)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game == null)
            {
                return NotFound();
            }

            var teams = game.Teams
                .OrderBy(t => t.OrderIndex)
                .Select(t => new TeamDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    OrderIndex = t.OrderIndex
                })
                .ToList();

            var rounds = game.Rounds
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
                .ToList();

            var totalScores = game.Teams.ToDictionary(t => t.Id, t => 0);
            foreach (var round in game.Rounds)
            {
                foreach (var score in round.Scores)
                {
                    totalScores[score.TeamId] += score.Score;
                }
            }

            var maxScore = totalScores.Count > 0 ? totalScores.Values.Max() : 0;
            var finished = maxScore >= 10000;

            Team? winningTeam = null;
            if (finished)
            {
                var winnerTeamId = totalScores
                    .OrderByDescending(kv => kv.Value)
                    .First().Key;

                winningTeam = game.Teams.First(t => t.Id == winnerTeamId);
                game.IsFinished = true;
                game.WinningTeamId = winningTeam.Id;
                await _context.SaveChangesAsync();
            }

            var dto = new GameDetailsDto
            {
                Id = game.Id,
                Teams = teams,
                Rounds = rounds,
                IsFinished = finished,
                WinningTeamId = winningTeam?.Id,
                WinningTeamName = winningTeam?.Name
            };

            return dto;
        }
    }

}
