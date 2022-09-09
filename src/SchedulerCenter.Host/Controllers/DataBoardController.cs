using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchedulerCenter.Host.Controllers
{
    public class DataBoardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
