using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchedulerCenter.Host.Attr;
using System;
using System.Net;
using System.Text;

namespace SchedulerCenter.Host.Controllers
{
    public class HealthController : Controller
    {
       
        public HealthController()
        {
           
        }
        /// <summary>
        /// 定时调用此接口让站点一直保持运行状态
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost, HttpGet]
        public IActionResult KeepAlive()
        {
            return Json(new { status = true });
        }

        [AllowAnonymous]
        [HttpPost, HttpGet]
        public IActionResult Index()
        {
            return Json(new { status = true, msg = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")});
        }

        [HttpPost, HttpGet]
        [TaskAuthorAttribute]
        public IActionResult Log(int pageSize=1)
        {
          return  new ContentResult()
            {
                Content = "",
                ContentType = "text/html",
                StatusCode = (int)HttpStatusCode.OK
            };
        }
    }
}