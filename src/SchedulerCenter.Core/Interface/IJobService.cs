using SchedulerCenter.Core.Common;
using SchedulerCenter.Core.DTO;
using SchedulerCenter.Core.Option;
using SchedulerCenter.Host.Models;
using SchedulerCenter.Core.Option;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerCenter.Core.Interface
{
  public  interface IJobService
    {



         Task<IJobService> Init(string schedName);

         Task<ApiResult<bool>> AddJob(TaskOPT opt);


        /// <summary>
        /// RemoveJob 移除任务
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="jobGroup"></param>
        /// <returns></returns>
        Task<ApiResult<bool>> RemoveJob(string schedulerName, string jobName, string jobGroup);
        /// <summary>
        /// 更新任务
        /// </summary>
        /// <param name="opt"></param>
        /// <returns></returns>
       Task<ApiResult<bool>> UpdateJob(TaskOPT opt);

        /// <summary>
        /// 开始任务
        /// </summary>
        /// <param name="triggerName"></param>
        /// <param name="triggerGroup"></param>
        /// <returns></returns>
        Task<ApiResult<bool>> StartJob(string schedulerName, string triggerName, string triggerGroup);
        /// <summary>
        /// 暂停任务
        /// </summary>
        /// <param name="triggerName"></param>
        /// <param name="triggerGroup"></param>
        /// <returns></returns>
        Task<ApiResult<bool>> PauseJob(string schedulerName, string triggerName, string triggerGroup);


        /// <summary>
        /// 获取任务列表
        /// </summary>
        /// <returns></returns>
        Task<ApiResult<IEnumerable<TaskOPT>>> GetJobs(string schedulerName);

        /// <summary>
        /// 运行任务
        /// </summary>
        /// <param name="jobGroup"></param>
        /// <param name="jobName"></param>
        /// <returns></returns>
        Task<ApiResult<bool>> RunJob(string schedulerName, string jobGroup, string jobName);


         Task<int> Logger(LoggerOPT opt);


        Task<IEnumerable<SchedulerDTO>> GetAllSchedulers();



      Task<ApiResult<IEnumerable<TaskLogDTO>>> GetJobLogPage(string taskName, string groupName, int page, int pageSize = 5);
       

    }
}
