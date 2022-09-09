using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
namespace SchedulerCenter.Infrastructure.Dapper
{
    public class DapperProvider
    {

        private Func<IDbConnection> _connCall;
        public DapperProvider(Func<IDbConnection> connCall)
        {

            _connCall = connCall;
        }

        public Task<int> ExcuteAsync(string sql, object paras=null)
        {


            using (var conn = _connCall())
            {
                return conn.ExecuteAsync(sql, paras);
            }

        }

        public Task<IEnumerable<T>> QueryAsync<T>(string sql,object paras=null) {

            using (var conn = _connCall())
            {
                return conn.QueryAsync<T>(sql, paras);
            }

        }

        public Task<T> QueryFirstOrDefaultAsync<T>(string sql, object paras = null)
        {

            using (var conn = _connCall())
            {
                return conn.QueryFirstOrDefaultAsync<T>(sql, paras);
            }

        }


    }
}
