using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Round
    {
        public int Id { get; set; }

        public int GameId { get; set; }
        public Game Game { get; set; } = null!;

        public int RoundNumber { get; set; }

        public ICollection<RoundScore> Scores { get; set; } = new List<RoundScore>();
    }

}
