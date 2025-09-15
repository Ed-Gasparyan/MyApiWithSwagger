using Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IReaderService : ICrudRepository<ReaderDTO>
    {
        IQueryable<BorrowRecordDTO> GetReaderHistory(int readerId);

    }
}
