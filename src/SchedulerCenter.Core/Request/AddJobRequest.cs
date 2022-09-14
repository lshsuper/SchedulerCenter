using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerCenter.Core.Request
{

    /// <summary>
    /// 请求体-创建任务
    /// </summary>
    public class AddJobRequest
    {
        /// <summary>
        /// 任务名称
        /// </summary>
        public string TaskName { get; set; }
        /// <summary>
        /// 任务分组
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// CRON表达式
        /// </summary>
        public string Interval { get; set; }
        /// <summary>
        /// Api请求地址
        /// </summary>
        public string ApiUrl { get; set; }
        /// <summary>
        /// 授权Key
        /// </summary>
        public string AuthKey { get; set; }
        /// <summary>
        /// 授权Value
        /// </summary>
        public string AuthValue { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Describe { get; set; }
        /// <summary>
        /// 请求类型
        /// </summary>
        public string RequestType { get; set; }
         /// <summary>
         /// 调取器名称
         /// </summary>
        public string SchedulerName { get; set; }

    }
}
