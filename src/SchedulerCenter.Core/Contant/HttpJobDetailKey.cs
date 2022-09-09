using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchedulerCenter.Core.Constant
{

    /// <summary>
    /// HttpJobDetailKey 任务扩展map
    /// </summary>
    public class HttpJobDetailKey
    {
        /// <summary>
        /// http请求地址
        /// </summary>
        public const string ApiUrl = "ApiUrl";
        /// <summary>
        /// http请求方式
        /// </summary>
        public const string ApiMethod = "ApiMethod";
        /// <summary>
        /// http-header-key
        /// </summary>
        public const string ApiAuthKey = "ApiAuthKey";
        /// <summary>
        /// http-header-value
        /// </summary>
        public const string ApiAuthValue = "ApiAuthValue";

    }
}
