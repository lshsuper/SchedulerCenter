using Quartz.Impl;
using Quartz.Impl.AdoJobStore;
using Quartz.Impl.AdoJobStore.Common;
using Quartz.Impl.Matchers;
using Quartz.Impl.Triggers;
using Quartz.Simpl;
using Quartz.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Quartz;
using SchedulerCenter.Infrastructure.QuartzNet.OPT;
using System.Collections.Concurrent;
using Quartz.Core;
using Quartz.Spi;

namespace SchedulerCenter.Infrastructure.QuartzNet
{

 
    public class QuartzProvider
    {
        /// <summary>
        /// 数据连接
        /// </summary>
        private IDbProvider dbProvider;
        /// <summary>
        /// ADO 数据类型
        /// </summary>
        private string driverDelegateType;

        private DirectSchedulerFactory factory;

     


        public QuartzProvider() {


           

       
        }

        public IDbProvider GetDbProvider() {

            return dbProvider;

        
        }


        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public async Task Init(InitConfig conf) { 

            InitDbProvider(conf.DbProviderName, conf.ConnectionString);
            InitDriverDelegateType(conf.DbProviderName); 

            DBConnectionManager.Instance.AddConnectionProvider("default", dbProvider);
            CreateScheduler(conf.SchedulerName);
            var scheduler = await SchedulerRepository.Instance.Lookup(conf.SchedulerName);
            await scheduler.Start();

        }


        public Task<IReadOnlyList<IScheduler>> GetAllSchedulers() {

           return  factory.GetAllSchedulers();
        
        }


        public void CreateScheduler(string schedulerName)
        {

            var serializer = new JsonObjectSerializer();
            serializer.Initialize();
            var jobStore = new JobStoreTX
            {
                DataSource = "default",
                TablePrefix = "QRTZ_",
                InstanceId = "AUTO",
                InstanceName = schedulerName,
                DriverDelegateType = driverDelegateType,
                ObjectSerializer = serializer,
                ClusterCheckinInterval = TimeSpan.FromSeconds(20),
                Clustered = true,     //启用集群模式
                UseDBLocks = true,   //使用数据库锁

            };

            factory = DirectSchedulerFactory.Instance;
            factory.CreateScheduler(schedulerName, "AUTO", new DefaultThreadPool(), jobStore);

        }


        public Task<IScheduler> GetScheduler(string schedulerName) {

            return factory.GetScheduler(schedulerName);
        }
       
        public async Task PauseTrigger(string schedulerName, string triggerKey,string triggerGroup) {

            var scheduler = await GetScheduler(schedulerName);
            await scheduler.PauseTrigger(new TriggerKey(triggerKey, triggerGroup));
            return;
        }

     
        public async Task UnscheduleJob(string schedulerName, string triggerKey,string triggerGroup) {
            var  scheduler = await GetScheduler(schedulerName);
           
            await scheduler.UnscheduleJob(new TriggerKey(triggerKey, triggerGroup));
            return ;
        }

        public async Task DeleteJob(string schedulerName,string jobName,string jobGroup) {

            var scheduler = await GetScheduler(schedulerName);
            await scheduler.DeleteJob(JobKey.Create(jobName, jobGroup));
            return ;
        }

        public async Task Stop(string schedulerName, string jobName) {
            var scheduler = await GetScheduler(schedulerName);
            if (!scheduler.IsShutdown) {
                await  scheduler.Shutdown();
            }
            return;
        }


        public bool CheckCron(string cron)
        {

            CronTriggerImpl trigger = new CronTriggerImpl();
            trigger.CronExpressionString = cron;
            DateTimeOffset? date = trigger.ComputeFirstFireTimeUtc(null);
            return date != null;

        }

        public async Task<bool> JobExists(string schedulerName,string tName, string gName)
        {
            var scheduler = await GetScheduler(schedulerName);
            return await scheduler.CheckExists(JobKey.Create(tName, gName));
        }

        public async Task<bool> AddCronJob<T>(AddCronJobOPT opt) where T : IJob
        {
            var scheduler = await GetScheduler(opt.SchedulerName);
            IJobDetail job = JobBuilder.Create<T>().SetJobData(new JobDataMap(opt.JobData)).WithDescription(opt.Descr).WithIdentity(opt.JobName, opt.JobGroup)
                 .Build();
            ITrigger trigger = TriggerBuilder.Create().UsingJobData(new JobDataMap(opt.JobData))
               .WithIdentity(opt.TriggerName, opt.JobGroup)
               .StartNow()
               .WithDescription(opt.Descr)
               .WithCronSchedule(opt.Cron)
               .Build();

            await scheduler.ScheduleJob(job, trigger);
            return true;

        }


        public async Task<IJobDetail> GetJobDetail(string schedulerName, string jobGroup, string jobName) {

            var scheduler = await GetScheduler(schedulerName);
            return  await scheduler.GetJobDetail(JobKey.Create(jobName,jobGroup));


        }

        public async Task ResumeTrigger(string schedulerName,string triggerName,string triggerGroup) {

            var scheduler = await GetScheduler(schedulerName);
            await scheduler.ResumeTrigger(new TriggerKey(triggerName, triggerGroup));
            return;

        }


        public async Task<IReadOnlyCollection<string>> GetJobGroupNames(string schedulerName) {

            var scheduler = await GetScheduler(schedulerName);
            return  await scheduler.GetJobGroupNames();

        }


        public async Task<IReadOnlyCollection<JobKey>> GetJobKeys(string schedulerName, string jobGroup) {

            var scheduler = await GetScheduler(schedulerName);
            return await scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(jobGroup));

        }

        public async Task<IReadOnlyCollection<ITrigger>> GetTriggersOfJob(string schedulerName, string jobGroup,string jobName) {
            var scheduler = await GetScheduler(schedulerName);
            return await scheduler.GetTriggersOfJob(JobKey.Create(jobName, jobGroup));


        }

        public  async Task<TriggerState> GetTriggerState(string schedulerName, string triggerGroup, string triggerKey)
        {
            var scheduler = await GetScheduler(schedulerName);
            return  await scheduler.GetTriggerState(new TriggerKey(triggerKey,triggerGroup));

        }

        public async Task TriggerJob(string schedulerName, string jobGroup,string jobName)
        {
            var scheduler = await GetScheduler(schedulerName);
            await scheduler.TriggerJob(JobKey.Create(jobName, jobGroup));
            return;

        }



        #region private

        private void InitDbProvider(string dbProviderName, string connectionString) {


            dbProvider = new DbProvider(dbProviderName, connectionString);

        }
        /// <summary>
        /// 初始化数据库引擎
        /// </summary>
        /// <param name="dbProviderName"></param>
        private void InitDriverDelegateType(string dbProviderName)
        {
            switch (dbProviderName)
            {
                case "SQLite-Microsoft":
                case "SQLite":
                    driverDelegateType = typeof(SQLiteDelegate).AssemblyQualifiedName;
                    break;
                case "MySql":
                    driverDelegateType = typeof(MySQLDelegate).AssemblyQualifiedName;
                    break;
                case "OracleODPManaged":
                    driverDelegateType = typeof(OracleDelegate).AssemblyQualifiedName;
                    break;
                case "SqlServer":
                case "SQLServerMOT":
                    driverDelegateType = typeof(SqlServerDelegate).AssemblyQualifiedName;
                    break;
                case "Npgsql":
                    driverDelegateType = typeof(PostgreSQLDelegate).AssemblyQualifiedName;
                    break;
                case "Firebird":
                    driverDelegateType = typeof(FirebirdDelegate).AssemblyQualifiedName;
                    break;
                default:
                    throw new Exception("dbProviderName unreasonable");
            }
        }



        #endregion 



    }
}
