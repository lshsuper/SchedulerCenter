using Microsoft.Extensions.Configuration;
using SchedulerCenter.Application.Services;
using SchedulerCenter.Core.Common;
using SchedulerCenter.Core.Interface;
using SchedulerCenter.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerCenter.Application.factory
{
  public  class JobServiceFactory
    {

        private IConfiguration _configuration { get; set; }
     
        private readonly IJobService _jobService;
        private readonly IJobService _jobRemoteService;
        public JobServiceFactory(IEnumerable<IJobService>jobServiceArr,IConfiguration configuration) {


            _configuration = configuration;
         
            _jobService = jobServiceArr.FirstOrDefault(f=>f.GetType()==typeof(JobService));
            _jobRemoteService = jobServiceArr.FirstOrDefault(f => f.GetType() == typeof(JobRemoteService));
        }

        /// <summary>8
        /// GetService 获取Service
        /// </summary>
        /// <param name="schedName"></param>
        /// <returns></returns>
        public async Task<IJobService> GetService(string schedName) {

            //判断是远程还是本地
            var appSetting = _configuration.Get<AppSetting>();

            if (appSetting.SchedulerName == schedName) {
                //本地调度
                return  await _jobService.Init(schedName);
               
            }

           return await _jobRemoteService.Init(schedName);

        }

        public async Task<IJobService> GetLocatService(string schedName)
        {
            //本地调度
            return await _jobService.Init(schedName);           

        }

    }
}
