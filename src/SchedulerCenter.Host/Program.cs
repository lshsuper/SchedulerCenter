using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SchedulerCenter.Host;

namespace SchedulerCenter.Hostx
{
    public class Program
    {

        //public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        //    WebHost.CreateDefaultBuilder(args).UseKestrel().UseUrls("http://*:9950")
        //        .UseStartup<Startup>();

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
               Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                   .ConfigureWebHostDefaults(webBuilder =>
                   {
                      // webBuilder.UseKestrel().UseUrls("http://*:9950");
                       webBuilder.UseIIS();
                       webBuilder.UseStartup<Startup>();
                   });
    }
}
