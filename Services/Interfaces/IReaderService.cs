using Domain.DTO;
using Services.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IReaderService : ICrudRepository<ReaderDTO>
    {

        public ReaderDTO GetProfile(int userId);
    }
}
