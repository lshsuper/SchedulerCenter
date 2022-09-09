using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
namespace SchedulerCenter.Host.Utility
{
    public class ServiceLocator
    {

        public static IServiceProvider _instance { get; private set; }

        public static void Init(IServiceProvider instance) {
            _instance = instance;
        }

        public static T  GetService<T>()  {

            return _instance.GetService<T>();
    }
    }
}
