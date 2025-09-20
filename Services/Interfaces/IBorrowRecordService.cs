using Domain.DTO;
using Services.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IBorrowRecordService 
    {
        public ServiceResult<bool> ReturnBook(int borrowRecordId);  
        public IQueryable<BorrowRecordDTO> GetActiveBorrows();

        public IQueryable<BorrowRecordDTO> GetBookHistory(int bookId);

        public IQueryable<BorrowRecordDTO> GetReaderHistory(int readerId);
        public IQueryable<BorrowRecordDTO> GetOverdueBooks();

        public ServiceResult<BorrowRecordDTO> BorrowBook(BorrowRecordDTO borrowRecordDTO);

    }
}
