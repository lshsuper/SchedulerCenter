using Quartz;
using System;

namespace SchedulerCenter.Host.Models
{
    public class TaskOptions
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
        public TriggerState Status { get; set; }
    }
}
