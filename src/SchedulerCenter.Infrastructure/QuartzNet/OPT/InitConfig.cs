using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchedulerCenter.Infrastructure.QuartzNet.OPT
{

    public class InitConfig
    {
        public string DbProviderName { get; set; }
        public string ConnectionString { get; set; }
      
        public  string SchedulerName { get; set; }
    }

}
