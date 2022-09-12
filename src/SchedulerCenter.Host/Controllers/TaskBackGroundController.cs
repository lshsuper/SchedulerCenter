using Microsoft.AspNetCore.Mvc;
using SchedulerCenter.Host.Attr;

using Quartz.Spi;
using System.Threading.Tasks;
using SchedulerCenter.Core.Option;
using SchedulerCenter.Application.Services;

namespace SchedulerCenter.Host.Controllers
{

    public class TaskBackGroundController : Controller
    {
      
      
        private readonly JobService _jobService;
        public TaskBackGroundController(JobService jobService)
        {
            _jobService = jobService;
        }

        public IActionResult Index()
        {
            return View("~/Views/TaskBackGround/Index.cshtml");
        }


        /// <summary>
        /// 获取所有的作业
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetSchedulers()
        {
            return Json(await _jobService.GetAllSchedulers());
        }


        /// <summary>
        /// 获取所有的作业
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetJobs(string schedulerName)
        {
            return Json(await _jobService.GetJobs(schedulerName));
        }


        /// <summary>
        /// 获取作业运行日志
        /// </summary>
        /// <param name="taskName"></param>
        /// <param name="groupName"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetRunLog(string taskName, string groupName, int page = 1)
        {
           
            return Json(await _jobService.GetJobLogPage(taskName, groupName, page));
        }
        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="taskOptions"></param>
        /// <returns></returns>
        [TaskAuthor]
        public async Task<IActionResult> Add(TaskOPT taskOptions)
        {
            return Json(await _jobService.AddJob(taskOptions));
        }
        [TaskAuthor]
        public async Task<IActionResult> Remove(TaskOPT taskOptions)
        {
            return Json(await _jobService.RemoveJob(taskOptions.SchedulerName, taskOptions.TaskName,taskOptions.GroupName));
        }
        [TaskAuthor]
        public async Task<IActionResult> Update(TaskOPT taskOptions)
        {
            return Json(await _jobService.UpdateJob(taskOptions));
        }
        [TaskAuthor]
        public async Task<IActionResult> Pause(TaskOPT taskOptions)
        {
            return Json(await _jobService.PauseJob(taskOptions.SchedulerName,taskOptions.TaskName,taskOptions.GroupName));
        }
        [TaskAuthor]
        public async Task<IActionResult> Start(TaskOPT taskOptions)
        {
            return Json(await _jobService.StartJob(taskOptions.SchedulerName,taskOptions.TaskName,taskOptions.GroupName));
        }
        [TaskAuthor]
        public async Task<IActionResult> Run(TaskOPT taskOptions)
        {
            return Json(await _jobService.RunJob(taskOptions.SchedulerName,taskOptions.GroupName,taskOptions.TaskName));
        }
    }




}