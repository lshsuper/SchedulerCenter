using Microsoft.Extensions.DependencyInjection;
using SchedulerCenter.Core.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerCenter.Application.Services
{
   public static class Register
    {

        public static void  RegisterService(this IServiceCollection services) {

            services.AddTransient<SettingService>();
            services.AddTransient<IJobService, JobRemoteService>();
            services.AddTransient<IJobService, JobService>();

        }

    }
}
