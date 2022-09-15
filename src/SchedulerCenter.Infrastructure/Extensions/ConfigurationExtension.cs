using Microsoft.Extensions.Configuration;
using SchedulerCenter.Core.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace SchedulerCenter.Infrastructure.Extensions
{
   public static class ConfigurationExtension
    {

        public static AppSetting GetAppSetting(this IConfiguration conf)
        {

            return conf.Get<AppSetting>();

        }

    }
}
