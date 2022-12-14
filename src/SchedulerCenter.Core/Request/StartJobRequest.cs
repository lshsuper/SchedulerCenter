using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerCenter.Core.Request
{

    /// <summary>
    /// 请求体-开始任务
    /// </summary>
    public class StartJobRequest
    {
        /// <summary>
        /// 调度器名称
        /// </summary>
        public string SchedulerName { get; set; }
        /// <summary>
        /// 任务名
        /// </summary>
        public string TaskName { get; set; }
        /// <summary>
        /// 分组名
        /// </summary>
        public string GroupName { get; set; }

    }
}
