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
        public static Dictionary<DateTime, double> dateDictionary { get; set; } = new Dictionary<DateTime, double>();

        public void chart1()
        {
            var mapper = Mappers.Xy<MeasureModel>()
                .X(model => model.X.ToOADate())                 //use DateTime.Ticks as X
                .Y(model => model.Value);                       //use the value property as Y

            //lets save the mapper globally.
            Charting.For<MeasureModel>(mapper);

            //the values property will store our values array
            ChartValues1 = new ChartValues<MeasureModel>();
            ChartValues2 = new ChartValues<MeasureModel>();
            ChartValues3 = new ChartValues<MeasureModel>();
            ChartValues4 = new ChartValues<MeasureModel>();


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
        public Func<double, string> XAxisLabelFormatter => value =>
        {
            DateTime dateTime = DateTime.FromOADate(value);
            return dateTime.ToString("HH"); // Format as needed
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
            var minElec = (efficiency + efficiency2 + efficiency3) * 8; // 최소 전력
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

                SetAxisLimits(today);

                //lets only use the last 150 values
                Application.Current.Dispatcher.Invoke(() =>
                {
                    AddToChart(ChartValues1, model1);
                    AddToChart(ChartValues2, model2);
                    AddToChart(ChartValues3, model3);
                    AddToChart(ChartValues4, model4);

                    SetAxisLimits(today);

                    if (ChartValues1.Count > 1000) ChartValues1.RemoveAt(0);
                    if (ChartValues2.Count > 1000) ChartValues2.RemoveAt(0);
                    if (ChartValues3.Count > 1000) ChartValues3.RemoveAt(0);
                    if (ChartValues4.Count > 1000) ChartValues4.RemoveAt(0);
                    label3.Text = "TODAY :" + dates[count].Date.ToString("yyyy-MM-dd");


                });

                
                if (x % 24 == 0)
                {
                    sumPower = (powerTotal - minElec);
                    powerTotal = minElec;
                    dateDictionary.Add(dates[count].Date, sumPower);
                    //dateResult = dateDictionary[dates[count].Date].ToString("n2");
                    //Application.Current.Dispatcher.Invoke(() => {
                    //label1.Text = "누적 판매 전력량 :" + dateResult;
                        
                    //});

                }
                count++;
                x++;

            }

        }
        private void AddToChart(ChartValues<MeasureModel> chartValues, MeasureModel newValue)
        {
            chartValues.Add(newValue);
            if (chartValues.Count > 15) // Limit to 15 values
            {
                chartValues.RemoveAt(0); // Remove the oldest value
            }
        }

        private void SetAxisLimits(DateTime currentDateTime)
        {
            AxisMax = currentDateTime.AddHours(10).ToOADate(); // Add 10 hours to the current date/time
            AxisMin = currentDateTime.AddHours(-5).ToOADate(); // Subtract 5 hours from the current date/time
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