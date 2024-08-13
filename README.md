# REMFactory Project by Team 라디얼가그


신재생 에너지 발전량에 따른 에너지 소비량 관리 프로그램

<br>

# 아이디어

1) RE100 실현
2) 스마트 팩토리 운영시 공장 효율성 극대화
3) 에너지 생산량에 따라 공장 전력 사용 조절 & 판단 가능
4) 잉여 에너지 판매 수익 관리 가능

<br>

# 사용 데이터
1) 한국서부발전(주)_태양광 발전 현황_20230630csv (출처 : 공공데이터포털 data.go.kr)

👉 전기별 1시간 단위 태양광 발전 현황(2017.01.01 ~ 2023.05.31)

2) 제주특별자치도개발공사_제주삼다수공장_시간별_전력사용량_20230930.csv(출처 : 공공데이터포털 data.go.kr)

👉 시간별 1시간 단위 전력 사용 현황(2023.01.01 ~ 2023.06.30)

3) 한국전력거래소_오늘의 REC 시장_20240502.csv(출처 : 공공데이터포털 data.go.kr))

👉 날짜별 전력 거래 시세 현황(2023.01.03 ~ 2023.06.29)

# 실행 화면

### Tap1)

Live Chart 실시간 그래프 (X축 : 날짜 및 시간, Y축 : 태양광 발전량)

Live Chart 실시간 그래프 (X축 : 날짜 및 시간, Y축 : 공장 라인별 전력 사용량)

사용량 퍼센테이지 라인별 Radial Gauge로 표현

생산량 Label로 표현해주고 slider로 조절 가능하게 구현

각 라인의 전력 효율 Radial Gauge로 표현

한계 퍼센트가 넘을 시 알람

새 창에 ScottPlot 그래프로 라인별 태양광 생산량 


![image](https://github.com/user-attachments/assets/709adf76-7f54-4667-986f-5ea599361129)
![image](https://github.com/user-attachments/assets/e67f62d0-9768-47ae-9f66-68e37d13c411)
![image](https://github.com/user-attachments/assets/3fd753a2-a019-4325-adba-529dcd354cf9)


### Tap2)

ScottPlot 그래프 (X축 : 날짜 및 시간, Y축 : 남은 전력량)

ScottPlot 그래프 (X축 : 날짜 및 시간, Y축 : 남은 전력량 판매 금액)

DataGrid로 날짜별 잔여 전력량 출력

전력 거래소가 운영하는 날에는 누적 전력 판매 금액 Label로 표시

운영하지 않는 날에는 텍스트로 출력


![image](https://github.com/user-attachments/assets/627c7f6b-c356-497d-8c22-e524668a1923)
![image](https://github.com/user-attachments/assets/45065568-25fb-4cbd-bae5-0196af7e25d5)

### 추가기능

관리자모드 추가(추후 구현 예정)
1)	라인별 max 용량 설정 기능
2)	라인별 이름 변경 기능 -> dropdown 버튼으로 구현
3)	라인별 알림 % 설정 기능(몇 %에서 알람이 울릴건가)
4)	그래프 이름 변경 기능





# 구성원
```swift
public Enginner RadialGag() {
  public Enginner SeungHyun;
  public Enginner JongHo;
  public designer SeungHoon;
  ...
}
```




# 개발 도구

<img src ="https://img.shields.io/badge/-C%23-000000?logo=Csharp&style=flat" style="height: 30px;">
<img src="https://img.shields.io/badge/-WPF-0078D7?style=flat&logo=windows&logoColor=white" style="height: 30px;">
<img src="https://img.shields.io/badge/-LiveChart-0078D7?style=flat&logo=windows&logoColor=white" style="height: 30px;">

# 소통 도구

<img src ="https://img.shields.io/badge/github-181717?logo=github&style=flat" style="height: 30px;">
<img src ="https://img.shields.io/badge/notion-000000?logo=notion&style=flat" style="height: 30px;">
<img src ="https://img.shields.io/badge/slack-4A154B?logo=slack&style=flat" style="height: 30px;">
