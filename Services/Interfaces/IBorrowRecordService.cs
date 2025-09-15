using Domain.DTO;
using Services.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IBorrowRecordService : ICrudRepository<BorrowRecordDTO>
    {
        public ServiceResult<bool> ReturnBook(int borrowRecordId);  
        public IQueryable<BorrowRecordDTO> GetActiveBorrows();

    }
}
