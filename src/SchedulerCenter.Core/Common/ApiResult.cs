using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchedulerCenter.Core.Common
{



    public class ApiResult<T>
    {
     
        
        [JsonProperty("status")]
        public bool Status { get; set; }

        [JsonProperty("msg")]
        public string Msg {get;set;}

        [JsonProperty("data")]
        public T Data { get; set; }


        public static ApiResult<T>OK(T data, string msg="")
        {
            return new ApiResult<T>()
            {
                Status = true,
                Data = data,
                Msg=msg
            };
        }

        public static ApiResult<T> Error(string msg ,T data=default(T))
        {
            return new ApiResult<T>()
            {
                Status = false,
               Msg= msg,
            };
        }
    }

   
}
