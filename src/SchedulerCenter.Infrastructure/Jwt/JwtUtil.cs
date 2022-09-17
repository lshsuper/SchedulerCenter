using Microsoft.IdentityModel.Tokens;
using SchedulerCenter.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SchedulerCenter.Infrastructure.Jwt
{
   public class JwtUtil
    {

        public static  string GetToken(JwtConfig cnf,Dictionary<string,object>kMap)
        {
            List<Claim> claims = new List<Claim>();
            kMap.ForEach((ele,i)=> {
                claims.Add(new Claim(ele.Key, ele.Value.ToString()));
            });
           
            var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cnf.Secret)), SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: cnf.Issuer,
                audience: cnf.Audience,
                claims: claims,
                expires: DateTime.Now.AddSeconds(cnf.Expire),
                signingCredentials: creds
                );
            var t = new JwtSecurityTokenHandler().WriteToken(token);
            return t;
        }

    }
}
