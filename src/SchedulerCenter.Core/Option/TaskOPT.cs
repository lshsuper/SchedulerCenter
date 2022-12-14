
using System;

namespace SchedulerCenter.Core.Option
{
    public class TaskOPT
    {
        public string TaskName { get; set; }
        public string GroupName { get; set; }
        public string Interval { get; set; }
        public string ApiUrl { get; set; }
        public string AuthKey { get; set; }
        public string AuthValue { get; set; }
        public string Describe { get; set; }
        public string RequestType { get; set; }
        public DateTime? LastRunTime { get; set; }
        public int Status { get; set; }

        public string SchedulerName { get; set; }
    }
}
