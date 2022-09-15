using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SchedulerCenter.Application.Services;
using SchedulerCenter.Core.Common;
using SchedulerCenter.Core.Option;
using SchedulerCenter.Core.Request;
using SchedulerCenter.Host.Attributes;
using SchedulerCenter.Host.Models;
using SchedulerCenter.Infrastructure.Extensions;
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
    [Route("api/[controller]/[action]"), ApiController, Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OpenApiController : ControllerBase
    {
        private readonly JobService _jobService;

        private readonly IConfiguration _configuration;
        /// <summary>
        /// 开放API-构造器
        /// </summary>
        /// <param name="configuration"></param>
        public OpenApiController(IConfiguration configuration, JobService jobService)
        {
            _configuration = configuration;
            _jobService = jobService;
        }

        #region +Open-API-Common
        /// <summary>
        ///获取SC-TOKEN
        /// </summary>
        /// <param name="ticket">票据码</param>
        /// <returns></returns>
        [SwaggerApiGroup(SwaggerApiGroupName.Common), HttpGet, AllowAnonymous]
        public ApiResult<string> Authorization(string ticket)
        {

            if (string.IsNullOrEmpty(ticket)) return ApiResult<string>.Error("[ticket]不能为空");

            var appSetting = _configuration.GetAppSetting();
            string _token = appSetting.Token;
            string superToken =appSetting.SuperToken;
            if (_token != ticket && superToken != ticket) return ApiResult<string>.Error("[ticket]不合法");

            var jwtConfig = appSetting.JwtConfig;
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim("Ticket", ticket));
            var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Secret)), SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: jwtConfig.Issuer,
                audience: jwtConfig.Audience,
                claims: claims,
                expires: DateTime.Now.AddSeconds(jwtConfig.Expire),
                signingCredentials: creds
                );
            var t = new JwtSecurityTokenHandler().WriteToken(token);
            return ApiResult<string>.OK(t);

        }
        #endregion

        #region +Open-API-Other
        /// <summary>
        /// 测试
        /// </summary>
        /// <returns></returns>
        [SwaggerApiGroup(SwaggerApiGroupName.Other), HttpGet]
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
        public async Task<ApiResult<IEnumerable<TaskOPT>>> GetJobs(string schedulerName)
        {
            return await _jobService.GetJobs(schedulerName);
        }


        /// <summary>
        /// 获取作业运行日志
        /// </summary>
        /// <param name="taskName"></param>
        /// <param name="groupName"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [SwaggerApiGroup(SwaggerApiGroupName.Job), HttpGet]
        public async Task<ApiResult<IEnumerable<TaskLogDTO>>> GetRunLog(string taskName, string groupName, int page = 1)
        {

            return await _jobService.GetJobLogPage(taskName, groupName, page);
        }


        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [SwaggerApiGroup(SwaggerApiGroupName.Job), HttpPost]
        public async Task<ApiResult<bool>> AddJob(AddJobRequest req)
        {
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
        [SwaggerApiGroup(SwaggerApiGroupName.Job), HttpDelete]
        public async Task<ApiResult<bool>> RemoveJob(RemoveJobRequest req)
        {
            return await _jobService.RemoveJob(req.SchedulerName, req.TaskName, req.GroupName);
        }
        /// <summary>
        /// 编辑任务
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [SwaggerApiGroup(SwaggerApiGroupName.Job), HttpPut]
        public async Task<ApiResult<bool>> UpdateJob(UpdateJobRequest req)
        {
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
        [SwaggerApiGroup(SwaggerApiGroupName.Job), HttpPatch]
        public async Task<ApiResult<bool>> PauseJOb(PauseJobRequest req)
        {
            return await _jobService.PauseJob(req.SchedulerName, req.TaskName, req.GroupName);
        }
        /// <summary>
        /// 开始任务
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [SwaggerApiGroup(SwaggerApiGroupName.Job), HttpPatch]
        public async Task<ApiResult<bool>> StartJob(StartJobRequest req)
        {
            return await _jobService.StartJob(req.SchedulerName, req.TaskName, req.GroupName);
        }

        /// <summary>
        /// 运行任务
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [SwaggerApiGroup(SwaggerApiGroupName.Job), HttpPatch]
        public async Task<ApiResult<bool>> RunJob(RunJobRequest req)
        {
            return await _jobService.RunJob(req.SchedulerName, req.GroupName, req.TaskName);
        }


        #endregion

    }
}
