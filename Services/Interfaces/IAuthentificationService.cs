using Services.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.DTO;

namespace Services.Interfaces
{
    public interface IAuthentificationService
    {
        public ServiceResult<string> Register(RegisterDTO dto);
        public ServiceResult<string> Login(LoginDTO loginDTO);

    }
}
