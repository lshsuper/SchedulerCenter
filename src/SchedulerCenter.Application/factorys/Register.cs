using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerCenter.Application.factorys
{
  public  static class Register
    {

        public static void RegisterFactory(this IServiceCollection services)
        {

         
            services.AddTransient<JobServiceFactory>();
           

        }

    }
}
