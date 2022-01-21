namespace HNInc.Communication.Library
{
    public class SocketRealTimeLoss
    {
        #region 속성
        public string _maeData { get; set; }
        public string _anomalyData { get; set; }
        #endregion
        #region 생성자
        public SocketRealTimeLoss(string maeData, string anomalyData)
        {
            _maeData = maeData;
            _anomalyData = anomalyData;
        }
        #endregion
        #region ToString 재정의
        public override string ToString() => $"Mae : {_maeData},Anomaly : {_anomalyData}";
        #endregion
    }
    public class SocketRealTimeLoad
    {
        #region 속성
        public string _opcode { get; set; }
        public string _time { get; set; }
        public string _scaleLoad { get; set; }
        public string _loadSpindle { get; set; }
        public string _tCode { get; set; }
        public string _serialNumber { get; set; }
        public string _scalePredict { get; set; }
        public string _predict { get; set; }
        public string _mae { get; set; }
        #endregion

        #region 생성자
        public SocketRealTimeLoad(string opcode, string time, string scaleLoad, string loadSpindle, string tCode, string serialNumber, string scalePredict, string predict, string mae)
        {
            _opcode = opcode;
            _time = time;
            _scaleLoad = scaleLoad;
            _loadSpindle = loadSpindle;
            _tCode = tCode;
            _serialNumber = serialNumber;
            _scalePredict = scalePredict;
            _predict = predict;
            _mae = mae;
        }
        #endregion
        #region ToString 재정의
        public override string ToString() => $"OPCode : {_opcode},Time : {_time},ScaleLoad : {_scaleLoad},LoadSpindle : {_loadSpindle},TCode : {_tCode},SerialNumber : {_serialNumber},ScalePredict : {_scalePredict},Predict : {_predict},MAE : {_mae}";
        #endregion
    }
    public class SocketQualityInformation
    {
        #region 속성
        public string _opcode { get; set; }
        public string _serialNumber { get; set; }
        public string _accuracy { get; set; }
        public string _predict { get; set; }
        public string _fileName { get; set; }
        public byte[] _imageBytes { get; set; }
        #endregion

        #region 생성자
        public SocketQualityInformation(string opcode, string serialNumber)
        {
            _opcode = opcode;
            _serialNumber = serialNumber;
            _accuracy = null;
            _predict = null;
            _fileName = null;
            _imageBytes = null;
        }

        public SocketQualityInformation(string opcode, string serialNumber, string accuracy, string predict, string fileName, byte[] imageBytes)
        {
            _opcode = opcode;
            _serialNumber = serialNumber;
            _accuracy = accuracy;
            _predict = predict;
            _fileName = fileName;
            _imageBytes = imageBytes;
        }
        #endregion
        #region ToString 재정의
        public override string ToString() => $"OPCode : {_opcode},SerialNumber : {_serialNumber},Accuracy : {_accuracy},Predict : {_predict},FileName : {_fileName}, ImageBytes : {_imageBytes}";
        #endregion
    }
    public class SocketProductInformation
    {
        #region 속성
        public string _opcode { get; set; }
        public string _serialNumber { get; set; }
        public string _accuracy { get; set; }
        public string _predict { get; set; }
        public string _startTime { get; set; }
        public string _endTime { get; set; }
        public string _mae { get; set; }
        #endregion
        #region 생성자
        public SocketProductInformation(string opcode, string serialNumber, string accuracy, string predict, string startTime, string endTime, string mae)
        {
            _opcode = opcode;
            _serialNumber = serialNumber;
            _accuracy = accuracy;
            _predict = predict;
            _startTime = startTime;
            _endTime = endTime;
            _mae = mae;
        }
        #endregion
        #region ToString 재정의
        public override string ToString() => $"OPCode : {_opcode},SerialNumber : {_serialNumber},Accuracy : {_accuracy},Predict : {_predict}";
        #endregion
    }
    public enum SocketEventNames
    {
        RTLossData,
        RTLoadData,
        judgeQualityStart,
        judgeQualityEnd,
        productInformation,
    }
    public enum SocketAbnormalProblems
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
