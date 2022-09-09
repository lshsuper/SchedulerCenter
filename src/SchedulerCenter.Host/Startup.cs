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
namespace SchedulerCenter.Host
{
    public class Startup
    {
         public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
           
        }

      
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

          


            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.LoginPath = new PathString("/Home/Index");
               
                options.ClaimsIssuer = CookieAuthenticationDefaults.AuthenticationScheme;
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
             
            });
            
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
            });
            
            services.AddHttpContextAccessor();
            services.AddHttpClient();
            services.AddSession().AddMemoryCache();
          
            services.AddSingleton<JobService>();
           
           
            services.AddQuartz(new InitConfig { 
            
                ConnectionString= Configuration.GetConnectionString("connStr"),
                DbProviderName= Configuration["DbProvider"]

            });

            services.AddDapper(()=> {
                var _quartzProvider = services.BuildServiceProvider().GetRequiredService<QuartzProvider>();
                return _quartzProvider.GetDbProvider().CreateConnection();
            });


        
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);


           



        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

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
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.UseQuartz().UseStaticHttpContext();
            app.UseStaticFiles();
            app.UseAuthentication();
            //app.UseMvc();
            app.UseRouting();
           
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=TaskBackGround}/{action=Index}/{id?}");
            });
        }
    }
}
