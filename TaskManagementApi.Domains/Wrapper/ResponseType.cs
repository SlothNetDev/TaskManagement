using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagementApi.Domains.Wrapper
{
    public class ResponseType<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }

        public static ResponseType<T> SuccessResult(T data, string message = "") =>
            new ResponseType<T> { Success = true, Data = data, Message = message };

        public static ResponseType<T> Failure(List<string> errors, string message = "") =>
            new ResponseType<T> { Success = false, Errors = errors, Message = message };
    }
}
