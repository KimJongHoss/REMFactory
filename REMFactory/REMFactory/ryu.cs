using Microsoft.Data.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using System.ComponentModel;
using System.Windows.Documents;
using ScottPlot.Palettes;
using static OpenTK.Graphics.OpenGL.GL;
using Dragablz.Dockablz;
using System.Windows.Media;


namespace REMFactory
{
    public class MeasureModel
    {
        public double X { get; set; }
        public double Value { get; set; }
    }
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        private double _axisMax;
        private double _axisMin;
        private double powerTotal;
        private double _gaugeValue;
        private double sumPower;
        private double nowTime;
        private DateTime today;
        public Dictionary<DateTime, double> dateDictionary { get; set; } = new Dictionary<DateTime, double>();

        public void chart1()
        {
            var mapper = Mappers.Xy<MeasureModel>()
                .X(model => model.X)   //use DateTime.Ticks as X
                .Y(model => model.Value);           //use the value property as Y

            //lets save the mapper globally.
            Charting.For<MeasureModel>(mapper);

            //the values property will store our values array
            ChartValues1 = new ChartValues<MeasureModel>();
            ChartValues2 = new ChartValues<MeasureModel>();
            ChartValues3 = new ChartValues<MeasureModel>();
            ChartValues4 = new ChartValues<MeasureModel>();

            //cartesianChart1.Series = new SeriesCollection
            //{
            //    new LineSeries
            //    {
            //        Title = "powerTotal",
            //        Values = ChartValues1,
            //        PointGeometrySize = 10,
            //        StrokeThickness = 2,
            //        LineSmoothness = 0,
            //        Stroke = Brushes.Red
            //    }
            //};
            //cartesianChart1.Series.Add(new LineSeries
            //{
            //    Title = "Line1",
            //    Values = ChartValues2, // 새 데이터
            //    PointGeometrySize = 10,
            //    StrokeThickness = 2,
            //    LineSmoothness = 0,
            //    Stroke = Brushes.Blue
            //});
            //cartesianChart1.Series.Add(new LineSeries
            //{
            //    Title = "Line2",
            //    Values = ChartValues3, // 새 데이터
            //    PointGeometrySize = 10,
            //    StrokeThickness = 2,
            //    LineSmoothness = 0,
            //    Stroke = Brushes.Brown
            //});
            //cartesianChart1.Series.Add(new LineSeries
            //{
            //    Title = "Line3",
            //    Values = ChartValues4, // 새 데이터
            //    PointGeometrySize = 10,
            //    StrokeThickness = 2,
            //    LineSmoothness = 0,
            //    Stroke = Brushes.Orange
            //});
            //lets set how to display the X LabelElecs
            //DateTimeFormatter = value => value.ToString("0");

            AxisStep = 1; // X축의 단위 설정
            AxisUnit = 1; // X축 단위 설정

            SetAxisLimits(1); // 시작 값을 1로 설정

            //The next code simulates data changes every 300 ms

            IsReading = false;

            DataContext = this;

        }
        public ChartValues<MeasureModel> ChartValues1 { get; set; }
        public ChartValues<MeasureModel> ChartValues2 { get; set; }
        public ChartValues<MeasureModel> ChartValues3 { get; set; }
        public ChartValues<MeasureModel> ChartValues4 { get; set; }
        public Func<double, string> XAxisLabelFormatter => value => {
            int intValue = (int)value;
            return ((intValue - 1) % 24 + 1).ToString(); // Ensures X-axis labels repeat from 1 to 24
        };
        public double AxisStep { get; set; }
        public double AxisUnit { get; set; }
        public string dateResult { get; set; }

        public double AxisMax
        {
            get { return _axisMax; }
            set
            {
                _axisMax = value;
                OnPropertyChanged("AxisMax");
            }
        }
        public double AxisMin
        {
            get { return _axisMin; }
            set
            {
                _axisMin = value;
                OnPropertyChanged("AxisMin");
            }
        }

        public double GaugeValue
        {
            get { return _gaugeValue; }
            set
            {
                _gaugeValue = value;
                OnPropertyChanged(nameof(GaugeValue));
            }
        }

        public bool IsReading { get; set; }

        private void Read()
        {
            var r = new Random();
            //var datapath1 = System.IO.Path.GetFullPath(@"제주특별자치도개발공사_제주삼다수공장_시간별_전력사용량_20230930.csv");
            var dataPath2 = System.IO.Path.GetFullPath(@"한국서부발전(주)_태양광 발전 현황_20230630.csv");
            //var dfJong = DataFrame.LoadCsv(datapath1);
            var df = DataFrame.LoadCsv(dataPath2);

            var dfLine1 = df.Rows.Where(row => (row["발전기명"].ToString().Contains("태양광1") == true) && (((DateTime)row["년월일"]).Year == 2023)).ToList();
            var dfLine2 = df.Rows.Where(row => (row["발전기명"].ToString().Contains("태양광2") == true) && (((DateTime)row["년월일"]).Year == 2023)).ToList();
            var dfLine3 = df.Rows.Where(row => (row["발전기명"].ToString().Contains("태양광3") == true) && (((DateTime)row["년월일"]).Year == 2023)).ToList();


            //var dfJong_1 = dfJong.Rows.Where(row => ((DateTime)row["일시"]).Month <= 6).ToList();
            List<Double> listRyu = new List<Double>();
            List<DateTime> dates = new List<DateTime>();
            List<double> power = new List<double>();
            //List<int> listJong = new List<int>();
            for (int i = 0; i < dfLine1.Count(); i++)
            {
                for (int j = 3; j < dfLine1[0].Count(); j++)
                {
                    listRyu.Add(Convert.ToDouble(dfLine1[i][j]) + Convert.ToDouble(dfLine2[i][j]) + Convert.ToDouble(dfLine3[i][j]));
                }

            }

            //for (int i = 0; i < dfJong_1.Count(); i++)
            //{
            //    for (int j = 1; j < dfJong_1[i].Count(); j++)
            //    {
            //        listJong.Add(Convert.ToInt32(dfJong_1[i][j]));
            //    }
            //}
            for (int i = 0; i < dfLine1.Count(); i++)
            {
                for (int j = 1; j <= 24; j++)
                {
                    dates.Add(Convert.ToDateTime(dfLine1[i][1]).AddHours(j));
                }
            }
            int count = 0;
            int x = 1;
            getPanel2Data();
            getefficiencyData();

            while (IsReading)
            {
                Thread.Sleep(1000);


                nowTime = x;
                today = dates[count];


                powerTotal += listRyu[count] / 10;
                powerTotal -= (doubleValue + doubleValue2 + doubleValue3);
                GaugeValue = powerTotal;
                var model1 = new MeasureModel
                {
                    X = nowTime,
                    Value = powerTotal
                };

                var model2 = new MeasureModel
                {
                    X = nowTime,
                    Value = doubleValue
                };
                var model3 = new MeasureModel
                {
                    X = nowTime,
                    Value = doubleValue2
                };
                var model4 = new MeasureModel
                {
                    X = nowTime,
                    Value = doubleValue3
                };

                SetAxisLimits(nowTime);

                //lets only use the last 150 values
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ChartValues1.Add(model1);
                    ChartValues2.Add(model2);
                    ChartValues3.Add(model3);
                    ChartValues4.Add(model4);
                    SetAxisLimits(nowTime);

                    if (ChartValues1.Count > 1000) ChartValues1.RemoveAt(0);
                    if (ChartValues2.Count > 1000) ChartValues2.RemoveAt(0);
                    if (ChartValues3.Count > 1000) ChartValues3.RemoveAt(0);
                    if (ChartValues4.Count > 1000) ChartValues4.RemoveAt(0);
                    label3.Text = "TODAY :" + dates[count].Date.ToString("yyyy-MM-dd");


                });
                
                
                if (x % 24 == 0)
                {
                    sumPower += (powerTotal - 50000);
                    powerTotal = 50000;
                    dateDictionary.Add(dates[count].Date, sumPower);
                    dateResult = dateDictionary[dates[count].Date].ToString();
                    Application.Current.Dispatcher.Invoke(() => {
                        label1.Text = "누적 판매 전력량 :" + dateResult.ToString();
                        
                    });

                }
                count++;
                x++;

            }

        }


        private void LabelElec()
        {
            labelElec.Text = "누적 판매 전력량 :" + sumPower.ToString();
        }
        private void SetAxisLimits(double currentX)
        {
            AxisMax = currentX + 10; // 현재 X 값에서 10을 더한 값으로 설정
            AxisMin = currentX - 5; // 현재 X 값에서 10을 뺀 값으로 설정
        }

        private void InjectStopOnClick(object sender, RoutedEventArgs e)
        {
            IsReading = !IsReading;
            if (IsReading) Task.Factory.StartNew(Read);
        }

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}