using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchedulerCenter.Host.Attributes
{

    public class SwaggerGroupInfoAttribute : Attribute
    {
        public string Title { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
    }

    /// <summary>
    /// 系统分组枚举值
    /// </summary>
    public enum SwaggerApiGroupName
    {

        [SwaggerGroupInfo(Title = "All", Description = "Open-API开放接口", Version = "v1")]
        All,
        [SwaggerGroupInfo(Title = "Common", Description = "公共接口", Version = "v1")]
        Common,
        [SwaggerGroupInfo(Title = "Job", Description = "任务相关分组", Version = "v1")]
        Job,
        [SwaggerGroupInfo(Title = "Other", Description = "其它接口", Version = "v1")]
        Other
       
    
    }
    public class SwaggerApiGroupAttribute: Attribute
    {
        public SwaggerApiGroupAttribute(params SwaggerApiGroupName[] name)
        {
            GroupNames = name;
        }

        public SwaggerApiGroupName[] GroupNames { get; set; }

    }
}
