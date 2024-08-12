using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.Analysis;
using System.Diagnostics.Eventing.Reader;
using ScottPlot.AxisLimitManagers;
using System.Collections;

namespace REMFactory
{
    public partial class MainWindow : Window
    {
        private string usingDataPath = Path.GetFullPath(@"제주특별자치도개발공사_제주삼다수공장_시간별_전력사용량_20230930.csv");//전력 사용량 csv 파일 path
        private int startDelayTime = 2000;
        Dictionary<DateTime, List<List<object>>> dateGroupedData;//날짜를 key로 하는 사용량 리스트를 넣는 딕셔너리
        Dictionary<DateTime, List<List<object>>> dateElectrocityStoreData;//날짜를 key로 하는 전력 저장소 시세 리스트를 넣는 딕셔너리
        double doubleValue;//Line1의 사용량을 double로 parsing한 값
        double doubleValue2;//Line2의 사용량을 double로 parsing한 값 
        double doubleValue3;//Line3의 사용량을 double로 parsing한 값
        private double efficiency = 10000;//라인1 max 전력량
        private double efficiency2 = 15000;//라인2 max 전력량
        private double efficiency3 = 20000;//라인3 max 전력량
        private bool openCheck = false;//전력 거래소 운영 여부 false가 운영X
        private List<double> electricitySellList;//하루 지났을 때 잔여 전력 판매 금액 리스트 ->여기에 판매 금액 넣고 판매 날짜(date)를 key값으로 딕셔너리 만들기
        private string openDataPath = Path.GetFullPath(@"한국전력거래소_오늘의 REC 시장_20240502.csv");//전력 판매소 날짜별 시세 csv 파일 path
        double electricityAllSell;//판매 누적 금액 저장
        bool checkDateChange = true;//날짜가 바뀌는 것을 확인하는 boolean
        private string adminID = "admin123";//관리자 아이디
        private string adminPW = "admin123";//관리자 비밀번호

        //사용량 데이터 가져와서 그래프로 띄우기
        private DataFrame LoadUsingDataFrame(string path)//path 이용해서 csv파일 불러오는 메서드
        {
            // Check if file exists
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("The specified data file does not exist.", path);
            }

            // Load the data into the DataFrame
            return DataFrame.LoadCsv(path);
        }

        public async Task getPanel2Data()//사용량 데이터 가져와서 그래프로 띄우기 메인 메서드
        {
            var df = LoadUsingDataFrame(usingDataPath);//데이터를 가져와서 dataFrame으로 반환

            var tempGroupedData = new Dictionary<DateTime, List<List<object>>>();

            dateGroupedData = usingDataToListDictionary(df, tempGroupedData);//데이터프레임 리스트로 만들어서 딕셔너리에 정리
            await Task.Delay(startDelayTime);
            // 결과 출력
            foreach (var date in dateGroupedData.Keys)
            {
                Console.WriteLine($"Date: {date.ToShortDateString()}");
                foreach (var row in dateGroupedData[date])
                {
                    foreach (var value in row)
                    {
                        if (Dispatcher.CheckAccess())
                        {
                            UpdateLabelAndSlider(value);//label value와 slider value를 바꾸는 메서드 
                        }
                        else
                        {
                            Dispatcher.Invoke(() => UpdateLabelAndSlider(value));
                        }

                        // 20ms 대기
                        await Task.Delay(1000);
                    }
                }
            }
        }

        void UpdateLabelAndSlider(object value)//사용량의 데이터를 라벨과 슬라이더에 업데이트하는 메서드
        {
            try
            {
                doubleValue = Convert.ToDouble(value);
                doubleValue2 = doubleValue * 1.5;
                doubleValue3 = doubleValue * 2;
                if (powerTotal < doubleValue + doubleValue2 + doubleValue3)
                {
                    doubleValue = 0;
                    doubleValue2 = 0;
                    doubleValue3 = 0;
                    sliderLine1.Value = doubleValue;
                    sliderLine2.Value = doubleValue2;
                    sliderLine3.Value = doubleValue3;
                }
                labelLine1.Text = doubleValue.ToString();
                labelLine2.Text = doubleValue2.ToString();
                labelLine3.Text = doubleValue3.ToString();
                sliderLine1.Value = doubleValue;
                sliderLine2.Value = doubleValue2;
                sliderLine3.Value = doubleValue3;
                //MessageBox.Show($"적용된 doublevalue: {doubleValue},적용된 doublevalue2:{doubleValue2},적용된 doublevalue3:{doubleValue3}");
            }
            catch (InvalidCastException)
            {
                Console.WriteLine("Object의 형식이 잘못되었습니다.");
            }
            catch (FormatException)
            {
                Console.WriteLine("Object의 형식이 잘못되었습니다.");
            }
            catch (OverflowException)
            {
                Console.WriteLine("값이 너무 큽니다.");
            }
            

           
        }

        public void getUsingData()
        {
            var dataPath = Path.GetFullPath(@"제주특별자치도개발공사_제주삼다수공장_시간별_전력사용량_20230930.csv");

            // Load the data into the data frame
            var df = DataFrame.LoadCsv(dataPath);
        }

        public Dictionary<DateTime, List<List<object>>> usingDataToListDictionary(DataFrame df, Dictionary<DateTime, List<List<object>>> dateListDictionary)
        {
            foreach (var row in df.Rows)
            {

                DateTime date;
                if (DateTime.TryParse(row[0].ToString(), out date))
                {
                    date = (DateTime)row[0]; // 첫 번째 컬럼이 날짜 컬럼이라고 가정

                    if (date.Month <= 6) //6월까지만 출력
                    {
                        var rowData = new List<object>();

                        foreach (var cell in row.Skip(1)) // 날짜 컬럼을 제외한 나머지 셀
                        {
                            rowData.Add(cell);
                        }

                        // 날짜별로 리스트에 추가
                        if (!dateListDictionary.ContainsKey(date))
                        {
                            dateListDictionary[date] = new List<List<object>>();
                        }
                        dateListDictionary[date].Add(rowData);
                    }
                }
                else
                {
                    // date 변환 실패 - 적절한 예외 처리 또는 로그 출력
                    throw new InvalidCastException("첫 번째 컬럼을 DateTime 형식으로 변환할 수 없습니다.");
                }
            }
            return dateListDictionary;
        }

        //public List<double> getModelData() // 사용량 데이터 가져와서 리스트로 반환
        //{
        //    var df = LoadUsingDataFrame(usingDataPath); // 데이터 로드

        //    var dateGroupedData = new Dictionary<DateTime, List<List<object>>>(); // 딕셔너리 초기화

        //    usingDataToListDictionary(df, dateGroupedData); // 데이터 그룹화

        //    var resultList = new List<double>(); // 결과 리스트 생성

        //    // 결과 출력
        //    foreach (var date in dateGroupedData.Keys)
        //    {
        //        Console.WriteLine($"Date: {date.ToShortDateString()}"); // 날짜 출력
        //        foreach (var row in dateGroupedData[date])
        //        {
        //            foreach (var value in row)
        //            {
        //                double valueDouble = Convert.ToDouble(value);
        //                if (powerTotal < doubleValue + doubleValue2 + doubleValue3)
        //                {
        //                    valueDouble = 0;
        //                }
        //                resultList.Add(valueDouble); // 결과 리스트에 값 추가
        //            }
        //        }
        //    }

        //    return resultList; // 결과 리스트 반환
        //}
        //각 라인당 전력 사용 효율

        public async Task getefficiencyData()
        {
            var df = LoadUsingDataFrame(usingDataPath);//데이터를 가져와서 dataFrame으로 반환

            //dateGroupedData = new Dictionary<DateTime, List<List<object>>>();

            usingDataToListDictionary(df, dateGroupedData);//데이터프레임 리스트로 만들어서 딕셔너리에 정리

            await Task.Delay(startDelayTime);
            // 결과 출력
            foreach (var date in dateGroupedData.Keys)
            {
                Console.WriteLine($"Date: {date.ToShortDateString()}");
                foreach (var row in dateGroupedData[date])
                {
                    foreach (var value in row)
                    {

                        if (Dispatcher.CheckAccess())
                        {
                            howefficiencyData(value);//label value와 slider value를 바꾸는 메서드 
                        }
                        else
                        {
                            Dispatcher.Invoke(() => howefficiencyData(value));
                        }

                        // 20ms 대기
                        await Task.Delay(1000);
                    }
                }
            }
        }

        void howefficiencyData(object value)
        {
            if (double.TryParse(value.ToString(), out double doubleValue))//파싱 성공하면 value를 doubleValue에 넣는다
            {
                int overValue = 0;
                if (powerTotal < doubleValue + doubleValue2 + doubleValue3)
                {
                    labelEfficiencyLine1.Text = overValue.ToString();
                    labelEfficiencyLine2.Text = overValue.ToString();
                    labelEfficiencyLine3.Text = overValue.ToString();
                    return;
                }
                double efficiencyData = doubleValue / efficiency * 100;
                double line2Data = doubleValue2 / efficiency2 * 100;
                double line3Data = doubleValue3 / efficiency3 * 100;
                labelEfficiencyLine1.Text = efficiencyData.ToString();
                labelEfficiencyLine2.Text = line2Data.ToString();
                labelEfficiencyLine3.Text = line3Data.ToString();
                //MessageBox.Show($"1: {efficiencyData},2:{line2Data},3:{line3Data}");
            }
            else
            {
                // value를 double로 변환할 수 없는 경우에 대한 예외 처리 또는 로그 출력
                Console.WriteLine("double로 처리할 수 없습니다.");
            }
        }

        //전력 거래소 관련 메서드
        public async Task ElectrocityStore()//사용량 데이터 가져와서 그래프로 띄우기 메인 메서드
        {
            var df = LoadUsingDataFrame(openDataPath);//데이터를 가져와서 dataFrame으로 반환

            dateElectrocityStoreData = new Dictionary<DateTime, List<List<object>>>();

            df = filteringDataFrame(df);

            //// 날짜 열과 육지 평균가 열을 설정합니다.
            //var dateColumnName = "거래일";
            //var priceColumnName = "육지 평균가(원)";

            //// 날짜 열과 육지 평균가 열을 가져옵니다.
            //var dateColumn = df.Columns[dateColumnName] as PrimitiveDataFrameColumn<DateTime>;
            //var priceColumn = df.Columns[priceColumnName] as PrimitiveDataFrameColumn<float>;

            //// 유효한 컬럼인지 확인
            //if (dateColumn == null || priceColumn == null)
            //{
            //    throw new InvalidOperationException("날짜 열 또는 육지 평균가 열이 올바른 형식이 아닙니다.");
            //}

            //// 필터링 조건을 만듭니다.
            //var startDate = new DateTime(2023, 1, 1);
            //var endDate = new DateTime(2023, 6, 30);

            //// 필터링된 데이터를 저장할 리스트를 만듭니다.
            //var filteredDates = new List<DateTime>();
            //var filteredPrices = new List<double>();

            //// 각 행을 순회하며 조건을 만족하는지 확인하고, 해당 데이터를 리스트에 추가합니다.
            //for (int i = 0; i < df.Rows.Count; i++)
            //{
            //    var dateTemp = dateColumn[i];
            //    var price = priceColumn[i];
            //    if (dateTemp.HasValue)
            //    {
            //        DateTime date = dateTemp.Value; // dateTemp가 null이 아닌 경우에만 사용
            //        if (date >= startDate && date <= endDate)
            //        {
            //            filteredDates.Add(date);
            //            filteredPrices.Add((double)price); // float 값을 double로 변환하여 추가
            //        }
            //    }

            //}
            //// 필터링된 데이터를 기반으로 새로운 DataFrame을 생성합니다.
            //var filteredDf = new DataFrame(new DataFrameColumn[]
            //{
            //new PrimitiveDataFrameColumn<DateTime>(dateColumnName, filteredDates),
            //new PrimitiveDataFrameColumn<double>(priceColumnName, filteredPrices)
            //});
            dateElectrocityStoreData = usingDataToListDictionary(df, dateElectrocityStoreData);//데이터프레임 리스트로 만들어서 딕셔너리에 정리

            // 결과 출력
            foreach (var date in dateElectrocityStoreData.Keys)
            {
                // today와 date가 동일한지 비교합니다.
                if (date.Date == today)
                {
                    Console.WriteLine($"Date: {date.ToShortDateString()}");

                    foreach (var row in dateElectrocityStoreData[date])
                    {
                        foreach (var value in row)
                        {
                            if (Dispatcher.CheckAccess())
                            {
                                UpdateElectrocityStroreLabel(value); // label value와 slider value를 바꾸는 메서드
                            }
                            else
                            {
                                Dispatcher.Invoke(() => UpdateElectrocityStroreLabel(value));
                            }
                        }
                    }
                }
                else
                {
                    maxElectrocitySoldLabel.Content = "오늘은 전력 거래소가 운영하지 않습니다.";
                }
            }
        }

        DataFrame filteringDataFrame(DataFrame dataFrame)
        {
            // 날짜 열과 육지 평균가 열을 설정합니다.
            var dateColumnName = "거래일";
            var priceColumnName = "육지 평균가(원)";

            // 날짜 열과 육지 평균가 열을 가져옵니다.
            var dateColumn = dataFrame.Columns[dateColumnName] as PrimitiveDataFrameColumn<DateTime>;
            var priceColumn = dataFrame.Columns[priceColumnName] as PrimitiveDataFrameColumn<float>;

            // 유효한 컬럼인지 확인
            if (dateColumn == null || priceColumn == null)
            {
                throw new InvalidOperationException("날짜 열 또는 육지 평균가 열이 올바른 형식이 아닙니다.");
            }

            // 필터링 조건을 만듭니다.
            var startDate = new DateTime(2023, 1, 1);
            var endDate = new DateTime(2023, 6, 30);

            // 필터링된 데이터를 저장할 리스트를 만듭니다.
            var filteredDates = new List<DateTime>();
            var filteredPrices = new List<double>();

            // 각 행을 순회하며 조건을 만족하는지 확인하고, 해당 데이터를 리스트에 추가합니다.
            for (int i = 0; i < dataFrame.Rows.Count; i++)
            {
                var dateTemp = dateColumn[i];
                var price = priceColumn[i];
                if (dateTemp.HasValue)
                {
                    DateTime date = dateTemp.Value; // dateTemp가 null이 아닌 경우에만 사용
                    if (date >= startDate && date <= endDate)
                    {
                        filteredDates.Add(date);
                        filteredPrices.Add((double)price); // float 값을 double로 변환하여 추가
                    }
                }

            }
            // 필터링된 데이터를 기반으로 새로운 DataFrame을 생성합니다.
            var filteredDf = new DataFrame(new DataFrameColumn[]
            {
            new PrimitiveDataFrameColumn<DateTime>(dateColumnName, filteredDates),
            new PrimitiveDataFrameColumn<double>(priceColumnName, filteredPrices)
            });
            return filteredDf;
        }

        void UpdateElectrocityStroreLabel(object value)//사용량의 데이터를 라벨과 슬라이더에 업데이트하는 메서드
        {
            if (checkDateChange == true )//전력거래소가 운영중인지 체크
            {
                try
                {
                    double maxElectorocityValue= Convert.ToDouble(value);

                    maxElectrocitySoldLabel.Content = "누적 전력 판매 금액 : "+maxElectorocityValue * powerTotal;

                    //MessageBox.Show($"적용된 doublevalue: {doubleValue},적용된 doublevalue2:{doubleValue2},적용된 doublevalue3:{doubleValue3}");
                }
                catch (InvalidCastException)
                {
                    Console.WriteLine("Object의 형식이 잘못되었습니다.");
                }
                catch (FormatException)
                {
                    Console.WriteLine("Object의 형식이 잘못되었습니다.");
                }
                catch (OverflowException)
                {
                    Console.WriteLine("값이 너무 큽니다.");
                }
                checkDateChange = false;//날짜가 변경 안됐다고 변경
            }
        }

        //관리자 모드
        public void login()
        {
            if (idTextBox.Text == adminID)
            {
                if (pwTextBox.Password == adminPW)
                {
                    MessageBox.Show("로그인 완료!");
                    loginGrid.Visibility = Visibility.Collapsed;
                    managerPage.Visibility = Visibility.Visible;
                }
                else
                {
                    MessageBox.Show("잘못된 비밀번호입니다.");
                }
            }
            else
            {
                MessageBox.Show("잘못된 아이디입니다.");
            }
        }

        //public void SetManagerPage()
        //{
        //    // 값 설정
        //    if (double.TryParse(valueTextBox.Text, out sliderLine1.Maximum))
        //    {
        //        // 슬라이더 값 설정
        //        slider.Value = sliderLine1.Maximum;
        //    }
        //    else
        //    {
        //        MessageBox.Show("Invalid value.");
        //    }
        //}

    }
}
