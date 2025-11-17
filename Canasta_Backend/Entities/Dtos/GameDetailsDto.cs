using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos
{
    public class GameDetailsDto
    {
        public int Id { get; set; }

        public List<TeamDto> Teams { get; set; } = new();
        public List<RoundDto> Rounds { get; set; } = new();

        public bool IsFinished { get; set; }
        public int? WinningTeamId { get; set; }
        public string? WinningTeamName { get; set; }
    }

    public class TeamDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int OrderIndex { get; set; }
    }

    public class RoundDto
    {
        public int RoundNumber { get; set; }
        public List<RoundScoreDto> Scores { get; set; } = new();
    }

    public class RoundScoreDto
    {
        public int TeamId { get; set; }
        public int Score { get; set; }
    }

}
