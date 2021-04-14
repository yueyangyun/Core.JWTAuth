using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Core.JWTAuth.Models
{
    public class ResponseResult
    {
        [JsonProperty("code")]
        public HttpStatusCode Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
    public class ResponseResult<T> : ResponseResult where T : class
    {
        [JsonProperty("data")]
        public T Data { get; set; }
    }
}
