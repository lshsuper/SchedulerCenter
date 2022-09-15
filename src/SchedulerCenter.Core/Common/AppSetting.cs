using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerCenter.Core.Common
{
  public  class AppSetting
    {

        public string Token { get; set; }
        public  string SuperToken { get; set; }

        public  string AllowedHosts { get; set; }

        public  string DbProvider { get; set; }
        public  string DbConnStr { get; set; }

        public  string SchedulerName { get; set; }

        public  string SchedulerHost { get; set; }
        public JwtConfig JwtConfig { get; set; }

    }
}
