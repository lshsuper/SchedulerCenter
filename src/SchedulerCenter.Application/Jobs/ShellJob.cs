using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerCenter.Application.Jobs
{
    public class ShellJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {


                      var str = $"go run main.go";
                                    Process p = new Process();
                                   //设置要启动的应用程序
                      p.StartInfo.FileName = "bash";
                                    //是否使用操作系统shell启动
                         p.StartInfo.UseShellExecute = false;
                                    // 接受来自调用程序的输入信息
                        p.StartInfo.RedirectStandardInput = true;
                                    //输出信息
                       p.StartInfo.RedirectStandardOutput = true;
                                    // 输出错误
                        p.StartInfo.RedirectStandardError = false;
                                    //不显示程序窗口
                       p.StartInfo.CreateNoWindow = true;
           
                                    p.Start();
                                    p.StandardInput.WriteLine(str);
                                    p.StandardInput.Close();
                                    var fileData = p.StandardOutput.ReadToEnd();
                                    p.WaitForExit();
                                    p.Close();
                                

        
        }
    }
}
