using HNInc.Communication.Library.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace HNInc.Communication.Library
{
    public class HttpDeiviceHealthCheck
    {
        #region 속성
        [JsonPropertyName("status")]
        public string _status { get; set; }
        public string _requestResult { get; set; }
        #endregion
        #region ToString 재정의
        public override string ToString() => $"Status : {_status}, RequestResult : {_requestResult}";
        #endregion
    }
    public class HttpProductCounts
    {
        #region 속성
        public string _date { get; set; }
        public int _count { get; set; }
        public string _requestResult { get; set; }
        #endregion
        #region 생성자 
        public HttpProductCounts(string date, int count, string requestResult)
        {
            _date = date;
            _count = count;
            _requestResult = requestResult;
        }
        #endregion
        #region ToString 재정의
        public override string ToString() => $"Date : {_date}, Count : {_count}, RequestResult : {_requestResult}";
        #endregion
    }
    public class HttpCycleTime
    {
        #region 속성
        [JsonPropertyName("time")]
        public string _time { get; set; }
        [JsonPropertyName("cycleTime")]
        public int _cycleTime { get; set; }
        [JsonPropertyName("endTime")]
        public string _endTime { get; set; }
        [JsonPropertyName("startTime")]
        public string _startTime { get; set; }
        [JsonPropertyName("transCycleTime")]
        public string _transCycleTime { get; set; }
        public string _requestResult { get; set; }
        #endregion
        #region 생성자 
        public HttpCycleTime(){}
        public HttpCycleTime(string time, int cycleTime, string endTime, string startTime, string transCycleTime, string requestResult)
        {
            _time = time;
            _cycleTime = cycleTime;
            _endTime = endTime;
            _startTime = startTime;
            _transCycleTime = transCycleTime;
            _requestResult = requestResult;
        }
        #endregion
        #region ToString 재정의
        public override string ToString() => $"Time : {_time}, CycleTime : {_cycleTime}, EndTime : {_endTime}, StartTime : {_startTime}, TransCycleTime : {_transCycleTime}, RequestResult : {_requestResult}";
        #endregion
    }
    public class HttpCycleTimeAverage
    {
        #region 속성
        [JsonPropertyName("mean")]
        public double _mean { get; set; }
        public string _requestResult { get; set; }
        #endregion
        #region 생성자 
        public HttpCycleTimeAverage() { }
        public HttpCycleTimeAverage(double mean, string requestResult)
        {
            _mean = mean;
            _requestResult = requestResult;
        }
        #endregion
        #region ToString 재정의
        public override string ToString() => $"Status : {_mean}, RequestResult : {_requestResult}";
        #endregion
    }
    public class HttpCycleInformaiton
    {
        #region 속성
        public string _time { get; set; }
        public string _scalePredict { get; set; }
        public string _scaleLoad { get; set; }
        public string _mae { get; set; }
        public string _requestResult { get; set; }
        #endregion
        #region 생성자 
        public HttpCycleInformaiton(string time, string scalePredict, string scaleLoad, string mae, string requestResult)
        {
            _time = time;
            _scalePredict = scalePredict;
            _scaleLoad = scaleLoad;
            _mae = mae;
            _requestResult = requestResult;
        }
        #endregion
        #region ToString 재정의
        public override string ToString() => $"Time : {_time}, ScalePredict : {_scalePredict}, ScaleLoad : {_scaleLoad}, Mae : {_mae}, RequestResult : {_requestResult}";
        #endregion
    }
    public class HttpRealTimeCount
    {
        #region 속성
        [JsonPropertyName("counts")]
        public int _count { get; set; }
        public string _requestResult { get; set; }
        #endregion
        #region 생성자 
        public HttpRealTimeCount() { }
        public HttpRealTimeCount(int count, string requestResult)
        {
            _count = count;
            _requestResult = requestResult;
        }
        #endregion
        #region ToString 재정의
        public override string ToString() => $"Counts : {_count}, RequestResult : {_requestResult}";
        #endregion
    }
    public class HttpSpindleLoad
    {
        #region 속성
        public string _time { get; set; }
        public string _meanScalePredict { get; set; }
        public string _meanScaleLoad { get; set; }
        public string _meanMae { get; set; }
        public string _requestResult { get; set; }
        #endregion
        #region 생성자 
        public HttpSpindleLoad(string time, string meanScalePredict, string meanScaleLoad, string meanMae, string requestResult)
        {
            _time = time;
            _meanScalePredict = meanScalePredict;
            _meanScaleLoad = meanScaleLoad;
            _meanMae = meanMae;
            _requestResult = requestResult;
        }
        #endregion
        #region ToString 재정의
        public override string ToString() => $"Time : {_time}, MeanScalePredict : {_meanScalePredict}, MeanScaleLoad : {_meanScaleLoad}, MeanMae : {_meanMae}, RequestResult : {_requestResult}";
        #endregion
    }
    public class HttpProductInformation
    {
        #region 속성
        [JsonPropertyName("op")]
        public string _opcode { get; set; }
        [JsonPropertyName("sn")]
        public string _serialNumber { get; set; }
        [JsonPropertyName("acc")]
        public string _accuracy { get; set; }
        [JsonPropertyName("predict")]
        public string _predict { get; set; }
        [JsonPropertyName("startTime")]
        public string _startTime { get; set; }
        [JsonPropertyName("endTime")]
        public string _endTime { get; set; }
        [JsonPropertyName("mae")]
        public string _mae { get; set; }
        public string _requestResult { get; set; }
        #endregion
        #region 생성자
        public HttpProductInformation(){}
        //임시 생성자
        public HttpProductInformation(string opcode, string serialNumber, string accuracy, string predict, string startTime, string endTime, string requestResult)
        {
            _opcode = opcode;
            _serialNumber = serialNumber;
            _accuracy = accuracy;
            _predict = predict;
            _startTime = startTime;
            _endTime = endTime;
            _requestResult = requestResult;
        }
        public HttpProductInformation(string opcode, string serialNumber, string accuracy, string predict, string startTime, string endTime, string mae, string requestResult)
        {
            _opcode = opcode;
            _serialNumber = serialNumber;
            _accuracy = accuracy;
            _predict = predict;
            _startTime = startTime;
            _endTime = endTime;
            _mae = mae;
            _requestResult = requestResult;
        }
        #endregion
        #region ToString 재정의
        public override string ToString() => $"OPCode : {_opcode},SerialNumber : {_serialNumber},Accuracy : {_accuracy},Predict : {_predict},StartTime : {_startTime},EndTime : {_endTime},Mae : {_mae}, RequestResult : {_requestResult}";
        #endregion
    }
    public class HttpQualityInformaiton
    {
        #region 속성
        [JsonPropertyName("op")]
        public string _opcode { get; set; }
        [JsonPropertyName("sn")]
        public string _serialNumber { get; set; }
        [JsonPropertyName("fn")]
        public string _fileName { get; set; }
        [JsonPropertyName("acc")]
        public string _accuracy { get; set; }
        [JsonPropertyName("predict")]
        public string _predict { get; set; }
        [JsonPropertyName("startTime")]
        public string _startTime { get; set; }
        [JsonPropertyName("endTime")]
        public string _endTime { get; set; }
        [JsonPropertyName("predictImg")]
        [JsonConverter(typeof(ByteArrayConverter))]
        public Byte[] _imageBytes { get; set; }
        [JsonPropertyName("imgSize")]
        public int _imageSize { get; set; }
        public string _requestResult { get; set; }
        #endregion

        #region 생성자
        public HttpQualityInformaiton(){}

        public HttpQualityInformaiton(string opcode, string serialNumber, string fileName, string accuracy, string predict, string startTime, string endTime, byte[] imageBytes, int imageSize, string requestResult)
        {
            _opcode = opcode;
            _serialNumber = serialNumber;
            _fileName = fileName;
            _accuracy = accuracy;
            _predict = predict;
            _startTime = startTime;
            _endTime = endTime;
            _imageBytes = imageBytes;
            _imageSize = imageSize;
            _requestResult = requestResult;
        }
        #endregion
        #region ToString 재정의
        public override string ToString() => $"OPCode : {_opcode},SerialNumber : {_serialNumber},FileName : {_fileName},Accuracy : {_accuracy},Predict : {_predict}, ImageBytes : {_imageBytes}, ImageSize : {_imageSize}, RequestResult : {_requestResult}";
        #endregion
    }
    public class HttpAuthentication
    {
        #region 속성
        public bool _checkPassword { get; set; }
        public string _processResult { get; set; }
        #endregion
        #region 생성자
        public HttpAuthentication()
        {
        }
        public HttpAuthentication(bool checkPassword, string processResult)
        {
            _checkPassword = checkPassword;
            _processResult = processResult;
        }
        #endregion
        #region ToString 재정의
        public override string ToString() => $"CheckPassword : {_checkPassword}, ProcessResult : {_processResult}";
        #endregion
    }
    //public class ErrorMessage
    //{
    //    #region 속성
    //    public string _errorMessage { get; set; }
    //    #endregion
    //    #region 생성자 
    //    [JsonConstructor]
    //    public ErrorMessage(string errorMessage)
    //    {
    //        _errorMessage = errorMessage;
    //    }
    //    #endregion
    //    #region ToString 재정의
    //    public override string ToString() => $"ErrorMessage : {_errorMessage}";
    //    #endregion
    //}
    public enum HttpOPCode 
    {
        OP10_3,
    }
    public enum HttpClassification
    {
        day,
        week,
        month,
        custom,
    }
    public enum HttpAbnormalProblems
    {
        Normal,
        T8080_Wear,
        T2030_Wear,
        T2020_Wear,
        T6070_Wear,
        T6060_Wear,
        T1212_Wear,
        T8080_Broken,
        T2030_Broken,
        T2020_Broken,
        T6070_Broken,
        T6060_Broken,
        T1212_Broken,
    }
}
