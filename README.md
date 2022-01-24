# CNC-Tool-Load-Monitoring

## Project description
**가공설비(CNC) 툴 모니터링 시스템 .Net Version**

## 로그인 화면
<img width="1000" src="https://user-images.githubusercontent.com/37472764/150469529-81bbc2b5-d3b1-4b19-a5e2-f5c1fdb124b2.PNG">

1. 사용자 ID 입력
2. 사용자 PW 입력
3. 로그인 버튼
- ID 혹은 PW 미 입력 시, 미 입력에 대한 안내 문구가 PW 텍스트 필드 하단에 생성 (빨간색 글씨)
- ID 미 입력 시 -> 아이디를 입력해주세요.
- PW 미 입력 시 -> 비밀번호를 입력해주세요.
- ID 혹은 PW 오류 시, 안내 문구와 함께 모든 text field 리셋 (아이디 혹은 비밀번호가 일치하지 않습니다.)
- 로그인 성공 후, Opreation1 페이지 이동


## 공정 화면
<img width="1000" src="https://user-images.githubusercontent.com/37472764/150473611-121278f9-79cb-412d-ab17-2b17cf9f0803.png">

**화면 구성 설명**
1. 로그아웃 버튼
- 클릭 시, 로그인 페이지로 이동
2. 공정 위치
 - 현재 스텝 정보 표시
3. 가동 현황
 - 공정의 가동현황 및 AI판정 여부
4. 품질 판정
 - AI를 통한 제품의 양/불량품 품질 판정
 - 판정 이미지 expansion
5. 실시간 부하량 차트
 - 실시간으로 부하량과 AI를 통한 예측 부하량 그리고 mae가 업데이트 되는 차트
6. 부하량 이력 조회 차트
 - 부하량과 AI를 통한 예측 부하량 그리고 mae에 대한 이력 조회 할 수 있는 차트
 - 차트 expansion
7. 제품 리스트
 - 생산된 제품의 정보와 품질을 확인할 수 있는 리스트
 - 한 제품의 공정 중 부하량 chart

**Dialog**
1. 생산량 이력 조회
<img width="500" src="https://user-images.githubusercontent.com/37472764/150488327-0a09740d-1d92-499d-a4e0-7a8cde53cb86.PNG">

탭 이동을 통해 특정 기간, 일별, 주별, 월별 데이터 확인
- 탭 이동을 통해 특정 기간, 일별, 주별, 월별 데이터 확인
- search 탭에서 설정한 기간과 기간에 해당하는 생산량 COUNT가 표시

2. Cycle Time 이력 조회
<img width="500" src="https://user-images.githubusercontent.com/37472764/150488381-25d35fca-b7a9-4211-8a3d-dad3532f00e4.PNG">

최근 10개 Cycle Time ( Bar Chart )
- 최근 10개의 cycle time 그래프
- 밀리세컨드까지 표시
최근 100개 Cycle Time ( Grid )
- 최근 100개의 cycle time 값
- 공정의 start time, end time 표시, 그 차이를 cycle time 으로 표시.

**Folding 기능**
<img width="1000" src="https://user-images.githubusercontent.com/37472764/150490762-67e5ecc8-7fa2-40ec-b629-0a28c88125fd.png">
                       
1. 이력 조회 오픈 버튼
 - 부하량 이력 조회 확인을 위한 버튼
2. 이력 조회 클로즈 버튼

### 공통 alert
1. 경고 알림 창
<img width="500" src="https://user-images.githubusercontent.com/37472764/150469665-7d16d102-fd39-4adb-aeb5-5515bfd9d4df.png">

2. 에러 알림 창
<img width="500" src="https://user-images.githubusercontent.com/37472764/150469757-e137ded3-50ef-499e-a32d-408e48096cf4.png">

3. 정보 알림 창
<img width="500" src="https://user-images.githubusercontent.com/37472764/150469832-4aee4e5c-f364-4ac5-8279-37ff73fdcb62.png">
