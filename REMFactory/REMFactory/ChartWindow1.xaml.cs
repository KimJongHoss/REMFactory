using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Definitions.Charts;
using LiveCharts.Wpf;
using LiveCharts.Wpf.Charts.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace REMFactory
{
    /// <summary>
    /// ChartWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ChartWindow1 : Window
    {
        private MainWindow _mainWindow;
        public ChartWindow1(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
        }
        public class Data
        {
            public DateTime date { get; set; }
            public double powerResult { get; set; }
        }
        private void loadData_Click(object sender, RoutedEventArgs e)
        {
            if (_mainWindow == null)
            {
                MessageBox.Show("MainWindow 인스턴스가 없습니다.");
                return;
            }

            if (_mainWindow.dateDictionary.Count == 0)
            {
                MessageBox.Show("MainWindow의 dateDictionary에 데이터가 없습니다.");
                return;
            }

            List<Data> datas = new List<Data>();
            var ndic = _mainWindow.dateDictionary;

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


    }
}
