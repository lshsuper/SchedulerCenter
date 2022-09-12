using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SchedulerCenter.Core.Common;
using SchedulerCenter.Host.Attributes;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerCenter.Host.Controllers
{


    /// <summary>
    /// 对外API
    /// </summary>
   [Route("api/[controller]/[action]"), ApiController, Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OpenApiController : ControllerBase
    {


        private readonly IConfiguration _configuration;
        /// <summary>
        /// 开发API-构造器
        /// </summary>
        /// <param name="configuration"></param>
        public OpenApiController(IConfiguration configuration) {
            _configuration = configuration;
        }

        #region +Open-API-Common
        /// <summary>
        ///获取SC-TOKEN
        /// </summary>
        /// <param name="ticket">票据码</param>
        /// <returns></returns>
        [SwaggerApiGroup(SwaggerApiGroupName.Common), HttpGet, AllowAnonymous]
        public string AuthToken(string ticket)
        {

            var jwtConfig = _configuration.GetSection("Jwt").Get<JWTConfig>();

            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim("Ticket", ticket));
            var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Secret)), SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: jwtConfig.Issuer,
                audience: jwtConfig.Audience,
                claims: claims,
                expires: DateTime.Now.AddSeconds(jwtConfig.Expire),
                signingCredentials: creds
                );
            var t = new JwtSecurityTokenHandler().WriteToken(token);

            return t;
        }
        #endregion

        #region +Open-API-Other
        /// <summary>
        /// 测试
        /// </summary>
        /// <returns></returns>
        [SwaggerApiGroup(SwaggerApiGroupName.Other), HttpGet]
        public string Test()
        {

            return "ok";

        }
        #endregion

        #region +Open-API-Job

        #endregion

    }
}
