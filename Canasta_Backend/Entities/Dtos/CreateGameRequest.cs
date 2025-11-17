using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos
{
    public class CreateGameRequest
    {
        public List<string> TeamNames { get; set; } = new();
    }

}
