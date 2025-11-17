using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Team
    {
        public int Id { get; set; }

        public int GameId { get; set; }
        public Game Game { get; set; } = null!;

        public string Name { get; set; } = null!;

        public int OrderIndex { get; set; }
    }

}
