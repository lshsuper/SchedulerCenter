using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using SchedulerCenter.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace SchedulerCenter.Host.Controllers
{
    public class HomeController : Controller
    {
        private IConfiguration _configuration { get; set; }
        private IMemoryCache _memoryCache { get; set; }
        public HomeController(IConfiguration configuration, IMemoryCache memoryCache)
        {
            this._configuration = configuration;
            this._memoryCache = memoryCache;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {

            var curUser=HttpContext.User;
            if (curUser != null && curUser.Identity.IsAuthenticated) {

                return Redirect("/TaskBackGround/Index");

            }
            if (!string.IsNullOrEmpty(HttpContext.Request("ReturnUrl")))
            {
                return new ContentResult
                {
                    ContentType = "text/html",
                    Content = string.Format("<script language='javaScript' type='text/javaScript'> window.parent.location.href = '/Home/Index';</script>")
                };
            }
            //string msg = _memoryCache.Get("msg")?.ToString();
            //if (msg != null)
            //{
            //    ViewBag.msg = msg;
            //    _memoryCache.Remove("msg");
            //}
            return View();
        }

        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<IActionResult> ValidateAuthor(string token)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            string _token = _configuration["token"];
            string superToken = _configuration["superToken"];
            if (!string.IsNullOrEmpty(token) && (token == _token || token == superToken))
            {
               
                ClaimsIdentity claimIdentity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                claimIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, token));
                await HttpContext.SignInAsync(new ClaimsPrincipal(claimIdentity), new AuthenticationProperties()
                {
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60)
                });
                return new RedirectResult("/TaskBackGround/index");
            }
            ViewBag.msg = "授权失败";
            return View("~/Views/Home/Index.cshtml");


        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return new RedirectResult("/");
        }

        //public IActionResult Guide()
        //{
        //    return View("~/Views/Home/Guide.cshtml");
        //}
        //public IActionResult Help()
        //{
        //    return View("~/Views/Home/Help.cshtml");
        //}

      
    }
}
