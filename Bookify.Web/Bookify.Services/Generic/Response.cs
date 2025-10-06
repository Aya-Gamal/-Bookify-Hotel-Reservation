using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Services.Generic
{
    public class Response
    {
        public bool Error { get; set; }
        public string Message { get; set; }

        protected Response(string msg, bool error)
        {
            Error = error;
            Message = msg;
        }

        public static Response OK(string message = default)
        {
            return new Response(message, false);
        }

        public static Response Fail(string message)
        {
            return new Response(message, true);
        }
    }

    public class Response<T> : Response
    {
        public T Data { get; set; }
        private Response(T data, string msg, bool error) : base(msg, error)
        {
            Data = data;
        }

        public static Response<T> Ok(T data, string message = default)
        {
            return new Response<T>(data, message, false);
        }

        public static Response<T> Fail(string message)
        {
            return new Response<T>(default, message, true);
        }
    }
}
