using Quartz.Impl.Triggers;
using SchedulerCenter.Application.Jobs;
using SchedulerCenter.Core.Common;
using SchedulerCenter.Core.Model;
using SchedulerCenter.Core.Option;
using SchedulerCenter.Host.Models;
using SchedulerCenter.Host.Options;
using SchedulerCenter.Infrastructure.Dapper;
using SchedulerCenter.Infrastructure.QuartzNet;
using SchedulerCenter.Infrastructure.QuartzNet.OPT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SchedulerCenter.Core.Constant;
using SchedulerCenter.Infrastructure.Extensions;

namespace SchedulerCenter.Application.Services
{
    public class JobService
    {

        private QuartzProvider _provider;

        private DapperProvider _dapperProvider;
        public JobService(QuartzProvider provider, DapperProvider dapperProvider) {
            _provider = provider;
            _dapperProvider = dapperProvider;
        }



        public async Task<ApiResult<bool>> AddJob(TaskOPT opt)
        {

            //验证表达式
            if (!_provider.CheckCron(opt.Interval)){ return ApiResult<bool>.Error("表达式格式不正确"); }
            //存在当前分组-任务名的任务
            if (_provider.JobExists(opt.TaskName, opt.GroupName).Result) { return ApiResult<bool>.Error("任务已存在"); }

            try
            {

                var res =await _provider.AddCronJob<HttpJob>(new AddCronJobOPT {
                
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

                    }

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
        public async Task<ApiResult<bool>> RemoveJob(string jobName,string jobGroup) {


            try
            {

                //1.当前job-remove
                var job = await _provider.GetJobDetail(jobName, jobGroup);
                await _provider.PauseTrigger(jobName, jobGroup);
                await _provider.UnscheduleJob(jobName, jobGroup);// 移除触发器
                await _provider.DeleteJob(jobName, jobGroup);

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
        public async Task<ApiResult<bool>> UpdateJob(TaskOPT opt)
        {


            try
            {

                //验证表达式
                if (!_provider.CheckCron(opt.Interval)) { return ApiResult<bool>.Error("表达式格式不正确"); }
                //存在当前分组-任务名的任务
                if (_provider.JobExists(opt.TaskName, opt.GroupName).Result) { return ApiResult<bool>.Error("任务已存在"); }

                //1.清除
                await _provider.PauseTrigger(opt.TaskName, opt.GroupName);
                await _provider.UnscheduleJob(opt.TaskName, opt.GroupName);// 移除触发器
                await _provider.DeleteJob(opt.TaskName, opt.GroupName);
                
                //2.新构建
                var res = await _provider.AddCronJob<HttpJob>(new AddCronJobOPT
                {

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
        public async Task<ApiResult<bool>> StartJob(string triggerName,string triggerGroup) {
            try
            {

                await _provider.ResumeTrigger(triggerName, triggerGroup);

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
        public async Task<ApiResult<bool>> PauseJob(string triggerName, string triggerGroup)
        {
            try
            {

                await _provider.PauseTrigger(triggerName, triggerGroup);

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
        public  async Task<List<TaskOPT>> GetJobs()
        {
           
            try
            {
                List<TaskOPT> list = new List<TaskOPT>();
                var groups = await _provider.GetJobGroupNames();
                groups.ForEach(async(groupName, i)=> {

                    var jobs = await _provider.GetJobKeys(groupName);
                    jobs.ForEach(async(jobKey,j) => {
                        var job = await _provider.GetJobDetail(jobKey.Group, jobKey.Name);
                        var triggers = await _provider.GetTriggersOfJob(jobKey.Group, jobKey.Name);
                        var trigger = triggers.AsEnumerable().FirstOrDefault();

                        list.Add(new TaskOPT()
                        {
                            GroupName = jobKey.Group,
                            TaskName = jobKey.Name,
                            Status = (await _provider.GetTriggerState(trigger.Key.Group, trigger.Key.Name)).GetHashCode(),
                            Describe = job.Description,
                            Interval = (trigger as CronTriggerImpl)?.CronExpressionString,
                            LastRunTime = trigger.GetPreviousFireTimeUtc()?.LocalDateTime,
                            ApiUrl = job.JobDataMap.GetString(HttpJobDetailKey.ApiUrl),
                            RequestType = job.JobDataMap.GetString(HttpJobDetailKey.ApiMethod)
                        });

                    });

                });
                return list;
              
            }
            catch (Exception ex)
            {
                //FileQuartz.WriteStartLog("获取作业异常：" + ex.Message + ex.StackTrace);
               
            }
            return new List<TaskOPT>();
        }

        /// <summary>
        /// 运行任务
        /// </summary>
        /// <param name="jobGroup"></param>
        /// <param name="jobName"></param>
        /// <returns></returns>
        public async Task<ApiResult<bool>> RunJob(string jobGroup, string jobName)
        {
            try
            {

                await _provider.TriggerJob(jobGroup, jobName);

                return ApiResult<bool>.OK(true, "任务执行成功");
            }
            catch (Exception ex)
            {

                return ApiResult<bool>.Error(ex.Message);
            }

        }


        
        public async Task<int> Logger(LoggerOPT opt) {


            return await _dapperProvider.ExcuteAsync("insert into qrtz_logs(start_time,end_time,job_name,job_group,trigger_name,trigger_group,content,level)values(@start_time,@end_time,@job_name,@job_group,@trigger_name,@trigger_group,@content,@level)", new {
                job_name=opt.JobName,
                job_group=opt.JobGroup,
                trigger_name=opt.TriggerName,
                trigger_group=opt.TriggerGroup,
                content=opt.Content,
                level=opt.Level.ToString(),
                start_time=opt.StartTime,
                end_time=opt.EndTime,
            });


        }


        public async Task<IEnumerable<TaskLogDTO>> GetJobLogPage(string taskName, string groupName, int page, int pageSize = 5) {


            int offset = (page - 1) * pageSize;
            string countSql = "select count(1) from qrtz_logs where job_name=@job_name and job_group=@job_group";
            int count = await _dapperProvider.QueryFirstOrDefaultAsync<int>(countSql, new { job_name=taskName, job_group=groupName });
            string dataSql = "select start_time 'StartTime',end_time 'Endtime',content 'Content' from qrtz_logs where job_name=@job_name and job_group=@job_group  order by id desc limit @offset,@limit";

           var data = await _dapperProvider.QueryAsync<LogModel>(dataSql, new { job_name = taskName, job_group = groupName, offset=offset, limit=pageSize });

            return data.Select((s) => new TaskLogDTO
            {
                 Msg=s.Content,
                 BeginDate=s.StartTime.ToString("yyyy-MM-dd HH:mm:ss:ms"),
                 EndDate=s.EndTime.ToString("yyyy-MM-dd HH:mm:ss:ms"),
            });
        }



    }
}
