using Microsoft.AspNetCore.Mvc;
using SchedulerCenter.Host.Attr;

using Quartz.Spi;
using System.Threading.Tasks;
using SchedulerCenter.Core.Option;
using SchedulerCenter.Application.Services;
using SchedulerCenter.Core.Interface;
using System;
using SchedulerCenter.Application.factory;

namespace SchedulerCenter.Host.Controllers
{

    public class TaskBackGroundController : Controller
    {


        private readonly JobServiceFactory _jobServiceFactory;
        private readonly SettingService _settingService;
        public TaskBackGroundController(SettingService settingService, JobServiceFactory jobServiceFactory)
        {
           
            _settingService =settingService;
            _jobServiceFactory =jobServiceFactory;
        }

        public IActionResult Index()
        {
            return View("~/Views/TaskBackGround/Index.cshtml");
        }


     
      
  


        /// <summary>
        /// 获取所有的作业
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetJobs(string schedulerName)
        {
            var service =await _jobServiceFactory.GetService(schedulerName);
            return Json(await service.GetJobs(schedulerName));
        }


        /// <summary>
        /// 获取作业运行日志
        /// </summary>
        /// <param name="taskName"></param>
        /// <param name="groupName"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetRunLog(string schedulerName,string taskName, string groupName, int page = 1)
        {
            var service = await _jobServiceFactory.GetService(schedulerName);
            return Json(await service.GetJobLogPage(taskName, groupName, page));
        }
        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="taskOptions"></param>
        /// <returns></returns>
        [TaskAuthor]
        public async Task<IActionResult> Add(TaskOPT taskOptions)
        {
            var service = await _jobServiceFactory.GetService(taskOptions.SchedulerName);
            return Json(await service.AddJob(taskOptions));
        }
        [TaskAuthor]
        public async Task<IActionResult> Remove(TaskOPT taskOptions)
        {
            var service = await _jobServiceFactory.GetService(taskOptions.SchedulerName);
            return Json(await service.RemoveJob(taskOptions.SchedulerName, taskOptions.TaskName,taskOptions.GroupName));
        }
        [TaskAuthor]
        public async Task<IActionResult> Update(TaskOPT taskOptions)
        {
            var service = await _jobServiceFactory.GetService(taskOptions.SchedulerName);
            return Json(await service.UpdateJob(taskOptions));
        }
        [TaskAuthor]
        public async Task<IActionResult> Pause(TaskOPT taskOptions)
        {
            var service = await _jobServiceFactory.GetService(taskOptions.SchedulerName);
            return Json(await service.PauseJob(taskOptions.SchedulerName,taskOptions.TaskName,taskOptions.GroupName));
        }
        [TaskAuthor]
        public async Task<IActionResult> Start(TaskOPT taskOptions)
        {
            var service = await _jobServiceFactory.GetService(taskOptions.SchedulerName);
            return Json(await service.StartJob(taskOptions.SchedulerName,taskOptions.TaskName,taskOptions.GroupName));
        }
        [TaskAuthor]
        public async Task<IActionResult> Run(TaskOPT taskOptions)
        {
            var service = await _jobServiceFactory.GetService(taskOptions.SchedulerName);
            return Json(await service.RunJob(taskOptions.SchedulerName,taskOptions.GroupName,taskOptions.TaskName));
        }
    }




}