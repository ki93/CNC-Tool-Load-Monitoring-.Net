using HNInc.Communication.Library;
using System;
using System.Collections.Generic;

namespace CncPrj_WPF_Core
{
    class SocketConnecting
    {
        HNSocketIO _hNSocketIO;
        public OpWindow opwin;
        JudgeQuality _judgeQuality;

        public SocketConnecting(ref OpWindow opwindow)
        {
            opwin = opwindow;
            SocketConnecting socketConnecting = this;
            _judgeQuality = new JudgeQuality(ref opwindow);
        }

        //소켓 연결
        public void SocketConn()
        {
            _hNSocketIO = new HNSocketIO();
            _hNSocketIO.Connect();

            while (!_hNSocketIO.IsConnected())
            {
                _hNSocketIO.SendData(SocketEventNames.RTLoadData);
                _hNSocketIO.SendData(SocketEventNames.RTLossData);
            }
            _hNSocketIO.ReceiveRealTimeLoadData();
            _hNSocketIO.RealTimeLoadEvent += opwin.InputRealTimeLoadData;

            _hNSocketIO.ReceiveRealTimeLossData();
            _hNSocketIO.RealTimeLossEvent += opwin.InputRealTimeLossData;

            _hNSocketIO.ReceiveQualityJudgmentProgresStart();
            _hNSocketIO.QualityJudgmentProgresStartEvent += _judgeQuality.InputJudgeQualityStart;

            _hNSocketIO.ReceiveQualityJudgmentProgressEnd();
            _hNSocketIO.QualityJudgmentProgresEndEvent += _judgeQuality.InputJudgeQualityEnd;

            _hNSocketIO.ReceiveProductInformation();
            _hNSocketIO.ProductInformationEvent += opwin.InputProductInformation;

        }
    }
}
