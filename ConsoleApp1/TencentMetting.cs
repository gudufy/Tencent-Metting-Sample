using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace ConsoleApp1
{
    /// <summary>
    /// 腾讯会议
    /// </summary>
    public class TencentMetting
    {
        private string SecretId = "BhiSJueYm6GxcCzfjM32naRkEdK0qIUObFTg";
        private string SecretKey = "V9mPuNel1zJd76f8MGRgDnBosqUkKtQH";
        private string AppId = "200000461";

        /// <summary>
        /// 开通会议室
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="host"></param>
        /// <param name="subject"></param>
        /// <param name="startDateTime"></param>
        /// <param name="endDateTime"></param>
        /// <returns></returns>
        public MeetingResult OpenMeeting(string userid, string host, string subject, DateTime startDateTime, DateTime endDateTime)
        {
            var Nonce = new Random().Next(Int32.MaxValue).ToString();
            var Timestamp = GetTimeStamp10(DateTime.Now).ToString();
            var hosts = new List<Dictionary<string, string>>();
            if (!string.IsNullOrEmpty(host))
            {
                var hostItem = new Dictionary<string, string>();
                hostItem.Add("userid", host);

                hosts.Add(hostItem);
            }

            var req_body = new
            {
                userid,
                instanceid = 1,
                subject,
                type = 0,
                start_time = GetTimeStamp10(startDateTime).ToString(),
                end_time = GetTimeStamp10(endDateTime).ToString(),
                hosts
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

            return JsonConvert.DeserializeObject<MeetingResult>(response);
        }

        /// <summary>
        /// 修改会议
        /// </summary>
        /// <param name="meetingId"></param>
        /// <param name="userid"></param>
        /// <param name="host"></param>
        /// <param name="subject"></param>
        /// <param name="startDateTime"></param>
        /// <param name="endDateTime"></param>
        /// <returns></returns>
        public MeetingResult UpdateMeeting(string meetingId,string userid, string host, string subject, DateTime startDateTime, DateTime endDateTime)
        {
            var Nonce = new Random().Next(Int32.MaxValue).ToString();
            var Timestamp = GetTimeStamp10(DateTime.Now).ToString();
            var hosts = new List<Dictionary<string, string>>();
            if (!string.IsNullOrEmpty(host))
            {
                var hostItem = new Dictionary<string, string>();
                hostItem.Add("userid", host);

                hosts.Add(hostItem);
            }

            var req_body = new
            {
                userid,
                instanceid = 1,
                subject,
                type = 0,
                start_time = GetTimeStamp10(startDateTime).ToString(),
                end_time = GetTimeStamp10(endDateTime).ToString(),
                hosts
            };
            var req_bodyString = JsonConvert.SerializeObject(req_body);
            var url = "/v1/meetings/" + meetingId;

            var headerString = $"X-TC-Key={SecretId}&X-TC-Nonce={Nonce}&X-TC-Timestamp={Timestamp}";
            var stringToSign = "PUT" + "\n" +
               headerString + "\n" +
               url + "\n" +
               req_bodyString;

            var sign = Sign(stringToSign, SecretKey);

            SortedDictionary<string, string> head = new SortedDictionary<string, string>(StringComparer.Ordinal);
            head["X-TC-Key"] = SecretId;
            head["X-TC-Nonce"] = Nonce;
            head["X-TC-Timestamp"] = Timestamp;
            head["X-TC-Signature"] = sign;
            head["AppId"] = AppId;

            var response = PostMoths("https://api.meeting.qq.com"+ url, req_bodyString, head,"PUT");

            return JsonConvert.DeserializeObject<MeetingResult>(response);
        }

        /// <summary>
        /// 取消会议
        /// </summary>
        /// <param name="meetingId"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public string CancelMeeting(string meetingId, string userid)
        {
            var Nonce = new Random().Next(Int32.MaxValue).ToString();
            var Timestamp = GetTimeStamp10(DateTime.Now).ToString();

            var req_body = new
            {
                userid,
                instanceid = 1,
                reason_code = 0
            };
            var req_bodyString = JsonConvert.SerializeObject(req_body);
            var url = "/v1/meetings/" + meetingId + "/cancel";

            var headerString = $"X-TC-Key={SecretId}&X-TC-Nonce={Nonce}&X-TC-Timestamp={Timestamp}";
            var stringToSign = "POST" + "\n" +
               headerString + "\n" +
               url + "\n" +
               req_bodyString;

            var sign = Sign(stringToSign, SecretKey);

            SortedDictionary<string, string> head = new SortedDictionary<string, string>(StringComparer.Ordinal);
            head["X-TC-Key"] = SecretId;
            head["X-TC-Nonce"] = Nonce;
            head["X-TC-Timestamp"] = Timestamp;
            head["X-TC-Signature"] = sign;
            head["AppId"] = AppId;

            var response = PostMoths("https://api.meeting.qq.com"+ url, req_bodyString, head);

            return response;
        }

        /// <summary>
        /// 查询会议
        /// </summary>
        /// <param name="meetingId"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public MeetingResult QueryMeeting(string meetingId, string userid)
        {
            var Nonce = new Random().Next(Int32.MaxValue).ToString();
            var Timestamp = GetTimeStamp10(DateTime.Now).ToString();

            var url = "/v1/meetings/" + meetingId + "?userid=" + userid + "&instanceid=1";

            var headerString = $"X-TC-Key={SecretId}&X-TC-Nonce={Nonce}&X-TC-Timestamp={Timestamp}";
            var stringToSign = "GET" + "\n" +
               headerString + "\n" +
               url + "\n" +
               "";

            var sign = Sign(stringToSign, SecretKey);

            SortedDictionary<string, string> head = new SortedDictionary<string, string>(StringComparer.Ordinal);
            head["X-TC-Key"] = SecretId;
            head["X-TC-Nonce"] = Nonce;
            head["X-TC-Timestamp"] = Timestamp;
            head["X-TC-Signature"] = sign;
            head["AppId"] = AppId;

            var response = GetMoths("https://api.meeting.qq.com"+ url, head);

            return JsonConvert.DeserializeObject<MeetingResult>(response);
        }

        public MeetingJoin GetMeetingJoins(string meetingId, string userid)
        {
            var Nonce = new Random().Next(Int32.MaxValue).ToString();
            var Timestamp = GetTimeStamp10(DateTime.Now).ToString();

            var url = "/v1/meetings/"+ meetingId +"/participants?userid="+userid;

            var headerString = $"X-TC-Key={SecretId}&X-TC-Nonce={Nonce}&X-TC-Timestamp={Timestamp}";
            var stringToSign = "GET" + "\n" +
               headerString + "\n" +
               url + "\n" +
               "";

            var sign = Sign(stringToSign, SecretKey);

            SortedDictionary<string, string> head = new SortedDictionary<string, string>(StringComparer.Ordinal);
            head["X-TC-Key"] = SecretId;
            head["X-TC-Nonce"] = Nonce;
            head["X-TC-Timestamp"] = Timestamp;
            head["X-TC-Signature"] = sign;
            head["AppId"] = AppId;

            var response = GetMoths("https://api.meeting.qq.com" + url, head);

            return JsonConvert.DeserializeObject<MeetingJoin>(response);
        }

        /// <summary>
        /// 获得10位时间戳
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public int GetTimeStamp10(DateTime dt)
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
        public string Sign(string message, string secret)
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
        public static string PostMoths(string url, string postString, SortedDictionary<string, string> dic = null,string method = "POST")
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
                byte[] responseData = webClient.UploadData(url, method, postData);
                response = Encoding.UTF8.GetString(responseData);
                return response;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static string GetMoths(string url, SortedDictionary<string, string> dic = null)
        {

            WebClient webClient = new WebClient();
            string response = string.Empty;
            webClient.Headers.Add("Content-Type", "application/json");
            foreach (var item in dic)
            {
                webClient.Headers.Add(item.Key, item.Value);
            }
            try
            {
                byte[] responseData = webClient.DownloadData(url);
                response = Encoding.UTF8.GetString(responseData);
                return response;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }

    public class MeetingResult
    {
        public int meeting_number { get; set; }
        public List<meeting_info> meeting_info_list { get; set; }
    }

    public class meeting_info
    {
        public string meeting_id { get; set; }
        public string join_url { get; set; }
    }

    public class MeetingJoin : meeting_info
    {
        public List<Participant> participants { get; set; }
    }

    public class Participant
    {
        public string userid { get; set; }
        public string join_time { get; set; }
        public string left_time { get; set; }
    }
}
