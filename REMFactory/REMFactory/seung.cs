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
        public double shoeValue = 50;
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
                    DataLabels = true
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
        private void UpdateShoeValues()
        {
            double shoe1 = doubleValue / shoeValue;
            double shoe2 = doubleValue2 / shoeValue;
            double shoe3 = doubleValue3 / shoeValue;

            shoe1TextBlock.Text = $"Shoe 1: {shoe1:F0}";
            shoe2TextBlock.Text = $"Shoe 2: {shoe2:F0}";
            shoe3TextBlock.Text = $"Shoe 3: {shoe3:F0}";
        }
    }
 }
