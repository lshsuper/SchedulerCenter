using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerCenter.Core.Common
{
    public class JWTConfig
    {
        /// <summary>
        /// 密钥 长度不少于16位
        /// </summary>
        [JsonProperty("Secret")]
        public string Secret { get; set; }

        /// <summary>
        /// 发行人
        /// </summary>
        [JsonProperty("Issuer")]
        public string Issuer { get; set; }

        /// <summary>
        /// 观众
        /// </summary>
        [JsonProperty("Audience")]
        public string Audience { get; set; }

        /// <summary>
        /// 访问过期
        /// </summary>
        [JsonProperty("Expire")]
        public int Expire { get; set; }

       
    }
}
