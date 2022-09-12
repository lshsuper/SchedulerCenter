using Microsoft.AspNetCore.Mvc;
using SchedulerCenter.Application.Services;
using SchedulerCenter.Host.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchedulerCenter.Host.Controllers
{
    [TaskAuthor]
    public class SettingController : Controller
    {

        private SettingService _settingService;
        public  SettingController(SettingService settingService)
        {
            _settingService = settingService;
        }
        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> GetNodes() {


            return Json(await _settingService.GetNodeList());

        }


    }
}
