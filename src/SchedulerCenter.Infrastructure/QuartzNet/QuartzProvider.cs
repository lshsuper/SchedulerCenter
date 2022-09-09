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

namespace SchedulerCenter.Infrastructure.QuartzNet
{

 
    public class QuartzProvider
    {


        private  ConcurrentDictionary<string,IScheduler> schedulers;
        /// <summary>
        /// 数据连接
        /// </summary>
        private IDbProvider dbProvider;
        /// <summary>
        /// ADO 数据类型
        /// </summary>
        private string driverDelegateType;


        public QuartzProvider(InitConfig conf) {


            InitDbProvider(conf.DbProviderName, conf.ConnectionString);
            InitDriverDelegateType(conf.DbProviderName);
            schedulers = new ConcurrentDictionary<string, IScheduler>();


        }

        public IDbProvider GetDbProvider() {

            return dbProvider;

        
        }


        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public async Task Init(string []schedulerNames) {

            DBConnectionManager.Instance.AddConnectionProvider("default", dbProvider);
            var serializer = new JsonObjectSerializer();
            serializer.Initialize();
            var jobStore = new JobStoreTX
            {
                DataSource = "default",
                TablePrefix = "QRTZ_",
                InstanceId = "AUTO",
                DriverDelegateType = driverDelegateType,
                ObjectSerializer = serializer,
            };
            
            foreach (var schedulerName in schedulerNames)
            {
                DirectSchedulerFactory.Instance.CreateScheduler(schedulerName, "AUTO", new DefaultThreadPool(), jobStore);
                var scheduler = await SchedulerRepository.Instance.Lookup(schedulerName);
                schedulers.TryAdd<string,IScheduler>(schedulerName,scheduler);
            }

            
        }


        public Task Start(string schedulerName) {
            return schedulers[schedulerName].Start();
        }


       
        public Task PauseTrigger(string schedulerName,string triggerKey,string triggerGroup) {
            return schedulers[schedulerName].PauseTrigger(new TriggerKey(triggerKey, triggerGroup));
        }

     
        public Task UnscheduleJob(string schedulerName,string triggerKey,string triggerGroup) {
            return schedulers[schedulerName].UnscheduleJob(new TriggerKey(triggerKey, triggerGroup));
        }

        public Task DeleteJob(string schedulerName,string jobName,string jobGroup) {
            return schedulers[schedulerName].DeleteJob(JobKey.Create(jobName,jobGroup));
        }

        public Task Stop(string schedulerName) {

            return schedulers[schedulerName].Clear();
        
        }


        public bool CheckCron(string cron)
        {

            CronTriggerImpl trigger = new CronTriggerImpl();
            trigger.CronExpressionString = cron;
            DateTimeOffset? date = trigger.ComputeFirstFireTimeUtc(null);
            return date != null;

        }

        public Task<bool> JobExists(string schedulerName,string tName, string gName)
        {
            return schedulers[schedulerName].CheckExists(JobKey.Create(tName, gName));
        }

        public async Task<bool> AddCronJob<T>(string schedulerName,AddCronJobOPT opt) where T : IJob
        {

            IJobDetail job = JobBuilder.Create<T>().SetJobData(new JobDataMap(opt.JobData)).WithDescription(opt.Descr).WithIdentity(opt.JobName, opt.JobGroup)
                 .Build();
            ITrigger trigger = TriggerBuilder.Create().UsingJobData(new JobDataMap(opt.JobData))
               .WithIdentity(opt.TriggerName, opt.JobGroup)
               .StartNow()
               .WithDescription(opt.Descr)
               .WithCronSchedule(opt.Cron)
               .Build();

            await schedulers[schedulerName].ScheduleJob(job, trigger);
            return true;

        }


        public Task<IJobDetail> GetJobDetail(string schedulerName,string jobGroup, string jobName) {


           return schedulers[schedulerName].GetJobDetail(JobKey.Create(jobName,jobGroup));


        }

        public Task ResumeTrigger(string schedulerName, string triggerName,string triggerGroup) {


            return schedulers[schedulerName].ResumeTrigger(new TriggerKey(triggerName, triggerGroup));

        }


        public Task<IReadOnlyCollection<string>> GetJobGroupNames(string schedulerName) {


            return  schedulers[schedulerName].GetJobGroupNames();

        }


        public Task<IReadOnlyCollection<JobKey>> GetJobKeys(string schedulerName,string jobGroup) {


            return schedulers[schedulerName].GetJobKeys(GroupMatcher<JobKey>.GroupEquals(jobGroup));

        }

        public Task<IReadOnlyCollection<ITrigger>> GetTriggersOfJob(string schedulerName,string jobGroup,string jobName) {

            return schedulers[ schedulerName].GetTriggersOfJob(JobKey.Create(jobName, jobGroup));


        }

        public  Task<TriggerState> GetTriggerState(string schedulerName, string triggerGroup, string triggerKey)
        {
            return schedulers[schedulerName].GetTriggerState(new TriggerKey(triggerKey,triggerGroup));

        }

        public Task TriggerJob(string schedulerName, string jobGroup,string jobName)
        {
            return schedulers[schedulerName].TriggerJob(JobKey.Create(jobName,jobGroup));

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
