﻿using Microsoft.AspNetCore.Builder;
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
        public static void AddQuartz(this IServiceCollection services)
        {

            services.AddSingleton(new QuartzProvider());

        }


        /// <summary>
        /// UseQuartz 启用Quartz
        /// </summary>
        /// <param name="applicationBuilder"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseQuartz(this IApplicationBuilder applicationBuilder, InitConfig conf)
        {


            var services = applicationBuilder.ApplicationServices;

            var quartzProvider = services.GetService<QuartzProvider>();
            //初始化
            quartzProvider.Init(conf).GetAwaiter();
           
            return applicationBuilder;
        }

    }
}
