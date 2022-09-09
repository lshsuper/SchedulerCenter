using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;


namespace SchedulerCenter.Infrastructure.Extensions
{
    public static class EnumExtension
    {


        public static string GetDescription(this System.Enum value, bool nameInstend = true)
         {
             Type type = value.GetType();
             string name = System.Enum.GetName(type, value);
             if (name==null)
             {                 return null;
            }
            FieldInfo field = type.GetField(name);             
            DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            if (attribute==null&&nameInstend==true)             {
                return name;
            }
             return attribute==null? null :attribute.Description;
         }
    }
}
