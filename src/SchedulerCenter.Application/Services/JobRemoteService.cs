
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SchedulerCenter.Core.Common;
using SchedulerCenter.Core.Contant;
using SchedulerCenter.Core.DTO;
using SchedulerCenter.Core.Interface;
using SchedulerCenter.Core.Option;
using SchedulerCenter.Core.Request;
using SchedulerCenter.Host.Models;
using SchedulerCenter.Infrastructure.Dapper;
using SchedulerCenter.Infrastructure.Jwt;

namespace SchedulerCenter.Application.Services
{


    /// <summary>
    /// 远程任务Service
    /// </summary>
    public class JobRemoteService : JobBaseService
    {

        readonly IHttpClientFactory _clientFactory;
        private string _host;
 
        private readonly SettingService _settingService;
        private IConfiguration _configuration;
        private DapperProvider _dapperProvider;
        public JobRemoteService(DapperProvider dapperProvider,IHttpClientFactory clientFactory, SettingService settingService, IConfiguration configuration):base(dapperProvider)
        {
            _clientFactory = clientFactory;
            _settingService = settingService;
            _configuration = configuration;
            _dapperProvider = dapperProvider;
        }

        
        public override async Task<IJobService> Init(string schedName) {

            var node =await _settingService.GetNode(schedName);
            _host = node.Addr;
            return this;

        }
        private HttpClient GetClient() {


           var appSetting=_configuration.Get<AppSetting>();
           var token = JwtUtil.GetToken(appSetting.JwtConfig,new Dictionary<string, object>() { 
                  ["Ticket"]=appSetting.SuperToken,
           });
           var client = _clientFactory.CreateClient();
           client.DefaultRequestHeaders.Add(AppKey.JwtTokenKey, token);
           client.BaseAddress =new Uri(_host);
           return client;
        }
        public override async Task<ApiResult<bool>> AddJob(TaskOPT opt)
        {

            var client = GetClient();
            var req = new StringContent(JsonConvert.SerializeObject(new AddJobRequest()
            {
                ApiUrl = opt.ApiUrl,
                AuthValue = opt.AuthValue,
                Describe = opt.Describe,
                TaskName = opt.TaskName,
                GroupName = opt.GroupName,
                Interval = opt.Interval,
                AuthKey = opt.AuthKey,
                RequestType = opt.RequestType,
                SchedulerName = opt.SchedulerName,
            }
            ));
            var res=await client.PostAsync("/api/OpenApi/AddJob", req);
            var content =await res.Content.ReadAsStringAsync();
            var result=JsonConvert.DeserializeObject<ApiResult<bool>>(content);
            //发送请求
            return result;

        }

        public override Task<IEnumerable<SchedulerDTO>> GetAllSchedulers()
        {
            throw new NotImplementedException();
        }

     

        public override async Task<ApiResult<IEnumerable<TaskOPT>>> GetJobs(string schedulerName)
        {
            var client = GetClient();
            var res = await client.GetAsync($"/api/OpenApi/GetJobs?schedulerName={schedulerName}");
            var content = await res.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ApiResult<IEnumerable<TaskOPT>>>(content);
            //发送请求
            return result;
        }

      

        public override async Task<ApiResult<bool>> PauseJob(string schedulerName, string jobName, string jobGroup)
        {
            var client = GetClient();
            var req = new StringContent(JsonConvert.SerializeObject(new PauseJobRequest()
            {

                TaskName = jobName,
                GroupName = jobGroup,
                SchedulerName = schedulerName,
            }
            ));
            var res = await client.PatchAsync("/api/OpenApi/PauseJob", req);
            var content = await res.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ApiResult<bool>>(content);
            //发送请求
            return result;
        }

        public override async Task<ApiResult<bool>> RemoveJob(string schedulerName, string jobName, string jobGroup)
        {
            var client = GetClient();
            var req = new StringContent(JsonConvert.SerializeObject(new RemoveJobRequest()
            {

                TaskName = jobName,
                GroupName = jobGroup,
                SchedulerName = schedulerName,
            }
            ));
            var res = await client.PatchAsync("/api/OpenApi/RemoveJob", req);
            var content = await res.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ApiResult<bool>>(content);
            //发送请求
            return result; 
        }

        public override async Task<ApiResult<bool>> RunJob(string schedulerName, string jobGroup, string jobName)
        {
            var client = GetClient();
            var req = new StringContent(JsonConvert.SerializeObject(new RunJobRequest()
            {

                TaskName = jobName,
                GroupName = jobGroup,
                SchedulerName = schedulerName,
            }
            ));
            var res = await client.PatchAsync("/api/OpenApi/RunJob", req);
            var content = await res.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ApiResult<bool>>(content);
            //发送请求
            return result;
        }

        public override async Task<ApiResult<bool>> StartJob(string schedulerName, string jobName, string jobGroup)
        {
            var client = GetClient();
            var req = new StringContent(JsonConvert.SerializeObject(new StartJobRequest()
            {
               
                TaskName = jobName,
                GroupName = jobGroup,
                SchedulerName = schedulerName,
            }
            ));
            var res = await client.PatchAsync("/api/OpenApi/StartJob", req);
            var content = await res.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ApiResult<bool>>(content);
            //发送请求
            return result;
        }

        public override async Task<ApiResult<bool>> UpdateJob(TaskOPT opt)
        {
            var client = GetClient();
            var req = new StringContent(JsonConvert.SerializeObject(new UpdateJobRequest()
            {
                ApiUrl = opt.ApiUrl,
                AuthValue = opt.AuthValue,
                Describe = opt.Describe,
                TaskName = opt.TaskName,
                GroupName = opt.GroupName,
                Interval = opt.Interval,
                AuthKey = opt.AuthKey,
                RequestType = opt.RequestType,
                SchedulerName = opt.SchedulerName,
            }
            ));
            var res = await client.PutAsync("/api/OpenApi/UpdateJob", req);
            var content = await res.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ApiResult<bool>>(content);
            //发送请求
            return result;
        }

      
    }
}
