using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;
using SchedulerCenter.Host.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Quartz.Logging;
using SchedulerCenter.Application.Services;
using SchedulerCenter.Infrastructure.Utility;
using SchedulerCenter.Core.Constant;
using Microsoft.Extensions.Options;
using SchedulerCenter.Host.Options;

namespace SchedulerCenter.Application.Jobs
{
    public class HttpJob : IJob
    {


        readonly  IHttpClientFactory _clientFactory;
        readonly JobService _jobService;
        public HttpJob() {
            _clientFactory = ServiceLocator.GetService<IHttpClientFactory>();
            _jobService = ServiceLocator.GetService<JobService>();

        }

        /// <summary>
        /// Execute 执行业务
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        
        public  async  Task Execute(IJobExecutionContext context)
        {


            string httpMessage = "",
                   triggerFullName="";
            DateTime dateTime = DateTime.Now;
            try
            {
             


                AbstractTrigger trigger = (context as JobExecutionContextImpl).Trigger as AbstractTrigger;
                triggerFullName = trigger.FullName;
                Dictionary<string, string> header = new Dictionary<string, string>();
                
                string httpAddr = context.JobDetail.JobDataMap.GetString(HttpJobDetailKey.ApiUrl),
                       reqType= context.JobDetail.JobDataMap.GetString(HttpJobDetailKey.ApiMethod),
                       apiAuthKey = context.JobDetail.JobDataMap.GetString(HttpJobDetailKey.ApiAuthKey),
                       apiAuthValue= context.JobDetail.JobDataMap.GetString(HttpJobDetailKey.ApiAuthValue);
                      
                HttpRequestMessage req=null;
                var client = _clientFactory.CreateClient();
                
                switch (reqType) {
                    case "get":
                        req = new HttpRequestMessage(HttpMethod.Get,httpAddr);
                        break;
                    case "post":
                        req = new HttpRequestMessage(HttpMethod.Post, httpAddr);
                        break;
                    case "put":
                        req = new HttpRequestMessage(HttpMethod.Put, httpAddr);
                        break;
                    case "delete":
                        req = new HttpRequestMessage(HttpMethod.Put, httpAddr);
                        break;
                    default:
                        break;

                
                }

                if (req == null)
                {
                    return;
                }

                if (!string.IsNullOrEmpty(apiAuthKey))
                {
                    req.Headers.Add(apiAuthKey,apiAuthValue);
                }


               var res= await client.SendAsync(req);
               var resStr = await res.Content.ReadAsStringAsync();
                await _jobService.Logger(new LoggerOPT() { 
                    JobGroup= context.JobDetail.Key.Group,
                    JobName=context.JobDetail.Key.Name,
                    TriggerName=trigger.Name,
                    TriggerGroup=trigger.Group,
                    Content=resStr,
                    Level=Quartz.Logging.LogLevel.Info.ToString(),
                    StartTime= dateTime,
                    EndTime=DateTime.Now,
                });


               Console.WriteLine(resStr);

             
            }
            catch (Exception ex)
            {
                httpMessage = ex.Message;
            }

            try
            {
                string logContent = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}_{dateTime.ToString("yyyy-MM-dd HH:mm:ss")}_{(string.IsNullOrEmpty(httpMessage) ? "OK" : httpMessage)}\r\n";
                //FileHelper.WriteFile(FileQuartz.LogPath + taskOptions.GroupName + "\\", $"{taskOptions.TaskName}.txt", logContent, true);
            }
            catch (Exception)
            {
                Console.WriteLine(triggerFullName + " " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:sss") + " " + httpMessage);
            }
           
            return;
        }
    }
}
