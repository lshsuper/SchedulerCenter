using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SchedulerCenter.Infrastructure.QuartzNet.OPT;
namespace SchedulerCenter.Infrastructure.QuartzNet
{
    public static class QuartzExtensions
    {
        /// <summary>
        /// AddQuartz 添加Quartz的应用
        /// </summary>
        /// <param name="services"></param>
        /// <param name="conf"></param>
        public static void AddQuartz(this IServiceCollection services, InitConfig conf)
        {

            var provider = new QuartzProvider();
            provider.Init(conf).Wait();
            services.AddSingleton(provider);
            return;

        }


      
    }
}
