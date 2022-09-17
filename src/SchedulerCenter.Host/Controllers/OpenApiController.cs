using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SchedulerCenter.Application.factorys;
using SchedulerCenter.Application.Services;
using SchedulerCenter.Core.Common;
using SchedulerCenter.Core.Interface;
using SchedulerCenter.Core.Option;
using SchedulerCenter.Core.Request;
using SchedulerCenter.Host.Attributes;
using SchedulerCenter.Host.Models;
using SchedulerCenter.Infrastructure.Extensions;
using SchedulerCenter.Infrastructure.Jwt;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerCenter.Host.Controllers
{


    /// <summary>
    /// 对外API
    /// </summary>
   
    [ApiController,Route("api/[controller]/[action]"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OpenApiController : ControllerBase
    {
        private readonly JobServiceFactory _jobServiceFactory;

        private readonly IConfiguration _configuration;
        private IJobService _jobService;
        /// <summary>
        /// 开放API-构造器
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="jobServiceFactory"></param>
        public OpenApiController(IConfiguration configuration, JobServiceFactory jobServiceFactory)
        {
            _configuration = configuration;
            _jobServiceFactory = jobServiceFactory;
           
        }

        #region +Open-API-Common
        /// <summary>
        /// 获取SC-TOKEN
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [SwaggerApiGroup(SwaggerApiGroupName.Common, SwaggerApiGroupName.All), AllowAnonymous, HttpPost]
        public ApiResult<string> Authorization(AuthorizationRequest req)
        {

            if (string.IsNullOrEmpty(req.Ticket)) return ApiResult<string>.Error("[ticket]不能为空");

            var appSetting = _configuration.Get<AppSetting>();
            string _token = appSetting.Token;
            string superToken =appSetting.SuperToken;
            if (_token != req.Ticket && superToken != req.Ticket) return ApiResult<string>.Error("[ticket]不合法");

            var t = JwtUtil.GetToken(appSetting.JwtConfig,new Dictionary<string, object>() { 
            
                ["Ticket"]= req.Ticket,

            });
            return ApiResult<string>.OK(t);

        }
        #endregion

        #region +Open-API-Other
        /// <summary>
        /// 测试
        /// </summary>
        /// <returns></returns>
     
        [SwaggerApiGroup(SwaggerApiGroupName.Other, SwaggerApiGroupName.All), AllowAnonymous, HttpGet]
        public string Test()
        {

            return "ok";

        }
        #endregion

        #region +Open-API-Job

        /// <summary>
        /// 获取所有的作业
        /// </summary>
        /// <returns></returns>
        [SwaggerApiGroup(SwaggerApiGroupName.Job, SwaggerApiGroupName.All), HttpGet]
        public async Task<ApiResult<IEnumerable<TaskOPT>>> GetJobs(string schedulerName)
        {
            var _jobService = await  _jobServiceFactory.GetService(schedulerName);
            return await _jobService.GetJobs(schedulerName);
        }


        /// <summary>
        /// 获取作业运行日志
        /// </summary>
        /// <param name="taskName"></param>
        /// <param name="groupName"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [SwaggerApiGroup(SwaggerApiGroupName.Job, SwaggerApiGroupName.All), HttpGet]
        public async Task<ApiResult<IEnumerable<TaskLogDTO>>> GetRunLog(string taskName, string groupName, int page = 1)
        {
            var _jobService = await _jobServiceFactory.GetLocatService();
            return await _jobService.GetJobLogPage(taskName, groupName, page);
        }


        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [SwaggerApiGroup(SwaggerApiGroupName.Job, SwaggerApiGroupName.All), HttpPost]
        public async Task<ApiResult<bool>> AddJob(AddJobRequest req)
        {
            var _jobService = await _jobServiceFactory.GetService(req.SchedulerName);
            return await _jobService.AddJob(new TaskOPT
            {

                ApiUrl = req.ApiUrl,
                AuthValue = req.AuthValue,
                Describe = req.Describe,
                TaskName = req.TaskName,
                GroupName = req.GroupName,
                Interval = req.Interval,
                AuthKey = req.AuthKey,
                RequestType = req.RequestType,
             
                SchedulerName = req.SchedulerName,
            });
        }
        /// <summary>
        /// 移除任务
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [SwaggerApiGroup(SwaggerApiGroupName.Job, SwaggerApiGroupName.All), HttpDelete]
        public async Task<ApiResult<bool>> RemoveJob(RemoveJobRequest req)
        {
            var _jobService = await _jobServiceFactory.GetService(req.SchedulerName);
            return await _jobService.RemoveJob(req.SchedulerName, req.TaskName, req.GroupName);
        }
        /// <summary>
        /// 编辑任务
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [SwaggerApiGroup(SwaggerApiGroupName.Job, SwaggerApiGroupName.All), HttpPut]
        public async Task<ApiResult<bool>> UpdateJob(UpdateJobRequest req)
        {
            var _jobService = await _jobServiceFactory.GetService(req.SchedulerName);
            return await _jobService.UpdateJob(new TaskOPT
            {

                ApiUrl = req.ApiUrl,
                AuthValue = req.AuthValue,
                Describe = req.Describe,
                TaskName = req.TaskName,
                GroupName = req.GroupName,
                Interval = req.Interval,
                AuthKey = req.AuthKey,
                RequestType = req.RequestType,
               
                SchedulerName = req.SchedulerName,
            });
        }
        /// <summary>
        /// 暂停任务
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [SwaggerApiGroup(SwaggerApiGroupName.Job, SwaggerApiGroupName.All), HttpPatch]
        public async Task<ApiResult<bool>> PauseJOb(PauseJobRequest req)
        {
            var _jobService = await _jobServiceFactory.GetService(req.SchedulerName);
            return await _jobService.PauseJob(req.SchedulerName, req.TaskName, req.GroupName);
        }
        /// <summary>
        /// 开始任务
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [SwaggerApiGroup(SwaggerApiGroupName.Job, SwaggerApiGroupName.All), HttpPatch]
        public async Task<ApiResult<bool>> StartJob(StartJobRequest req)
        {
            var _jobService = await _jobServiceFactory.GetService(req.SchedulerName);
            return await _jobService.StartJob(req.SchedulerName, req.TaskName, req.GroupName);
        }

        /// <summary>
        /// 运行任务
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [SwaggerApiGroup(SwaggerApiGroupName.Job, SwaggerApiGroupName.All), HttpPatch]
        public async Task<ApiResult<bool>> RunJob(RunJobRequest req)
        {
            var _jobService = await _jobServiceFactory.GetService(req.SchedulerName);
            return await _jobService.RunJob(req.SchedulerName, req.GroupName, req.TaskName);
        }


        #endregion

    }
}
