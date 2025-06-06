using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs
{
    public class ApiResponse
    {
        public int code { get; set; }
        public string? message { get; set; }

        public ApiResponse()
        {
        }

        public ApiResponse(int code, string message)
        {
            this.code = code;
            this.message = message;
        }
    }

    public class ApiResponse<T> : ApiResponse
    {
        public T? result { get; set; }

        public ApiResponse() : base()
        {
        }

        public ApiResponse(int code, string message) : base(code, message)
        {
        }

        public ApiResponse(int code, string message, T result) : base(code, message)
        {
            this.result = result;
        }
    }
}
