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

namespace SchedulerCenter.Infrastructure.QuartzNet
{

 
    public class QuartzProvider
    {


        private IScheduler scheduler;
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


        }

        public IDbProvider GetDbProvider() {

            return dbProvider;

        
        }


        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public async Task Init() {

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
            DirectSchedulerFactory.Instance.CreateScheduler("bennyScheduler", "AUTO", new DefaultThreadPool(), jobStore);
            scheduler = await SchedulerRepository.Instance.Lookup("bennyScheduler");

          

        }


        public Task Start() {

           return scheduler.Start();

        }


       
        public Task PauseTrigger(string triggerKey,string triggerGroup) {
            return scheduler.PauseTrigger(new TriggerKey(triggerKey, triggerGroup));
        }

     
        public Task UnscheduleJob(string triggerKey,string triggerGroup) {
            return scheduler.UnscheduleJob(new TriggerKey(triggerKey, triggerGroup));
        }

        public Task DeleteJob(string jobName,string jobGroup) {
            return scheduler.DeleteJob(JobKey.Create(jobName,jobGroup));
        }

        public Task Stop() {

            return scheduler.Clear();
        
        }


        public bool CheckCron(string cron)
        {

            CronTriggerImpl trigger = new CronTriggerImpl();
            trigger.CronExpressionString = cron;
            DateTimeOffset? date = trigger.ComputeFirstFireTimeUtc(null);
            return date != null;

        }

        public Task<bool> JobExists(string tName, string gName)
        {
            return scheduler.CheckExists(JobKey.Create(tName, gName));
        }

        public async Task<bool> AddCronJob<T>(AddCronJobOPT opt) where T : IJob
        {

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


        public Task<IJobDetail> GetJobDetail(string jobGroup, string jobName) {


           return scheduler.GetJobDetail(JobKey.Create(jobName,jobGroup));


        }

        public Task ResumeTrigger(string triggerName,string triggerGroup) {


            return scheduler.ResumeTrigger(new TriggerKey(triggerName, triggerGroup));

        }


        public Task<IReadOnlyCollection<string>> GetJobGroupNames() {


            return  scheduler.GetJobGroupNames();

        }


        public Task<IReadOnlyCollection<JobKey>> GetJobKeys(string jobGroup) {


            return scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(jobGroup));

        }

        public Task<IReadOnlyCollection<ITrigger>> GetTriggersOfJob(string jobGroup,string jobName) {

            return scheduler.GetTriggersOfJob(JobKey.Create(jobName, jobGroup));


        }

        public  Task<TriggerState> GetTriggerState(string triggerGroup, string triggerKey)
        {
            return scheduler.GetTriggerState(new TriggerKey(triggerKey,triggerGroup));

        }

        public Task TriggerJob(string jobGroup,string jobName)
        {
            return scheduler.TriggerJob(JobKey.Create(jobName,jobGroup));

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
