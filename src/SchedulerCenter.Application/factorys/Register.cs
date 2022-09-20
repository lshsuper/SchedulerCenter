using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerCenter.Application.Factorys
{
  public  static class Register
    {

        public static void RegisterFactory(this IServiceCollection services)
        {

         
            services.AddTransient<JobServiceFactory>();
           

        }

    }
}
