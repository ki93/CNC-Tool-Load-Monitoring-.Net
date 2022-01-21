using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace HNInc.Communication.Library
{
    public class HNHttp
    {
        string _wasUrl;
        string _accountDBUrl;
        string _accountDB;
        // 생성자 필요
        public HNHttp(string wasUrl, string accountDBUrl, string accountDB)
        {
            _wasUrl = wasUrl;
            _accountDBUrl = accountDBUrl;
            _accountDB = accountDB;
        }
        public HttpDeiviceHealthCheck GetDeiviceHealthCheck(HttpOPCode oPCode)
        {
            HttpDeiviceHealthCheck deiviceHealthCheck = new HttpDeiviceHealthCheck();
            // Create a request for the URL.
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"{_wasUrl}product/device-health-check?");
            stringBuilder.AppendFormat("opcode={0}", oPCode.ToString().Replace("_", "-"));
            string URI = stringBuilder.ToString();
            WebRequest request = (HttpWebRequest)WebRequest.Create(URI);
            // Get the response.
            // 409??
            WebResponse response;
            try
            {
                using (response = (HttpWebResponse)request.GetResponse())
                {
                    // Get the stream containing content returned by the server.
                    // The using block ensures the stream is automatically closed.
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        // Open the stream using a StreamReader for easy access.
                        StreamReader reader = new StreamReader(dataStream);
                        string getResponse = reader.ReadToEnd();
                        deiviceHealthCheck = JsonSerializer.Deserialize<HttpDeiviceHealthCheck>(getResponse);
                        deiviceHealthCheck._requestResult = "Success";
                    }
                    // Close the response.
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("409"))
                {
                    deiviceHealthCheck._status = "Deivice Doesn't Works";
                    deiviceHealthCheck._requestResult = e.Message;
                }
                else
                {
                    deiviceHealthCheck._status = "Error";
                    deiviceHealthCheck._requestResult = e.Message;
                }
            }
            return deiviceHealthCheck;
        }
        public List<HttpProductCounts> GetProductCountsList(DateTime startTime, DateTime endTime, HttpOPCode oPCode, HttpClassification classification)
        {
            HttpProductCounts productCounts = null;
            List<HttpProductCounts> productCountList = new List<HttpProductCounts>();

            string startDate = startTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ssZ");
            string endDate = endTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ssZ");
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"{_wasUrl}product/counts?");
            stringBuilder.AppendFormat("startDate={0}", startDate);
            stringBuilder.AppendFormat("&endDate={0}", endDate);
            stringBuilder.AppendFormat("&opcode={0}", oPCode.ToString().Replace("_", "-"));
            stringBuilder.AppendFormat("&classification={0}", classification.ToString());
            string URI = stringBuilder.ToString();
            WebRequest request = (HttpWebRequest)WebRequest.Create(URI);
            WebResponse response;
            try
            {
                using (response = (HttpWebResponse)request.GetResponse())
                {
                    // Get the stream containing content returned by the server.
                    // The using block ensures the stream is automatically closed.
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        // Open the stream using a StreamReader for easy access.
                        StreamReader reader = new StreamReader(dataStream);
                        while (!reader.EndOfStream)
                        {
                            string responseData = reader.ReadLine();
                            string[] responseDataArray = responseData.Split(new string[] { "{", "}", ": ", "\"", ", ", "\\n" }, StringSplitOptions.RemoveEmptyEntries);
                            string date = responseDataArray[1];
                            int count = Int32.Parse(responseDataArray[3]);
                            string requestResult = "Success";
                            productCounts = new HttpProductCounts(date, count, requestResult);
                            productCountList.Add(productCounts);
                        }
                    }
                    // Close the response.
                }
            }
            catch (Exception e)
            {
                productCounts = new HttpProductCounts("Error", -1, e.Message);
                productCountList.Add(productCounts);
            }
            return productCountList;
        }
        public  List<HttpCycleTime> GetCycleTimeList(HttpOPCode oPCode, int count)
        {
            HttpCycleTime cycleTime = null;
            List<HttpCycleTime> cycleTimes = new List<HttpCycleTime>();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"{_wasUrl}product/cycle-time?");
            stringBuilder.AppendFormat("opcode={0}", oPCode.ToString().Replace("_", "-"));
            stringBuilder.AppendFormat("&count={0}", count);
            string URI = stringBuilder.ToString();
            WebRequest request = (HttpWebRequest)WebRequest.Create(URI);
            // Get the response.
            WebResponse response;
            try
            {
                using (response = (HttpWebResponse)request.GetResponse())
                {
                    // Get the stream containing content returned by the server.
                    // The using block ensures the stream is automatically closed.
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        // Open the stream using a StreamReader for easy access.
                        StreamReader reader = new StreamReader(dataStream);
                        string getResponse = reader.ReadToEnd();
                        //Console.WriteLine(getResponse);
                        cycleTimes = JsonSerializer.Deserialize<List<HttpCycleTime>>(getResponse);
                    }
                    // Close the response.
                }
            }
            catch (Exception e)
            {
                cycleTime = new HttpCycleTime("Error", -1, "Error", "Error", "Error", e.Message);
                cycleTimes.Add(cycleTime);
            }
            return cycleTimes;
        }
        public HttpCycleTimeAverage GetCycleTimeAverage(int count)
        {
            HttpCycleTimeAverage cycleTimeAverage = null;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"{_wasUrl}product/CTAvg?");
            stringBuilder.AppendFormat("count={0}", count);
            string URI = stringBuilder.ToString();
            WebRequest request = (HttpWebRequest)WebRequest.Create(URI);
            WebResponse response;
            try
            {
                using(response = (HttpWebResponse)request.GetResponse())
                {
                    // Get the stream containing content returned by the server.
                    // The using block ensures the stream is automatically closed.
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        // Open the stream using a StreamReader for easy access.
                        StreamReader reader = new StreamReader(dataStream);
                        string getResponse = reader.ReadToEnd();
                        cycleTimeAverage = JsonSerializer.Deserialize<HttpCycleTimeAverage>(getResponse);
                        cycleTimeAverage._requestResult = "Success";
                    }
                    // Close the response.
                }
            }
            catch (Exception e)
            {
                cycleTimeAverage = new HttpCycleTimeAverage(-1, e.Message);
            }
            return cycleTimeAverage;
        }
        public List<HttpCycleInformaiton> GetCycleInformationList(HttpOPCode oPCode, string serialNumber)
        {
            HttpCycleInformaiton cycleInformaiton = null;
            List<HttpCycleInformaiton> cycleInformaitons = new List<HttpCycleInformaiton>();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"{_wasUrl}product/a-cycle?");
            stringBuilder.AppendFormat("opcode={0}", oPCode.ToString().Replace("_", "-"));
            stringBuilder.AppendFormat("&sn={0}", serialNumber);
            string URI = stringBuilder.ToString();
            WebRequest request = (HttpWebRequest)WebRequest.Create(URI);
            WebResponse response;
            try
            {
                using (response = (HttpWebResponse)request.GetResponse())
                {
                    // Get the stream containing content returned by the server.
                    // The using block ensures the stream is automatically closed.
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        // Open the stream using a StreamReader for easy access.
                        StreamReader reader = new StreamReader(dataStream);
                        string getResponse = reader.ReadToEnd();
                        //split try catch 설정
                        string[] receiveDataArray = getResponse.ToString().Split(new string[] { "{", "}", "[{", "}]", "[", "]", "\":\"", "\"", ",", "\\n" }, StringSplitOptions.RemoveEmptyEntries);
                        // 배열 null 혹은 개수 체크
                        if (receiveDataArray != null)
                        {
                            for (int i = 0; i < receiveDataArray.Length; i = i + 8)
                            {
                                string time = receiveDataArray[i + 1];
                                string scalePredict = receiveDataArray[i + 3].Replace(":", "");
                                string scaleLoad = receiveDataArray[i + 5].Replace(":", "");
                                string mae = receiveDataArray[i + 7].Replace(":", "");
                                string requestResult = "Success";
                                cycleInformaiton = new HttpCycleInformaiton(time, scalePredict, scaleLoad, mae, requestResult);
                                cycleInformaitons.Add(cycleInformaiton);
                            }
                        }
                    }
                    // Close the response.
                }
            }
            catch (Exception e)
            {
                cycleInformaiton = new HttpCycleInformaiton("Error", "Error", "Error", "Error", e.Message);
                cycleInformaitons.Add(cycleInformaiton);
            }
            return cycleInformaitons;
        }
        public HttpRealTimeCount GetRealTimeCount(HttpOPCode oPCode)
        {
            HttpRealTimeCount realTimeCount = null;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"{_wasUrl}product/RTcnt?");
            stringBuilder.AppendFormat("&opcode={0}", oPCode.ToString().Replace("_", "-"));
            string URI = stringBuilder.ToString();
            WebRequest request = (HttpWebRequest)WebRequest.Create(URI);
            WebResponse response;
            try
            {
                using (response = (HttpWebResponse)request.GetResponse())
                {
                    // Get the stream containing content returned by the server.
                    // The using block ensures the stream is automatically closed.
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        // Open the stream using a StreamReader for easy access.
                        StreamReader reader = new StreamReader(dataStream);
                        string getResponse = reader.ReadToEnd();
                        //Console.WriteLine(getResponse);
                        realTimeCount = JsonSerializer.Deserialize<HttpRealTimeCount>(getResponse);
                        realTimeCount._requestResult = "Success";
                    }
                    // Close the response.
                }
            }
            catch (Exception e)
            {
                realTimeCount = new HttpRealTimeCount(-1, e.Message);
            }
            return realTimeCount;
        }
        public List<HttpSpindleLoad> GetSpindleLoadList(DateTime startTime, DateTime endTime, HttpOPCode oPCode, string groupBy)
        {
            HttpSpindleLoad spindleLoad = null;
            List<HttpSpindleLoad> spindleLoads = new List<HttpSpindleLoad>();

            string startDate = startTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ssZ");
            string endDate = endTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ssZ");
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"{_wasUrl}product/spindle-load?");
            stringBuilder.AppendFormat("startDate={0}", startDate);
            stringBuilder.AppendFormat("&endDate={0}", endDate);
            stringBuilder.AppendFormat("&opcode={0}", oPCode.ToString().Replace("_", "-"));
            stringBuilder.AppendFormat("&groupBy={0}", groupBy);
            string URI = stringBuilder.ToString();
            WebRequest request = (HttpWebRequest)WebRequest.Create(URI);
            WebResponse response;
            try
            {
                using (response = (HttpWebResponse)request.GetResponse())
                {
                    // Get the stream containing content returned by the server.
                    // The using block ensures the stream is automatically closed.
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        // Open the stream using a StreamReader for easy access.
                        StreamReader reader = new StreamReader(dataStream);
                        string getResponse = reader.ReadToEnd();
                        //split try catch 설정
                        string[] receiveDataArray = getResponse.ToString().Split(new string[] { "{", "}", "[{", "}]", "[", "]", "\":\"", "\"", ",", "\\n" }, StringSplitOptions.RemoveEmptyEntries);
                        // 배열 null 혹은 개수 체크
                        if (receiveDataArray != null)
                        {
                            for (int i = 0; i < receiveDataArray.Length; i = i + 8)
                            {
                                string time = receiveDataArray[i + 1];
                                string meanScalePredict = receiveDataArray[i + 3].Replace(":", "");
                                string meanScaleLoad = receiveDataArray[i + 5].Replace(":", "");
                                string meanMae = receiveDataArray[i + 7].Replace(":", "");
                                string requestResult = "Success";
                                spindleLoad = new HttpSpindleLoad(time, meanScalePredict, meanScaleLoad, meanMae, requestResult);
                                spindleLoads.Add(spindleLoad);
                            }
                        }
                    }
                    // Close the response.
                }
            }
            catch (Exception e)
            {
                spindleLoad = new HttpSpindleLoad("Error", "Error", "Error", "Error", e.Message);
                spindleLoads.Add(spindleLoad);
            }
            return spindleLoads;
        }
        public List<HttpProductInformation> GetProductInformationList(int days)
        {
            HttpProductInformation productInformation = null;
            List<HttpProductInformation> productInformations = new List<HttpProductInformation>();

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"{_wasUrl}quality/record?");
            stringBuilder.AppendFormat("base={0}", days);
            string URI = stringBuilder.ToString();
            WebRequest request = (HttpWebRequest)WebRequest.Create(URI);
            WebResponse response;
            try
            {
                using (response = (HttpWebResponse)request.GetResponse())
                {
                    // Get the stream containing content returned by the server.
                    // The using block ensures the stream is automatically closed.
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        // Open the stream using a StreamReader for easy access.
                        StreamReader reader = new StreamReader(dataStream);
                        string getResponse = reader.ReadToEnd();
                        //split try catch 설정
                        // 현재 오류 Mae가 비는 오류가 있음
                        //string[] receiveDataArray = getResponse.ToString().Split(new string[] { "{", "}", "[", "]", ":", "\"", ",", "\n","\\"}, StringSplitOptions.RemoveEmptyEntries);
                        //// 배열 null 혹은 개수 체크
                        //foreach (var item in receiveDataArray)
                        //{
                        //    Debug.WriteLine(item);
                        //}
                        //if (receiveDataArray != null)
                        //{
                        //    for (int i = 0; i < receiveDataArray.Length; i = i + 15)
                        //    {
                        //        string opCode = receiveDataArray[i + 2];
                        //        string serialNumber = receiveDataArray[i + 4];
                        //        string accuracy = receiveDataArray[i + 6];
                        //        string predict = Enum.GetName(typeof(HttpAbnormalProblems), Int32.Parse(receiveDataArray[i + 8]));
                        //        string startTime = receiveDataArray[i + 10];
                        //        string endTime = receiveDataArray[i + 12];
                        //        string mae = receiveDataArray[i + 14];
                        //        string requestResult = "Success";
                        //        if (receiveDataArray[i + 8].Equals("0") && mae.Equals("abnormal"))
                        //        {
                        //            predict = "Abnormal data exists. Please check...";
                        //        }
                        //        productInformation = new HttpProductInformation(opCode,serialNumber,accuracy,predict,startTime,endTime,mae,requestResult);
                        //        Debug.WriteLine(productInformation);
                        //        productInformations.Add(productInformation);
                        //    }
                        //}
                        //임시 방편
                        string[] receiveDataArray = getResponse.ToString().Split(new string[] { "{", "}", "[", "]", ":", "\"", ",", "\n", "\\","mae", "abnormal","normal" }, StringSplitOptions.RemoveEmptyEntries);
                        // 배열 null 혹은 개수 체크
                        if (receiveDataArray != null)
                        {
                            for (int i = 0; i < receiveDataArray.Length; i = i + 13)
                            {
                                string opCode = receiveDataArray[i + 2];
                                string serialNumber = receiveDataArray[i + 4];
                                string accuracy = receiveDataArray[i + 6];
                                string predict = Enum.GetName(typeof(HttpAbnormalProblems), Int32.Parse(receiveDataArray[i + 8]));
                                string startTime = receiveDataArray[i + 10];
                                string endTime = receiveDataArray[i + 12];
                                string requestResult = "Success";
                                productInformation = new HttpProductInformation(opCode, serialNumber, accuracy, predict, startTime, endTime, requestResult);
                                Debug.WriteLine(productInformation);
                                productInformations.Add(productInformation);
                            }
                        }
                    }
                    // Close the response.
                }
            }
            catch (Exception e)
            {
                productInformation = new HttpProductInformation("Error", "Error", "Error", "Error", "Error", "Error", "Error", e.Message);
                productInformations.Add(productInformation);
            }
            return productInformations;
        }
        public HttpQualityInformaiton GetQualityInformaiton()
        {
            HttpQualityInformaiton qualityInformaiton = null;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"{_wasUrl}quality/history?");
            string URI = stringBuilder.ToString();
            WebRequest request = (HttpWebRequest)WebRequest.Create(URI);
            WebResponse response;
            try
            {
                using (response = (HttpWebResponse)request.GetResponse())
                {
                    // Get the stream containing content returned by the server.
                    // The using block ensures the stream is automatically closed.
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        // Open the stream using a StreamReader for easy access.
                        StreamReader reader = new StreamReader(dataStream);
                        string getResponse = reader.ReadToEnd();
                        qualityInformaiton = JsonSerializer.Deserialize<HttpQualityInformaiton>(getResponse);
                        qualityInformaiton._predict = Enum.GetName(typeof(HttpAbnormalProblems), Int32.Parse(qualityInformaiton._predict));
                        qualityInformaiton._requestResult = "Success";
                    }
                    // Close the response.
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                qualityInformaiton = new HttpQualityInformaiton("Error", "Error", "Error", "Error", "Error", "Error", "Error", null, -1, e.Message);
            }
            return qualityInformaiton;
        }
        public HttpAuthentication CheckAuthentication(string id, string password)
        {
            //암호화
            byte[] passwordArray = Encoding.Default.GetBytes(password);
            string sha256Password = string.Empty;
            using (SHA256 sHA256 = SHA256.Create())
            {
                foreach (byte item in sHA256.ComputeHash(passwordArray))
                {
                    sha256Password += String.Format("{0:x2}", item);
                }
            }
            //Connection
            HttpAuthentication authentication = new HttpAuthentication();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(_accountDBUrl);
            stringBuilder.Append(_accountDB);
            string conn_string = stringBuilder.ToString();
            using(MySqlConnection mariaDBConnection = new MySqlConnection(conn_string))
            {
                MySqlCommand mariaDBCommand = mariaDBConnection.CreateCommand();
                string query = $"select `pw` from `cnc_wpf`.`user` where  `id`='{id}';";

                ////ID 비번 입력 부분
                //string query = $"INSERT INTO `cnc_wpf`.`user` (`ID`, `PW`) VALUES ('admin', '{sha256Password}');";
                mariaDBCommand.CommandText = query;
                MySqlDataReader mariaDBDataReader = null;
                string dbPassword = string.Empty;
                try
                {
                    mariaDBConnection.Open();
                    using(mariaDBDataReader = mariaDBCommand.ExecuteReader())
                    {
                        while (mariaDBDataReader.Read())
                        {
                            dbPassword = mariaDBDataReader["PW"].ToString();
                        }
                        if (sha256Password.Equals(dbPassword))
                        {
                            authentication._checkPassword = true;
                            authentication._processResult = "Success";
                        }
                        else
                        {
                            authentication._checkPassword = false;
                            authentication._processResult = "ID or Password Doesn't Match";
                        }
                    }
                }
                catch (Exception e)
                {
                    authentication._checkPassword = false;
                    authentication._processResult = $"Fail to Get Password..{e.Message}";
                }
            }
            return authentication;
        } 
    }
}

