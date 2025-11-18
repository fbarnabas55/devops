using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos
{
    public class SaveRoundRequest
    {
        public int GameId { get; set; }
        public int RoundNumber { get; set; }
        public List<RoundScoreDto> Scores { get; set; } = new();
    }
}
