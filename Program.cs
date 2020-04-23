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
        //创建会议室
        static void Main(string[] args)
        {
            var SecretId = "your id";
            var SecretKey = "your key";
            var AppId = "your app id";

            var Nonce = new Random().Next(Int32.MaxValue).ToString();
            var Timestamp = GetTimeStamp10(DateTime.Now).ToString();

            var req_body = new
            {
                userid = "tester",
                instanceid = 1,
                subject = "tester's meeting",
                type = 0,
                start_time = GetTimeStamp10(DateTime.Now).ToString(),
                end_time = GetTimeStamp10(DateTime.Now.AddHours(1)).ToString()
            };
            var req_bodyString = JsonConvert.SerializeObject(req_body);

            var headerString = $"X-TC-Key={SecretId}&X-TC-Nonce={Nonce}&X-TC-Timestamp={Timestamp}";
            var stringToSign = "POST" + "\n" +
               headerString + "\n" +
               "/v1/meetings" + "\n" +
               req_bodyString;

            var sign = Sign(stringToSign, SecretKey);

            SortedDictionary<string, string> head = new SortedDictionary<string, string>(StringComparer.Ordinal);
            head["X-TC-Key"] = SecretId;
            head["X-TC-Nonce"] = Nonce;
            head["X-TC-Timestamp"] = Timestamp;
            head["X-TC-Signature"] = sign;
            head["AppId"] = AppId;


            var response = PostMoths("https://api.meeting.qq.com/v1/meetings", req_bodyString, head);


            Console.WriteLine(sign);
        }

        public static int GetTimeStamp10(DateTime dt)
        {
            DateTime dateStart = new DateTime(1970, 1, 1, 8, 0, 0);
            int timeStamp = Convert.ToInt32((dt - dateStart).TotalSeconds);
            return timeStamp;
        }

        /// <summary>
        /// HMACSHA256签名
        /// </summary>
        /// <param name="message"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        private static string Sign(string message, string secret)
        {
            var encoding = System.Text.Encoding.UTF8;
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashmessage.Length; i++)
                {
                    sb.Append(hashmessage[i].ToString("x2"));
                }
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(sb.ToString()));
            }
        }

        /// <summary>
        /// 发送post
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postString">json</param>
        /// <param name="dic">headers</param>
        /// <returns></returns>
        public static string PostMoths(string url, string postString, SortedDictionary<string, string> dic = null)
        {

            WebClient webClient = new WebClient();
            string response = string.Empty;
            webClient.Headers.Add("Content-Type", "application/json");
            foreach (var item in dic)
            {
                webClient.Headers.Add(item.Key, item.Value);
            }
            byte[] postData = Encoding.UTF8.GetBytes(postString);
            try
            {
                byte[] responseData = webClient.UploadData(url, "POST", postData);
                response = Encoding.UTF8.GetString(responseData);
                return response;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
