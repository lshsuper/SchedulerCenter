
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SchedulerCenter.Core.Common;
using SchedulerCenter.Core.DTO;
using SchedulerCenter.Core.Interface;
using SchedulerCenter.Core.Option;
using SchedulerCenter.Host.Models;

namespace SchedulerCenter.Application.Services
{


    /// <summary>
    /// 远程任务Service
    /// </summary>
    public class JobRemoteService : IJobService
    {

        readonly IHttpClientFactory _clientFactory;
        private string _host;
 
        private readonly SettingService _settingService;
        public JobRemoteService(IHttpClientFactory clientFactory, SettingService settingService)
        {
            _clientFactory = clientFactory;
            _settingService = settingService;
        }


        public  async Task<IJobService> Init(string schedName) {

            var node =await _settingService.GetNode(schedName);
            _host = node.Addr;
            return this;

        }

        public Task<ApiResult<bool>> AddJob(TaskOPT opt)
        {
            throw new NotImplementedException();

        }

        public Task<IEnumerable<SchedulerDTO>> GetAllSchedulers()
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult<IEnumerable<TaskLogDTO>>> GetJobLogPage(string taskName, string groupName, int page, int pageSize = 5)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult<IEnumerable<TaskOPT>>> GetJobs(string schedulerName)
        {
            throw new NotImplementedException();
        }

        public Task<int> Logger(LoggerOPT opt)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult<bool>> PauseJob(string schedulerName, string triggerName, string triggerGroup)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult<bool>> RemoveJob(string schedulerName, string jobName, string jobGroup)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult<bool>> RunJob(string schedulerName, string jobGroup, string jobName)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult<bool>> StartJob(string schedulerName, string triggerName, string triggerGroup)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult<bool>> UpdateJob(TaskOPT opt)
        {
            throw new NotImplementedException();
        }

      
    }
}
