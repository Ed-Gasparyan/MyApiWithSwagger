using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Shared
{
    public class ServiceResult<T>
    {
        public bool Success { get; private set; }


        public string? Message { get; private set; }

        public T? Data { get; private set; }

        private ServiceResult(bool success, T? data, string? message)
        {
            Success = success;
            Data = data;
            Message = message;
        }

        public static ServiceResult<T> Ok(T? data) => new ServiceResult<T>(true, data, null);
        public static ServiceResult<T> Ok(T? data,string warningMessage) => new ServiceResult<T>(true, data, warningMessage);

        public static ServiceResult<T> Fail(string errorMessage) => new ServiceResult<T>(false, default, errorMessage);
    }
}

