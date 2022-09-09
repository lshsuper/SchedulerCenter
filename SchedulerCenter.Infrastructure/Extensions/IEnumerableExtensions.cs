using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchedulerCenter.Infrastructure.Extensions
{
    public static class IEnumerableExtensions
    {

        public static void ForEach<T>(this IEnumerable<T>list,Action<T,int>func){

            int i = 0;
            foreach (var ele in list) {

                func(ele, i);
                i++;
            }

        }

    }
}
