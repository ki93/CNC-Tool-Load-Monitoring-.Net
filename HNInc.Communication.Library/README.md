# HNInc.Communication.Libray
![Generic badge](https://img.shields.io/badge/WPF-gray.svg) ![Generic badge](https://img.shields.io/badge/Nuget-6.0.1-blue.svg) ![Generic badge](https://img.shields.io/badge/.NET-5.0-blue.svg)
## Description
- CNC Tool Monitoring Project에서 사용한 Server와의 통신API
- WebSocekt 통신과 Http 통신을 이용한 Data 송수신
## Environment
- Visual Studio Community  2022
- .Net 5.0
## Prerequisite
- Nuget (6.0.1)
    - MySql.Data (8.0.27)
    - SocketIOClient(3.0.5)
## Usage
### Socket
---
#### Namespace
```C#
using HNInc.Communication.Library;
```
#### Constructor
```C#
HNSocketIO hNSokcetIO = new HNSocketIO(Uri uri);
HNSocketIO hNSokcetIO = new HNSocketIO(Uri uri, SocketIOClient.Transport.TransportProtocol transport);
HNSocketIO hNSokcetIO = new HNSocketIO(Uri uri, SocketIOClient.Transport.TransportProtocol transport, TimeSpan connectionTimeout, int reconnectionAttempts);
```
#### Method
- Connect()
    - void
    - Socekt 연결
- IsConnected()
    - return Blooean
    - Socket 연결 상태 확인
- Diconnect()
    - Socket 연결 해제
- SendData(SocketEventNames eventName)
    - void
    - 데이터 전송
- ReceiveAnyData()
    - void
    - Data 수신 시 이벤트 발생
- ReceiveRealTimeLossData(), ReceiveRealTimeLoadData(), ReceiveQualityJudgmentProgresStart(), ReceiveQualityJudgmentProgressEnd(), ReceiveProductInformation()
    - void
    - Data 수신 시 이벤트 발생
    - QualityJudment 관련 함수와 ReceiveProductInformation 함수는 한 사이클 끝날 시 이벤트 발생
- OffReceiveData(SocketEventNames eventName)
    - void
    - eventName의 Data 수신 종료
#### Event
- AnyDataEvent, RealTimeLossEvent, RealTimeLoadEvent, QualityJudgmentProgresStartEvent, QualityJudgmentProgresEndEvent, ProductInformationEvent
    - type : FuncDelegate <= delegate void FuncDelegate(string evetName, object data)
- SocketConnectedEvent, SocketDisConnectedEvent, SocketConnectingEvent
    - type : DeliverDelegate <= delegate void DeliverDelegate(object sender, object data)
### Http
---
#### Namespace
```C++
using HNInc.Communication.Library;
```
#### Constructor
```C#
HNHttp hNHttp = new HNHttp(string wasUrl, string accountDBUrl, string accountDB);
```
#### Method
```C#
HttpDeiviceHealthCheck httpDeiviceHealthCheck= hNHttp.GetDeiviceHealthCheck(HttpOPCode oPCode);
List<HttpProductCounts> httpProductCountsList = NHttp.GetProductCountsList(DateTime startTime, DateTime endTime, HttpOPCode oPCode, HttpClassification classification);
List<HttpCycleTime> httpCycleTimeList = hNHttp.GetCycleTimeList(HttpOPCode oPCode, int count);
HttpCycleTimeAverage httpCycleTimeAverage = hNHttp.GetCycleTimeAverage(int count);
List<HttpCycleInformaiton> httpCycleInformaitonList = hNHttp.GetCycleInformationList(HttpOPCode oPCode, string serialNumber);
HttpRealTimeCount httpRealTimeCount = hNHttp.GetRealTimeCount(HttpOPCode oPCode);
List<HttpSpindleLoad> httpSpindleLoadList = hNHttp.GetSpindleLoadList(DateTime startTime, DateTime endTime, HttpOPCode oPCode, string groupBy);
List<HttpProductInformation> httpProductInformationList = hNHttp.GetProductInformationList(int days);
HttpQualityInformaiton httpQualityInformaiton = hNHttp.GetQualityInformaiton();
HttpAuthentication httpAuthentication = hNHttp.CheckAuthentication(string id, string password);
```
- GET Method를 이용한 데이터 요청
- 회원 인증시 SHA256을 이용하여 비밀번호 암호화