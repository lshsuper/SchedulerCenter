using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerCenter.Core.Request
{

   /// <summary>
   /// 请求体-获取Token
   /// </summary>
   public class AuthorizationRequest
    {
        /// <summary>
        /// 票据码
        /// </summary>
        [JsonProperty("ticket")]
        public string Ticket { get; set; }

    }
}
