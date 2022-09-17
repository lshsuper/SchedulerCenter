using SchedulerCenter.Core.Common;
using SchedulerCenter.Core.DTO;
using SchedulerCenter.Core.Interface;
using SchedulerCenter.Core.Model;
using SchedulerCenter.Core.Option;
using SchedulerCenter.Host.Models;
using SchedulerCenter.Infrastructure.Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerCenter.Application.Services
{
    public class JobBaseService : IJobService
    {


        private DapperProvider _dapperProvider;
        public JobBaseService(DapperProvider dapperProvider)
        {
           
            _dapperProvider = dapperProvider;
        }
        public virtual Task<ApiResult<bool>> AddJob(TaskOPT opt)
        {
            throw new NotImplementedException();
        }

        public virtual Task<IEnumerable<SchedulerDTO>> GetAllSchedulers()
        {
            throw new NotImplementedException();
        }

        public virtual async Task<ApiResult<IEnumerable<TaskLogDTO>>> GetJobLogPage(string taskName, string groupName, int page, int pageSize = 5)
        {


            int offset = (page - 1) * pageSize;
            string countSql = "select count(1) from qrtz_logs where job_name=@job_name and job_group=@job_group";
            int count = await _dapperProvider.FindAsync<int>(countSql, new { job_name = taskName, job_group = groupName });
            string dataSql = "select start_time 'StartTime',end_time 'Endtime',content 'Content' from qrtz_logs where job_name=@job_name and job_group=@job_group  order by id desc limit @offset,@limit";

            var data = await _dapperProvider.QueryAsync<LogModel>(dataSql, new { job_name = taskName, job_group = groupName, offset = offset, limit = pageSize });

            return ApiResult<IEnumerable<TaskLogDTO>>.OK(data.Select((s) => new TaskLogDTO
            {
                Msg = s.Content,
                BeginDate = s.StartTime.ToString("yyyy-MM-dd HH:mm:ss:ms"),
                EndDate = s.EndTime.ToString("yyyy-MM-dd HH:mm:ss:ms"),
            }));
        }


        public virtual Task<ApiResult<IEnumerable<TaskOPT>>> GetJobs(string schedulerName)
        {
            throw new NotImplementedException();
        }

        public virtual Task<IJobService> Init(string schedName)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<int> Logger(LoggerOPT opt)
        {
            return await _dapperProvider.ExcuteAsync("insert into qrtz_logs(start_time,end_time,job_name,job_group,trigger_name,trigger_group,content,level)values(@start_time,@end_time,@job_name,@job_group,@trigger_name,@trigger_group,@content,@level)", new
            {
                job_name = opt.JobName,
                job_group = opt.JobGroup,
                trigger_name = opt.TriggerName,
                trigger_group = opt.TriggerGroup,
                content = opt.Content,
                level = opt.Level.ToString(),
                start_time = opt.StartTime,
                end_time = opt.EndTime,
            });

        }

        public virtual Task<ApiResult<bool>> PauseJob(string schedulerName, string triggerName, string triggerGroup)
        {
            throw new NotImplementedException();
        }

        public virtual Task<ApiResult<bool>> RemoveJob(string schedulerName, string jobName, string jobGroup)
        {
            throw new NotImplementedException();
        }

        public virtual Task<ApiResult<bool>> RunJob(string schedulerName, string jobGroup, string jobName)
        {
            throw new NotImplementedException();
        }

        public virtual Task<ApiResult<bool>> StartJob(string schedulerName, string triggerName, string triggerGroup)
        {
            throw new NotImplementedException();
        }

        public virtual Task<ApiResult<bool>> UpdateJob(TaskOPT opt)
        {
            throw new NotImplementedException();
        }
    }
}
