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
        public DateTime X { get; set; }
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
        private double _trend1;
        private double _trend2;
        private double _trend3;
        public List<DateTime> dates = new List<DateTime>();

        public static Dictionary<DateTime, double> dateDictionary { get; set; } = new Dictionary<DateTime, double>();


        public void chart1()
        {
            var mapper = Mappers.Xy<MeasureModel>()
                .X(model => model.X.ToOADate())                 // DateTime.Ticks 을 X로 사용
                .Y(model => model.Value);                       //value값을 Y로 사용

            var soldMapper = Mappers.Xy<MeasureSoldModel>()
               .X(model => model.X.ToOADate())                 // DateTime.Ticks 을 X로 사용
               .Y(model => model.Value);                       //value값을 Y로 사용

            //lets save the mapper globally.
            Charting.For<MeasureModel>(mapper);
            Charting.For<MeasureSoldModel>(soldMapper);

            //the values property will store our values array
            ChartValues1 = new ChartValues<MeasureModel>();
            ChartValues2 = new ChartValues<MeasureModel>();
            ChartValues3 = new ChartValues<MeasureModel>();
            ChartValues4 = new ChartValues<MeasureModel>();
            ChartValues5 = new ChartValues<MeasureSoldModel>();
            ChartValues6 = new ChartValues<MeasureModel>();
            ChartValues7 = new ChartValues<MeasureModel>();
            ChartValues8 = new ChartValues<MeasureModel>();

            AxisStep = 1 / 24.0; // X축의 시간 간격 설정
            AxisUnit = 1; // X축 단위 설정

            SetAxisLimits(DateTime.Now);


            IsReading = false;

            DataContext = this;

        }
        public ChartValues<MeasureModel> ChartValues1 { get; set; }
        public ChartValues<MeasureModel> ChartValues2 { get; set; }
        public ChartValues<MeasureModel> ChartValues3 { get; set; }
        public ChartValues<MeasureModel> ChartValues4 { get; set; }
        public ChartValues<MeasureSoldModel> ChartValues5 { get; set; }
        public ChartValues<MeasureModel> ChartValues6 { get; set; }
        public ChartValues<MeasureModel> ChartValues7 { get; set; }
        public ChartValues<MeasureModel> ChartValues8 { get; set; }
        public Func<double, string> XAxisLabelFormatter => value =>
        {
            DateTime dateTime = DateTime.FromOADate(value);
            return dateTime.ToString("HH");                // 그래프 x축 시간 표현
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
            var dataPath2 = System.IO.Path.GetFullPath(@"한국서부발전(주)_태양광 발전 현황_20230630.csv");
            var df = DataFrame.LoadCsv(dataPath2);

            var dfLine1 = df.Rows.Where(row => (row["발전기명"].ToString().Contains("태양광1") == true) && (((DateTime)row["년월일"]).Year == 2023)).ToList();
            var dfLine2 = df.Rows.Where(row => (row["발전기명"].ToString().Contains("태양광2") == true) && (((DateTime)row["년월일"]).Year == 2023)).ToList();
            var dfLine3 = df.Rows.Where(row => (row["발전기명"].ToString().Contains("태양광3") == true) && (((DateTime)row["년월일"]).Year == 2023)).ToList();

            List<Double> listRyu = new List<Double>();

            List<double> power = new List<double>();
            List<Double> dfLine1List = new List<Double>();
            List<Double> dfLine2List = new List<Double>();
            List<Double> dfLine3List = new List<Double>();
            for (int i = 0; i < dfLine1.Count(); i++)
            {
                for (int j = 3; j < dfLine1[0].Count(); j++)
                {
                    listRyu.Add(Convert.ToDouble(dfLine1[i][j]) + Convert.ToDouble(dfLine2[i][j]) + Convert.ToDouble(dfLine3[i][j]));
                    dfLine1List.Add(Convert.ToDouble(dfLine1[i][j]));
                    dfLine2List.Add(Convert.ToDouble(dfLine2[i][j]));
                    dfLine3List.Add(Convert.ToDouble(dfLine3[i][j]));
                }

            }

            for (int i = 0; i < dfLine1.Count(); i++)
            {
                for (int j = 1; j <= 24; j++)
                {
                    dates.Add(Convert.ToDateTime(dfLine1[i][1]).AddHours(j));
                }
            }
            var minElec = (efficiency + efficiency2 + efficiency3) * 8; // 전력이 생산 되지 않는 시간동안 가동에 필요한 최소 전력
            int count = 0;
            int x = 1;
            getPanel2Data();
            getefficiencyData();

            while (IsReading)
            {
                Thread.Sleep(1000);


                nowTime = x;
                today = dates[count];
                _trend1 = dfLine1List[count];
                _trend2 = dfLine2List[count];
                _trend3 = dfLine3List[count];


                powerTotal += listRyu[count] / 10;

                powerTotal -= (doubleValue + doubleValue2 + doubleValue3);
                GaugeValue = powerTotal;
                var model1 = new MeasureModel
                {
                    X = today,
                    Value = powerTotal
                };

                var model2 = new MeasureModel
                {
                    X = today,
                    Value = doubleValue
                };
                var model3 = new MeasureModel
                {
                    X = today,
                    Value = doubleValue2
                };
                var model4 = new MeasureModel
                {
                    X = today,
                    Value = doubleValue3
                };
                var model5 = new MeasureSoldModel
                {
                    X = openTime,
                    Value = devideValue
                };
                var model6 = new MeasureModel
                {
                    X = today,
                    Value = _trend1
                };
                var model7 = new MeasureModel
                {
                    X = today,
                    Value = _trend2
                };
                var model8 = new MeasureModel
                {
                    X = today,
                    Value = _trend3
                };
                SetAxisLimits(today);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    ChartValues1.Add(model1);
                    ChartValues2.Add(model2);
                    ChartValues3.Add(model3);
                    ChartValues4.Add(model4);
                    ChartValues6.Add(model6);
                    ChartValues7.Add(model7);
                    ChartValues8.Add(model8);


                    SetAxisLimits(today);

                    if (ChartValues1.Count > 15) ChartValues1.RemoveAt(0);     // 그래프의 y값이 15개가 넘어갈 경우 인덱스0 값 삭제
                    if (ChartValues2.Count > 15) ChartValues2.RemoveAt(0);
                    if (ChartValues3.Count > 15) ChartValues3.RemoveAt(0);
                    if (ChartValues4.Count > 15) ChartValues4.RemoveAt(0);
                    if (ChartValues6.Count > 15) ChartValues6.RemoveAt(0);
                    if (ChartValues7.Count > 15) ChartValues7.RemoveAt(0);
                    if (ChartValues8.Count > 15) ChartValues8.RemoveAt(0);
                    label3.Text = dates[count].Date.ToString("yyyy-MM-dd");


                });


                if (x % 24 == 0)
                {
                    if (powerTotal > minElec)
                    {
                        sumPower = (powerTotal - minElec);
                        powerTotal = minElec;
                    }
                    else
                    {
                        sumPower = 0;
                    }
                    dateDictionary.Add(dates[count].Date, sumPower);
                    cumulativeElectrocity += sumPower;
                    checkDateChange = true;
                    ElectrocityStore();
                }
                count++;
                x++;

            }

        }

        private void SetAxisLimits(DateTime currentDateTime)
        {
            AxisMax = currentDateTime.AddHours(10).ToOADate(); // 10시간 후 데이터
            AxisMin = currentDateTime.AddHours(-5).ToOADate(); // 5시간 전 데이터 까지 표현 그래프에 표현
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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}