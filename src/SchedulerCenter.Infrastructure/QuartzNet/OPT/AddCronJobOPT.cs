using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchedulerCenter.Infrastructure.QuartzNet.OPT
{
    public class AddCronJobOPT
    {
        public string JobName { get; set; }
        public string JobGroup { get; set; }
        public string TriggerName { get; set; }
        public string TriggerGroup { get; set; }
        public string Cron { get; set; }
        public string Descr { get; set; }
        public IDictionary<string, object> JobData { get; set; }
        public  string SchedulerName { get; set; }
    }
}
