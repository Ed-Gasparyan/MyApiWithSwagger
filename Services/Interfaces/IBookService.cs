using Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IBookService : ICrudRepository<BookDTO>
    {
        IQueryable<BookDTO> GetAvailableBooks();
        IQueryable<BorrowRecordDTO> GetBookHistory(int bookId);


    }
}
