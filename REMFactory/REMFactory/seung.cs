using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Globalization;
using System.Windows.Input;
using LiveCharts.Defaults;
using LiveCharts.Definitions.Charts;
using static REMFactory.ChartWindow1;

namespace REMFactory
{

    public partial class MainWindow : Window
    {
        private bool isBlinkingRed = false;
        DispatcherTimer blinkingTimer;
        private double efficiencyData1;
        private double efficiencyData2;
        private double efficiencyData3;
        private void buttonExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(); // WPF 애플리케이션 종료
        }
        private void OpenLink_Click(object sender, RoutedEventArgs e)
        {
            string url = "https://github.com/KimJongHoss/REMFactory";  // 여기에 원하는 URL을 입력하세요
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true  // 이 옵션은 .NET Core와 .NET Framework에서 URL을 열 때 필요합니다
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to open link: " + ex.Message);
            }
        }
        private void UpdateBorderColor(double currentValue, double maxValue, Border targetBorder)
        {
            double percentage = currentValue / maxValue * 100;
            if (percentage >= 80)
            {
                StartBlinking(targetBorder);  // 80% 이상일 때 깜빡임 시작
            }
            else
            {
                StopBlinking(targetBorder);  // 80% 미만일 때 깜빡임 중지
            }
        }
        private Dictionary<string, DispatcherTimer> borderTimers = new Dictionary<string, DispatcherTimer>();

        private void StartBlinking(Border targetBorder)
        {
            string borderName = targetBorder.Name;

            // 타이머가 없으면 생성
            if (!borderTimers.ContainsKey(borderName))
            {
                DispatcherTimer newTimer = new DispatcherTimer();
                newTimer.Interval = TimeSpan.FromMilliseconds(500);
                newTimer.Tick += (sender, e) =>
                {
                    if (targetBorder.Background == Brushes.Red)
                        targetBorder.Background = new SolidColorBrush(Color.FromRgb(60, 60, 66));
                    else
                        targetBorder.Background = Brushes.Red;
                };
                borderTimers[borderName] = newTimer;
            }

            borderTimers[borderName].Start();
        }

        private void StopBlinking(Border targetBorder)
        {
            string borderName = targetBorder.Name;

            if (borderTimers.ContainsKey(borderName) && borderTimers[borderName].IsEnabled)
            {
                borderTimers[borderName].Stop();
                targetBorder.Background = new SolidColorBrush(Color.FromRgb(60, 60, 66));
            }
        }
        public class Data
        {
            public bool IsSelected { get; set; }
            public DateTime date { get; set; }
            public double powerResult { get; set; }
        }

        public class soldData
        {
            public bool IsSelected { get; set; }
            public DateTime date { get; set; }
            public double devideValue { get; set; }
        }

        private void loadData_Click(object sender, RoutedEventArgs e)   // Tab2 옮겨놈
            {
                if (MainWindow.dateDictionary.Count == 0)
                {
                    MessageBox.Show("MainWindow의 dateDictionary에 데이터가 없습니다.");
                    return;
                }

                List<Data> datas = new List<Data>();
                var ndic = MainWindow.dateDictionary;

                foreach (KeyValuePair<DateTime, double> item in ndic)
                {
                    datas.Add(new Data { date = item.Key, powerResult = item.Value });
                }

                datatList.ItemsSource = datas;
                var xAxis = new Axis
                {
                    Title = "Date",
                    LabelFormatter = value =>
                    {
                        var date = DateTime.FromOADate(value);
                        return date.ToString("MM/dd/yyyy");
                    },
                    Separator = new LiveCharts.Wpf.Separator
                    {
                        Step = 1, // 날짜 단위
                                  //Unit = TimeSpan.FromDays(1) // 1일 단위로 표시
                    }
                };

                var yAxis = new Axis
                {
                    Title = "Power Result"
                };

                // 시리즈 설정
                var series = new ColumnSeries
                {
                    Title = "Power Result",
                    Values = new ChartValues<ObservablePoint>(datas.Select(d => new ObservablePoint(
                        d.date.ToOADate(), d.powerResult // 날짜를 OADate로 변환
                    ))),
                    DataLabels = true,
                    LabelPoint = point => point.Y.ToString("N0")
                };

                // 차트에 시리즈와 축 추가
                cartesianChart.Series = new SeriesCollection { series };
                cartesianChart.AxisX.Clear();
                cartesianChart.AxisY.Clear();
                cartesianChart.AxisX.Add(xAxis);
                cartesianChart.AxisY.Add(yAxis);

                // X축 레이블 포맷 설정
                cartesianChart.AxisX[0].LabelFormatter = value =>
                {
                    var date = DateTime.FromOADate(value);
                    return date.ToString("MM/dd/yyyy");
                };
            }
        private void maxElectrocitySoldData_Click(object sender, RoutedEventArgs e)
        {
            // 판매한 전력이 없으면 경고 메시지를 표시
            if (MainWindow.soldDateDictionary == null || MainWindow.soldDateDictionary.Count == 0)
            {
                MessageBox.Show("판매한 전력이 없습니다.");
                return;
            }

            // soldDateDictionary에서 soldData 리스트를 생성
            List<soldData> soldDatas = new List<soldData>();
            var ndic2 = MainWindow.soldDateDictionary;

            foreach (KeyValuePair<DateTime, double> item in ndic2)
            {
                // soldData 객체를 생성하고 리스트에 추가
                soldDatas.Add(new soldData { date = item.Key, devideValue = item.Value });
            }

            // soldDatas를 리스트 뷰에 바인딩
            //maxElectrocitySoldList.ItemsSource = soldDatas;

            // X축 설정
            var xAxis = new Axis
            {
                Title = "Date",
                LabelFormatter = value =>
                {
                    // X축 값이 OADate 형식이므로 이를 날짜 형식으로 변환
                    var date = DateTime.FromOADate(value);
                    return date.ToString("MM/dd/yyyy");
                },
                Separator = new LiveCharts.Wpf.Separator
                {
                    Step = 1, // 날짜 단위로 구분
                }
            };

            // Y축 설정 (변경된 부분: 제목을 "Devide Value"로 변경)
            var yAxis = new Axis
            {
                Title = "Devide Value"
            };

            // 시리즈 설정 (변경된 부분: soldData의 devideValue를 사용)
            var series = new ColumnSeries
            {
                Title = "Devide Value",
                Values = new ChartValues<ObservablePoint>(soldDatas.Select(d => new ObservablePoint(
                    d.date.ToOADate(), d.devideValue // 날짜를 OADate로 변환하고 devideValue를 사용
                ))),
                DataLabels = true,
                LabelPoint = point => point.Y.ToString("N0")
            };

            // 차트에 시리즈와 축 추가
            maxElectrocitySoldChart.Series = new SeriesCollection { series };
            maxElectrocitySoldChart.AxisX.Clear();
            maxElectrocitySoldChart.AxisY.Clear();
            maxElectrocitySoldChart.AxisX.Add(xAxis);
            maxElectrocitySoldChart.AxisY.Add(yAxis);

            // X축 레이블 포맷 설정
            maxElectrocitySoldChart.AxisX[0].LabelFormatter = value =>
            {
                var date = DateTime.FromOADate(value);
                return date.ToString("MM/dd/yyyy");
            };
        }
    }

}
