using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
namespace Services.Interfaces
{
    public interface IJwtService
    {
        public string GenerateToken(Reader user);
    }
}
