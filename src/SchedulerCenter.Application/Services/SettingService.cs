using SchedulerCenter.Core.DTO;
using SchedulerCenter.Core.Model;
using SchedulerCenter.Infrastructure.Dapper;
using SchedulerCenter.Infrastructure.QuartzNet;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerCenter.Application.Services
{
   public class SettingService
    {

        private QuartzProvider _provider;

        private DapperProvider _dapperProvider;
        public SettingService(QuartzProvider provider, DapperProvider dapperProvider)
        {
            _provider = provider;
            _dapperProvider = dapperProvider;
        }

        /// <summary>
        /// 获取节点列表
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<NodeDTO>> GetNodeList() {

           return _dapperProvider.QueryAsync<NodeDTO>("select  sched_name as 'SchedName',ID,Addr  from qrtz_nodes");
        }

        /// <summary>
        /// SaveOrUpdateNode 保存或者更新节点信息
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="schedName"></param>
        /// <returns></returns>
        public async Task<bool> SaveOrUpdateNode(string addr, string schedName) {


            var node = await _dapperProvider.QueryFirstOrDefaultAsync<NodeModel>("select * from qrtz_nodes where sched_name=@sched_name",new { sched_name=schedName });

            if (node != null)
            {
                //做更新
                return (await _dapperProvider.ExcuteAsync("update qrtz_nodes set sched_name=@sched_name,addr=@addr where id=@id", new
                {
                    sched_name = schedName,
                    addr = addr,
                    id=node.ID,
                })) > 0;
            }

         return  (await  _dapperProvider.ExcuteAsync("insert into qrtz_nodes(sched_name,addr)values(@sched_name,@addr)",new {
                sched_name=schedName,
                addr=addr,
            }))>0;

        }



    }
}
