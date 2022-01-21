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

**기능 설명**
1. 로그아웃 버튼
- 클릭 시, 로그인 페이지로 이동
2. 공정 위치
 - 현재 스텝 정보 표시
3. 가동 현황
 - 공정의 가동현황 및 AI판정 여부
4. 품질 판정
 - AI를 통한 제품의 양/불량품 품질 판정
5. 실시간 부하량 차트
 - 실시간으로 부하량과 AI를 통한 예측 부하량 그리고 mae가 업데이트 되는 차트
6. 부하량 이력 조회 차트
 - 부하량과 AI를 통한 예측 부하량 그리고 mae에 대한 이력 조회 할 수 있는 차트
7. 제품 리스트
 - 생산된 제품의 정보와 품질을 확인할 수 있는 리스트

### 공통 alert
1. 경고 알림 창
<img width="500" src="https://user-images.githubusercontent.com/37472764/150469665-7d16d102-fd39-4adb-aeb5-5515bfd9d4df.png">

2. 에러 알림 창
<img width="500" src="https://user-images.githubusercontent.com/37472764/150469757-e137ded3-50ef-499e-a32d-408e48096cf4.png">

3. 정보 알림 창
<img width="500" src="https://user-images.githubusercontent.com/37472764/150469832-4aee4e5c-f364-4ac5-8279-37ff73fdcb62.png">
