using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Game
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsFinished { get; set; }

        public int? WinningTeamId { get; set; }
        public Team? WinningTeam { get; set; }

        public ICollection<Team> Teams { get; set; } = new List<Team>();
        public ICollection<Round> Rounds { get; set; } = new List<Round>();
    }

}
