using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchedulerCenter.Core.Model
{
    public class LogModel
    {
        public string JobName { get; set; }

        public string JobGroup { get; set; }
        public string TriggerName { get; set; }
        public string TriggerGroup { get; set; }

        public string Content { get; set; }

        public string Level { get; set; }

        public DateTime StartTime  { get; set; }
        public DateTime EndTime { get; set; }
    }
}
