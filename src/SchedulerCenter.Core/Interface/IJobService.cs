using SchedulerCenter.Core.Common;
using SchedulerCenter.Core.DTO;
using SchedulerCenter.Core.Option;
using SchedulerCenter.Host.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerCenter.Core.Interface
{

    /// <summary>
    /// 任务Service
    /// </summary>
    public interface IJobService
    {


        /// <summary>
        ///Init 初始化
        /// </summary>
        /// <param name="schedName"></param>
        /// <returns></returns>
        Task<IJobService> Init(string schedName);

        /// <summary>
        ///AddJob 添加
        /// </summary>
        /// <param name="opt"></param>
        /// <returns></returns>
        Task<ApiResult<bool>> AddJob(TaskOPT opt);


        /// <summary>
        /// RemoveJob 移除任务
        /// </summary>
        /// <param name="schedulerName"></param>
        /// <param name="jobName"></param>
        /// <param name="jobGroup"></param>
        /// <returns></returns>
        Task<ApiResult<bool>> RemoveJob(string schedulerName, string jobName, string jobGroup);
        /// <summary>
        ///UpdateJob 更新
        /// </summary>
        /// <param name="opt"></param>
        /// <returns></returns>
        Task<ApiResult<bool>> UpdateJob(TaskOPT opt);

        /// <summary>
        /// 开始
        /// </summary>
        /// <param name="schedulerName"></param>
        /// <param name="triggerName"></param>
        /// <param name="triggerGroup"></param>
        /// <returns></returns>
        Task<ApiResult<bool>> StartJob(string schedulerName, string triggerName, string triggerGroup);
        /// <summary>
        /// PauseJob 暂停任务
        /// </summary>
        /// <param name="schedulerName"></param>
        /// <param name="triggerName"></param>
        /// <param name="triggerGroup"></param>
        /// <returns></returns>
        Task<ApiResult<bool>> PauseJob(string schedulerName, string triggerName, string triggerGroup);


        /// <summary>
        /// GetJobs 获取任务列表
        /// </summary>
        /// <param name="schedulerName"></param>
        /// <returns></returns>
        Task<ApiResult<IEnumerable<TaskOPT>>> GetJobs(string schedulerName);

        /// <summary>
        /// RunJob 运行任务
        /// </summary>
        /// <param name="schedulerName"></param>
        /// <param name="jobGroup"></param>
        /// <param name="jobName"></param>
        /// <returns></returns>
        Task<ApiResult<bool>> RunJob(string schedulerName, string jobGroup, string jobName);

        /// <summary>
        /// RunJob 运行任务
        /// </summary>
        /// <param name="opt"></param>
        /// <returns></returns>
        Task<int> Logger(LoggerOPT opt);

        /// <summary>
        ///GetAllSchedulers  获取所有的调度
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<SchedulerDTO>> GetAllSchedulers();


        /// <summary>
        /// GetJobLogPage 获取日志分页
        /// </summary>
        /// <param name="taskName"></param>
        /// <param name="groupName"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<ApiResult<IEnumerable<TaskLogDTO>>> GetJobLogPage(string taskName, string groupName, int page, int pageSize = 5);


    }
}
