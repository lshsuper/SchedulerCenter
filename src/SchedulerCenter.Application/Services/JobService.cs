using Quartz.Impl.Triggers;
using SchedulerCenter.Application.Jobs;
using SchedulerCenter.Infrastructure.QuartzNet;
using SchedulerCenter.Infrastructure.QuartzNet.OPT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SchedulerCenter.Infrastructure.Extensions;
using SchedulerCenter.Infrastructure.Dapper;
using SchedulerCenter.Core.Interface;
using SchedulerCenter.Core.Option;
using SchedulerCenter.Core.Common;
using SchedulerCenter.Core.Constant;
using SchedulerCenter.Core.DTO;
using SchedulerCenter.Host.Models;
using SchedulerCenter.Core.Model;

namespace SchedulerCenter.Application.Services
{
    public class JobService: JobBaseService
    {

        private QuartzProvider _provider;

        private DapperProvider _dapperProvider;
        public JobService(QuartzProvider provider, DapperProvider dapperProvider):base(dapperProvider) {
            _provider = provider;
            _dapperProvider = dapperProvider;
        }



        public override async  Task<ApiResult<bool>> AddJob(TaskOPT opt)
        {

            //验证表达式
            if (!_provider.CheckCron(opt.Interval)){ return ApiResult<bool>.Error("表达式格式不正确"); }
            //存在当前分组-任务名的任务
            if (_provider.JobExists(opt.SchedulerName,opt.TaskName, opt.GroupName).Result) { return ApiResult<bool>.Error("任务已存在"); }

            try
            {
                if (string.IsNullOrEmpty(opt.AuthKey)) opt.AuthKey = string.Empty;
                if (string.IsNullOrEmpty(opt.AuthValue)) opt.AuthValue = string.Empty;
                var res =await _provider.AddCronJob<HttpJob>( new AddCronJobOPT {
                
                    TriggerGroup=opt.GroupName,
                    TriggerName=opt.TaskName,
                    JobName=opt.TaskName,
                    JobGroup=opt.GroupName,
                    Descr=opt.Describe,
                    Cron=opt.Interval,
                    JobData=new Dictionary<string, object>() { 

                       [HttpJobDetailKey.ApiUrl]=opt.ApiUrl,
                       [HttpJobDetailKey.ApiMethod]=opt.RequestType,
                       [HttpJobDetailKey.ApiAuthKey] =opt.AuthKey,
                       [HttpJobDetailKey.ApiAuthValue] = opt.AuthValue,

                    },SchedulerName=opt.SchedulerName

                });

                if (!res) { return ApiResult<bool>.Error("任务构建失败"); }


                return ApiResult<bool>.OK(true, "任务构建成功");

               
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Error(ex.Message);
            }
          
        }

        /// <summary>
        /// RemoveJob 移除任务
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="jobGroup"></param>
        /// <returns></returns>
        public override async Task<ApiResult<bool>> RemoveJob(string schedulerName, string jobName,string jobGroup) {


            try
            {

                ////1.当前job-remove
                var job = await _provider.GetJobDetail(schedulerName,jobName, jobGroup);
                await _provider.PauseTrigger(schedulerName,jobName, jobGroup);
                await _provider.UnscheduleJob(schedulerName,jobName, jobGroup);// 移除触发器
                await _provider.DeleteJob(schedulerName,jobName, jobGroup);

                return ApiResult<bool>.OK(true, "任务删除成功");

            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Error(ex.Message);
            }
            



                      

        
        }

        /// <summary>
        /// 更新任务
        /// </summary>
        /// <param name="opt"></param>
        /// <returns></returns>
        public override async Task<ApiResult<bool>> UpdateJob(TaskOPT opt)
        {


            try
            {

                //验证表达式
                if (!_provider.CheckCron(opt.Interval)) { return ApiResult<bool>.Error("表达式格式不正确"); }
                //存在当前分组-任务名的任务
                if (_provider.JobExists(opt.SchedulerName,opt.TaskName, opt.GroupName).Result) { return ApiResult<bool>.Error("任务已存在"); }

                //1.清除
                await _provider.PauseTrigger(opt.SchedulerName,opt.TaskName, opt.GroupName);
                await _provider.UnscheduleJob(opt.SchedulerName,opt.TaskName, opt.GroupName);// 移除触发器
                await _provider.DeleteJob(opt.SchedulerName,opt.TaskName, opt.GroupName);


                if (string.IsNullOrEmpty(opt.AuthKey)) opt.AuthKey = string.Empty;
                if (string.IsNullOrEmpty(opt.AuthValue)) opt.AuthValue = string.Empty;
                //2.新构建
                var res = await _provider.AddCronJob<HttpJob>(new AddCronJobOPT
                {
                    SchedulerName=opt.SchedulerName,
                    TriggerGroup = opt.GroupName,
                    TriggerName = opt.TaskName,
                    JobName = opt.TaskName,
                    JobGroup = opt.GroupName,
                    Descr = opt.Describe,
                    Cron = opt.Interval,
                    JobData=new Dictionary<string, object>() {
                        [HttpJobDetailKey.ApiUrl] = opt.ApiUrl,
                        [HttpJobDetailKey.ApiMethod] = opt.RequestType,
                        [HttpJobDetailKey.ApiAuthKey] = opt.AuthKey,
                        [HttpJobDetailKey.ApiAuthValue] = opt.AuthValue,
                    }

                });

               
                if (!res) { return ApiResult<bool>.Error("任务构建失败"); }

                return ApiResult<bool>.OK(true, "任务更新成功");

            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Error(ex.Message);
            }







        }


        /// <summary>
        /// 开始任务
        /// </summary>
        /// <param name="triggerName"></param>
        /// <param name="triggerGroup"></param>
        /// <returns></returns>
        public override async Task<ApiResult<bool>> StartJob(string schedulerName, string triggerName,string triggerGroup) {
            try
            {

                await _provider.ResumeTrigger(schedulerName,triggerName, triggerGroup);

                return ApiResult<bool>.OK(true, "任务启动成功");
            }
            catch (Exception ex)
            {

                return ApiResult<bool>.Error(ex.Message);
            }

        }
        /// <summary>
        /// 暂停任务
        /// </summary>
        /// <param name="triggerName"></param>
        /// <param name="triggerGroup"></param>
        /// <returns></returns>
        public override async Task<ApiResult<bool>> PauseJob(string schedulerName, string triggerName, string triggerGroup)
        {
            try
            {

                await _provider.PauseTrigger(schedulerName,triggerName, triggerGroup);

                return ApiResult<bool>.OK(true, "任务暂停成功");
            }
            catch (Exception ex)
            {

                return ApiResult<bool>.Error(ex.Message);
            }

        }

        /// <summary>
        /// 获取任务列表
        /// </summary>
        /// <returns></returns>
        public override async Task<ApiResult<IEnumerable<TaskOPT>>> GetJobs(string schedulerName)
        {
           
            try
            {
                List<TaskOPT> list = new List<TaskOPT>();
                var groups = await _provider.GetJobGroupNames(schedulerName);
                groups.ForEach(async(groupName, i)=> {

                    var jobs = await _provider.GetJobKeys(schedulerName,groupName);
                    jobs.ForEach(async(jobKey,j) => {
                        var job = await _provider.GetJobDetail(schedulerName,jobKey.Group, jobKey.Name);
                        var triggers = await _provider.GetTriggersOfJob(schedulerName,jobKey.Group, jobKey.Name);
                        var trigger = triggers.AsEnumerable().FirstOrDefault();

                        list.Add(new TaskOPT()
                        {
                            GroupName = jobKey.Group,
                            TaskName = jobKey.Name,
                            Status = (await _provider.GetTriggerState(schedulerName,trigger.Key.Group, trigger.Key.Name)).GetHashCode(),
                            Describe = job.Description,
                            Interval = (trigger as CronTriggerImpl)?.CronExpressionString,
                            LastRunTime = trigger.GetPreviousFireTimeUtc()?.LocalDateTime,
                            ApiUrl = job.JobDataMap.GetString(HttpJobDetailKey.ApiUrl),
                            RequestType = job.JobDataMap.GetString(HttpJobDetailKey.ApiMethod),
                           SchedulerName=schedulerName,
                        });

                    });

                });
                return ApiResult<IEnumerable<TaskOPT>>.OK(list);
              
            }
            catch (Exception ex)
            {
                //FileQuartz.WriteStartLog("获取作业异常：" + ex.Message + ex.StackTrace);
               
            }
            return ApiResult<IEnumerable<TaskOPT>>.Error("回去任务列表失败",new List<TaskOPT>());
        }

        /// <summary>
        /// 运行任务
        /// </summary>
        /// <param name="jobGroup"></param>
        /// <param name="jobName"></param>
        /// <returns></returns>
        public override async Task<ApiResult<bool>> RunJob(string schedulerName,string jobGroup, string jobName)
        {
            try
            {

                await _provider.TriggerJob(schedulerName,jobGroup, jobName);

                return ApiResult<bool>.OK(true, "任务执行成功");
            }
            catch (Exception ex)
            {

                return ApiResult<bool>.Error(ex.Message);
            }

        }


        
     


        public override async Task<IEnumerable<SchedulerDTO>> GetAllSchedulers() {

            var all = await _provider.GetAllSchedulers();
            return all.Select(s=> new SchedulerDTO() { 
                 SchedulerName=s.SchedulerName,
            });


        }


      
      
       public override Task<IJobService> Init(string schedName)
        {
          
            return Task.FromResult<IJobService>(this);
        }
    }
}
