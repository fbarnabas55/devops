using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class RoundScore
    {
        public int Id { get; set; }

        public int RoundId { get; set; }
        public Round Round { get; set; } = null!;

        public int TeamId { get; set; }
        public Team Team { get; set; } = null!;

        public int Score { get; set; }
    }

}
