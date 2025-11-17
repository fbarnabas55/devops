using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos
{
    public class UpdateScoreRequest
    {
        public int GameId { get; set; }
        public int RoundNumber { get; set; }
        public int TeamId { get; set; }
        public int Score { get; set; }
    }
}
