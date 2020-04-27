using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var tencentMetting = new TencentMetting();
            //var update = tencentMetting.UpdateMeeting("17192087516727153565", "admin", "杨毓强","什么是股票", DateTime.Now, DateTime.Now.AddHours(1));
            //var query = tencentMetting.QueryMeeting("17192087516727153565", "admin");
             var join = tencentMetting.GetMeetingJoins("17192087516727153565", "admin");

            Console.WriteLine("修改");
        }
    }
}
