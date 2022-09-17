using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using SchedulerCenter.Host.Filters;
using SchedulerCenter.Infrastructure.Utility;
using SchedulerCenter.Infrastructure.QuartzNet;
using System;
using SchedulerCenter.Infrastructure.Dapper;
using SchedulerCenter.Infrastructure.QuartzNet.OPT;
using SchedulerCenter.Application.Services;
using SchedulerCenter.Infrastructure.Extensions;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;
using SchedulerCenter.Core.Common;
using SchedulerCenter.Host.Attributes;
using Microsoft.AspNetCore.Mvc.Controllers;
using IGeekFan.AspNetCore.Knife4jUI;
using SchedulerCenter.Core.Interface;
using SchedulerCenter.Application.factorys;
using SchedulerCenter.Core.Contant;

namespace SchedulerCenter.Host
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            configuration = configuration;

        }


        public IConfiguration _configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            var appSetting = _configuration.Get<AppSetting>();

            services.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

            }).AddCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.LoginPath = new PathString("/Home/Index");
                options.ClaimsIssuer = CookieAuthenticationDefaults.AuthenticationScheme;
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromSeconds(60);
                options.Cookie.Name = "SC-SESSION";


            }).AddJwtBearer(option =>
            {
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = appSetting.JwtConfig.Issuer,
                    ValidAudience = appSetting.JwtConfig.Audience,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSetting.JwtConfig.Secret)),
                    //缓冲过期时间，总的有效时间等于这个时间加上jwt的过期时间
                    ClockSkew = TimeSpan.FromSeconds(0),



                };

                option.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents()
                {

                    OnMessageReceived = (ctx) =>
                    {


                        ctx.Token = ctx.Request.Headers[AppKey.JwtTokenKey];

                        return Task.CompletedTask;

                    }

                };

            }); ;

            services.AddControllers()
              .AddNewtonsoftJson(op =>
              {
                  op.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                  op.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
              });
            services.AddMvc(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                                .RequireAuthenticatedUser()
                                .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
                options.Filters.Add(typeof(TaskAuthorizeFilter));
            }).AddRazorRuntimeCompilation();
          
            services.AddHttpContextAccessor();
            services.AddHttpClient();
            services.AddSession().AddMemoryCache();
            services.AddTransient<SettingService>();
            services.AddTransient<JobServiceFactory>();
            services.AddTransient<IJobService, JobRemoteService>();
                  services.AddTransient<IJobService, JobService>();
          
            

            services.AddQuartz(new InitConfig
            {

                ConnectionString = appSetting.DbConnStr,
                DbProviderName = appSetting.DbProvider,
                SchedulerName = appSetting.SchedulerName

            });

            services.AddDapper(() =>
            {
                var _quartzProvider = services.BuildServiceProvider().GetRequiredService<QuartzProvider>();
                return _quartzProvider.GetDbProvider().CreateConnection();
            });

         

            services.AddSwaggerGen(c =>
            {
                var assemblyName = Assembly.GetExecutingAssembly().GetName();
                var lastBuildTime = File.GetLastWriteTime(GetType().Assembly.Location);
                var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();


                typeof(SwaggerApiGroupName).GetFields().Skip(1).ToList().ForEach(f =>
                {
                    //获取枚举值上的特性
                    var info = f.GetCustomAttributes(typeof(SwaggerGroupInfoAttribute), false).OfType<SwaggerGroupInfoAttribute>().FirstOrDefault();
                    c.SwaggerDoc(f.Name, new OpenApiInfo
                    {
                        Title = info.Title,
                        Version = info.Version,
                        Description = $"{info.Description} build at:{lastBuildTime}"
                    });
                });



                c.DocInclusionPredicate((docName, apiDescription) =>
                {
                    //反射拿到值
                    var actionlist = apiDescription.ActionDescriptor.EndpointMetadata.Where(x => x is SwaggerApiGroupAttribute);
                    if (actionlist.Count() > 0)
                    {
                        //判断是否包含这个分组
                        var actionfilter = actionlist.FirstOrDefault() as SwaggerApiGroupAttribute;
                        return actionfilter.GroupNames.Count(x => x.ToString() == docName) > 0;
                    }
                    return false;
                });

                c.AddServer(new OpenApiServer()
                {
                    Url = "",
                    Description = "vvv"
                });
                c.CustomOperationIds(apiDesc =>
                {
                    var controllerAction = apiDesc.ActionDescriptor as ControllerActionDescriptor;
                    return controllerAction.ControllerName + "-" + controllerAction.ActionName;
                });
                //c.AddSecurityDefinition("SC-TOKEN", new OpenApiSecurityScheme
                //{
                //    Name = "SC-TOKEN",
                //    Type = SecuritySchemeType.ApiKey,
                //    In = ParameterLocation.Header,
                //    Description = "JWT Authorization header using the Bearer scheme."
                //});
                //c.AddSecurityRequirement(new OpenApiSecurityRequirement
                //{
                //    {
                //        new OpenApiSecurityScheme
                //        {
                //            Reference = new OpenApiReference
                //            {
                //                Type = ReferenceType.SecurityScheme,
                //                 Id="SC-TOKEN"
                //            }
                //        },
                //        new string[] { }
                //    }
                //});
                var apiXmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var apiXmlPath = Path.Combine(AppContext.BaseDirectory, apiXmlFile);
                var coreXmlFile = $"{Assembly.Load("SchedulerCenter.Core").GetName().Name}.xml";
                var coreXmlPath = Path.Combine(AppContext.BaseDirectory, coreXmlFile);
                c.IncludeXmlComments(apiXmlPath);
                c.IncludeXmlComments(coreXmlPath);


            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);






        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            var appSetting = _configuration.Get<AppSetting>();
            //启动时注册节点
            var settingService = app.ApplicationServices.GetRequiredService<SettingService>();
            settingService.SaveOrUpdateNode(appSetting.SchedulerHost, appSetting.SchedulerName).GetAwaiter();

            ServiceLocator.Init(app.ApplicationServices);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }


            app.UseSwagger();
           
            app.UseKnife4UI(c =>
            {

                typeof(SwaggerApiGroupName).GetFields().Skip(1).ToList().ForEach(f =>
                {
                    //获取枚举值上的特性
                    var info = f.GetCustomAttributes(typeof(SwaggerGroupInfoAttribute), false).OfType<SwaggerGroupInfoAttribute>().FirstOrDefault();
                    c.SwaggerEndpoint($"/{f.Name}/swagger.json", f.Name);
                    c.RoutePrefix = "api-doc";

                });


            });

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            }).UseStaticHttpContext();


          
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapSwagger("/api-doc/{documentName}/swagger.json");

            });



        }
    }
}
