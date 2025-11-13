using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Services.Helpers
{
    public class ResponseHelper
    {
        public bool Error { get; set; }
        public string Message { get; set; }

        protected ResponseHelper(string msg, bool error)
        {
            Error = error;
            Message = msg;
        }

        public static ResponseHelper OK(string message = default)
        {
            return new ResponseHelper(message, false);
        }

        public static ResponseHelper Fail(string message)
        {
            return new ResponseHelper(message, true);
        }
    }

    public class ResponseHelper<T> : ResponseHelper
    {
        public T Data { get; set; }
        private ResponseHelper(T data, string msg, bool error) : base(msg, error)
        {
            Data = data;
        }

        public static ResponseHelper<T> Ok(T data, string message = default)
        {
            return new ResponseHelper<T>(data, message, false);
        }

        public static ResponseHelper<T> Fail(string message)
        {
            return new ResponseHelper<T>(default, message, true);
        }
    }
}
