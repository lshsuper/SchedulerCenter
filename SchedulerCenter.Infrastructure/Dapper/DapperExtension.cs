using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
namespace SchedulerCenter.Infrastructure.Dapper
{
    public static class DapperExtension
    {

        public static void AddDapper(this IServiceCollection services,Func<IDbConnection>call) {

            services.AddSingleton(new DapperProvider(call));
        
        }

    }
}
