using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos
{
    public class GameListItemDto
    {
        public int Id { get; set; }
        public List<string> TeamNames { get; set; } = new();
        public bool IsFinished { get; set; }
        public string StatusText { get; set; } = string.Empty;
    }

}
