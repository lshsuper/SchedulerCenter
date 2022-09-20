using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SchedulerCenter.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SchedulerCenter.Infrastructure.Jwt
{
   public class JwtUtil<T> where T:class,new()
    {

        public static  string GetToken(JwtConfig cnf,T claim)
        {
            List<Claim> claims = new List<Claim>();

            claim.GetType().GetProperties().ForEach((ele, i) => {
              
                claims.Add(new Claim(ele.Name, ele.GetValue(claim).ToString()));
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

        public static T ParseToken(string token) {

            return JsonConvert.DeserializeObject<T>(new JwtSecurityToken(token).Payload.SerializeToJson());

        }

    }
}
