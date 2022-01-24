using SocketIOClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace HNInc.Communication.Library
{
    public class HNSocketIO
    {
        //SocektIO Library
        private SocketIO _socketIO;
        #region field
        // 해당 회수만큼 Connect 시도
        private readonly int _reconnectionLimmit = 5;
        private int _reconnectionCount = 0;
        #endregion 
        #region Event
        public delegate void FuncDelegate(string evetName, object data);
        public delegate void DeliverDelegate(object sender, object data);
        public event FuncDelegate AnyDataEvent;
        public event FuncDelegate RealTimeLossEvent;
        public event FuncDelegate RealTimeLoadEvent;
        public event FuncDelegate QualityJudgmentProgresStartEvent;
        public event FuncDelegate QualityJudgmentProgresEndEvent;
        public event FuncDelegate ProductInformationEvent;
        public event DeliverDelegate SocketConnectedEvent;
        public event DeliverDelegate SocketDisConnectedEvent;
        public event DeliverDelegate SocketConnectingEvent;
        #endregion
        //Default
        #region Constructor
        public HNSocketIO(Uri uri)
        {
            _socketIO = new SocketIO(uri, new SocketIOOptions
            {
                Query = new Dictionary<string, string>
                {
                    {"token", "V4" }
                },
                EIO = 4,
                Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
            });
            _socketIO.OnConnected += SocketOnConnected;
            _socketIO.OnPing += SocketOnPing;
            _socketIO.OnPong += SocketOnPong;
            _socketIO.OnDisconnected += SocketOnDisconnected;
            _socketIO.OnReconnectAttempt += SocketOnReconnecting;
        }
        public HNSocketIO(Uri uri, SocketIOClient.Transport.TransportProtocol transport)
        {
            _socketIO = new SocketIO(uri, new SocketIOOptions
            {
                Query = new Dictionary<string, string>
                {
                    {"token", "V4" }
                },
                EIO = 4,
                Transport = transport
            });
            _socketIO.OnConnected += SocketOnConnected;
            _socketIO.OnPing += SocketOnPing;
            _socketIO.OnPong += SocketOnPong;
            _socketIO.OnDisconnected += SocketOnDisconnected;
            _socketIO.OnReconnectAttempt += SocketOnReconnecting;
        }
        public HNSocketIO(Uri uri, SocketIOClient.Transport.TransportProtocol transport, TimeSpan connectionTimeout, int reconnectionAttempts)
        {
            _socketIO = new SocketIO(uri, new SocketIOOptions
            {
                Query = new Dictionary<string, string>
                {
                    {"token", "V4" }
                },
                EIO = 4,
                Transport = transport,
                ConnectionTimeout = connectionTimeout,
                ReconnectionAttempts = reconnectionAttempts
            });
            _socketIO.OnConnected += SocketOnConnected;
            _socketIO.OnPing += SocketOnPing;
            _socketIO.OnPong += SocketOnPong;
            _socketIO.OnDisconnected += SocketOnDisconnected;
            _socketIO.OnReconnectAttempt += SocketOnReconnecting;
        }
        #endregion 
        #region SocketIO Event
        private void SocketOnReconnecting(object sender, int e)
        {
            Console.WriteLine($"{DateTime.Now} Reconnecting: attempt = {e}");
            if (SocketConnectingEvent != null)
            {
                SocketConnectingEvent(sender, e);
            }
            else
            {
                Debug.WriteLine("SocketConnectingEvent is NULL, Register SocketConnectingEvent");
            }
        }
        private void SocketOnDisconnected(object sender, string e)
        {
            Console.WriteLine("disconnect: " + e);
            if (SocketDisConnectedEvent != null)
            {
                SocketDisConnectedEvent(sender, e);
            }
            else
            {
                Debug.WriteLine("SocketDisConnectedEvent is NULL, Register SocketDisConnectedEvent");
            }
        }
        private void SocketOnConnected(object sender, EventArgs e)
        {
            Debug.WriteLine("Socket_OnConnected");
            var socket = sender as SocketIO;
            Debug.WriteLine("Socket.Id:" + socket.Id);
            if (SocketConnectedEvent != null)
            {
                SocketConnectedEvent(sender, e);
            }
            else
            {
                Debug.WriteLine("SocketOnConnected is NULL, Register SocketOnConnected");
            }
        }
        private static void SocketOnPing(object sender, EventArgs e)
        {
            Console.WriteLine("Ping");
        }
        private static void SocketOnPong(object sender, TimeSpan e)
        {
            Console.WriteLine("Pong: " + e.TotalMilliseconds);
        }
        #endregion 
        #region Method
        public void Connect()
        {
            _socketIO.ConnectAsync();
        }
        public Boolean IsConnected()
        {
            return _socketIO.Connected;
        }
        public void Disconnect()
        {
            _socketIO.DisconnectAsync();
            _socketIO.Dispose();
        }
        public void SendData(SocketEventNames eventName)
        {
            try
            {
                _socketIO.EmitAsync(eventName.ToString(), DateTime.Now.ToString());
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{DateTime.Now} Error Message : {e.ToString()}");
            }
        }
        public void ReceiveAnyData()
        {
            _socketIO.OnAny((eventName, response) =>
            {
                if (AnyDataEvent != null)
                {
                    AnyDataEvent(eventName, response.GetValue().ToString());
                }
                else
                {
                    Debug.WriteLine("AnyDataEvent is NULL, Register AnyDataEvent");
                }
            });
        }
        public void OffReceiveAnyData()
        {
            //_socketIO.OffAny(handler);
        }
        public void ReceiveRealTimeLossData()
        {
            string eventName = SocketEventNames.RTLossData.ToString();
            _socketIO.On(eventName, response =>
            {
                var responseValue = response.GetValue();
                string mae = responseValue.GetProperty("mae").GetRawText();
                string anomaly = responseValue.GetProperty("anomaly").GetString();
                SocketRealTimeLoss realTimeLoss = new SocketRealTimeLoss(mae, anomaly);
                if (RealTimeLossEvent != null)
                {
                    RealTimeLossEvent(eventName, realTimeLoss);
                }
                else
                {
                    Debug.WriteLine("RealTimeLossEvent is NULL, Register RealTimeLossEvent");
                }
            });
        }
        public void ReceiveRealTimeLoadData()
        {
            string eventName = SocketEventNames.RTLoadData.ToString();
            _socketIO.On(eventName, response =>
            {

                //split try catch 설정
                string[] receiveDataArray;
                try
                {
                    receiveDataArray = response.ToString().Split(new string[] { "[", "]", "\"", ",", "\\n" }, StringSplitOptions.RemoveEmptyEntries);
                }
                catch (Exception e)
                {
                    receiveDataArray = null;
                    Debug.WriteLine("Fail to Split RealTimeLoad, Error Message: {0}, Error Target: {1}", e.Message, e.TargetSite);
                }
                List<SocketRealTimeLoad> rTLoads = new List<SocketRealTimeLoad>();
                SocketRealTimeLoad realTimeLoad;
                // 배열 null 혹은 개수 체크
                if (receiveDataArray != null)
                {
                    for (int i = 0; i < receiveDataArray.Length; i = i + 9)
                    {
                        realTimeLoad = new SocketRealTimeLoad(receiveDataArray[i], receiveDataArray[i + 1], receiveDataArray[i + 2], receiveDataArray[i + 3],
                            receiveDataArray[i + 4], receiveDataArray[i + 5], receiveDataArray[i + 6], receiveDataArray[i + 7], receiveDataArray[i + 8]);
                        rTLoads.Add(realTimeLoad);
                    }
                }
                if (RealTimeLoadEvent != null)
                {
                    RealTimeLoadEvent(eventName, rTLoads);
                }
                else
                {
                    Debug.WriteLine("RealTimeLoadEvent is NULL, Register RealTimeLoadEvent");
                }
            });
        }
        public void ReceiveQualityJudgmentProgresStart()
        {
            string eventName = SocketEventNames.judgeQualityStart.ToString();
            _socketIO.On(eventName, response =>
            {
                var responseValue = response.GetValue();
                string opCode = responseValue.GetProperty("op").GetString();
                string serialNumber = responseValue.GetProperty("sn").GetString();
                SocketQualityInformation qualityInformation = new SocketQualityInformation(opCode, serialNumber);
                if (QualityJudgmentProgresStartEvent != null)
                {
                    QualityJudgmentProgresStartEvent(eventName, qualityInformation);
                }
                else
                {
                    Debug.WriteLine("QualityJudgmentProgresStartEvent is NULL, Register QualityJudgmentProgresStartEvent");
                }
            });
        }
        public void ReceiveQualityJudgmentProgressEnd()
        {
            string eventName = SocketEventNames.judgeQualityEnd.ToString();
            _socketIO.On(eventName, response =>
            {
                var responseValue = response.GetValue();
                string opCode = responseValue.GetProperty("op").GetString();
                string serialNumber = responseValue.GetProperty("sn").GetString();
                string accuracy = responseValue.GetProperty("acc").GetString();
                string temp = responseValue.GetProperty("predict").GetString();
                string predict = Enum.GetName(typeof(SocketAbnormalProblems), Int32.Parse(temp));
                string fileName = responseValue.GetProperty("fn").GetString();
                Byte[] imageBytes = response.InComingBytes[0];

                SocketQualityInformation qualityInformation = new SocketQualityInformation(opCode, serialNumber, accuracy, predict, fileName, imageBytes);
                if (QualityJudgmentProgresEndEvent != null)
                {
                    QualityJudgmentProgresEndEvent(eventName, qualityInformation);
                }
                else
                {
                    Debug.WriteLine("QualityJudgmentProgresEndEvent is NULL, Register QualityJudgmentProgresEndEvent");
                }
            });
        }
        public void ReceiveProductInformation()
        {
            string eventName = SocketEventNames.productInformation.ToString();
            _socketIO.On(eventName, response =>
            {
                var responseValue = response.GetValue();
                string opCode = responseValue.GetProperty("op").GetString();
                string serialNumber = responseValue.GetProperty("sn").GetString();
                string accuracy = responseValue.GetProperty("acc").GetString();
                string temp = responseValue.GetProperty("predict").GetString();
                string predict = Enum.GetName(typeof(SocketAbnormalProblems), Int32.Parse(temp));
                string startTime = responseValue.GetProperty("startTime").GetString();
                string endTime = responseValue.GetProperty("endTime").GetString();
                string mae = responseValue.GetProperty("mae").GetString();
                if (temp.Equals("0") && mae.Equals("abnormal"))
                {
                    predict = "Abnormal data exists. Please check...";
                }
                SocketProductInformation productInformation = new SocketProductInformation(opCode, serialNumber, accuracy, predict, startTime, endTime, mae);
                if (ProductInformationEvent != null)
                {
                    ProductInformationEvent(eventName, productInformation);
                }
                else
                {
                    Debug.WriteLine("ProductInformationEvent is NULL, Register ProductInformationEvent");
                }
            });
        }
        public void OffReceiveData(SocketEventNames eventName)
        {
            string eventNameString = eventName.ToString();
            _socketIO.Off(eventNameString);
        }
        #endregion 
    }
}

