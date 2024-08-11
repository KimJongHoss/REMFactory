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

namespace REMFactory
{
    public class MeasureModel
    {
        public System.DateTime DateTime { get; set; }
        public double Value { get; set; }
    }
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        private double _axisMax;
        private double _axisMin;
        private double powerTotal;
        private double usePower1;
        private double usePower2;
        private double usePower3;
        private double _gaugeValue;
        private double sumPower;
        private DateTime nowTime;
        private Dictionary<DateTime, double> dateDictionary = new Dictionary<DateTime, double>();

        public void chart1()
        {
            var mapper = Mappers.Xy<MeasureModel>()
                .X(model => model.DateTime.Ticks)   //use DateTime.Ticks as X
                .Y(model => model.Value);           //use the value property as Y

            //lets save the mapper globally.
            Charting.For<MeasureModel>(mapper);

            //the values property will store our values array
            ChartValues1 = new ChartValues<MeasureModel>();
            ChartValues2 = new ChartValues<MeasureModel>();
            ChartValues3 = new ChartValues<MeasureModel>();
            ChartValues4 = new ChartValues<MeasureModel>();


            //lets set how to display the X LabelElecs
            DateTimeFormatter = value => new DateTime((long)value).ToString("HH");

            //AxisStep forces the distance between each separator in the X axis
            AxisStep = TimeSpan.FromSeconds(1).Ticks;
            //AxisUnit forces lets the axis know that we are plotting seconds
            //this is not always necessary, but it can prevent wrong labeling
            AxisUnit = TimeSpan.TicksPerSecond;

            SetAxisLimits(DateTime.Now);

            //The next code simulates data changes every 300 ms

            IsReading = false;

            DataContext = this;

        }
        public ChartValues<MeasureModel> ChartValues1 { get; set; }
        public ChartValues<MeasureModel> ChartValues2 { get; set; }
        public ChartValues<MeasureModel> ChartValues3 { get; set; }
        public ChartValues<MeasureModel> ChartValues4 { get; set; }
        public Func<double, string> DateTimeFormatter { get; set; }
        public double AxisStep { get; set; }
        public double AxisUnit { get; set; }

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
            
            getPanel2Data();
            getefficiencyData();
            
            while (IsReading)
            {
                Thread.Sleep(1000);
               

                nowTime = DateTime.Now;


                powerTotal += listRyu[count] / 10;
                powerTotal -= (doubleValue + doubleValue2 + doubleValue3);
                GaugeValue = powerTotal;
                var model1 = new MeasureModel
                {
                    DateTime = nowTime,
                    Value = powerTotal
                };

                var model2 = new MeasureModel
                {
                    DateTime = nowTime,
                    Value = doubleValue
                };
                var model3 = new MeasureModel
                {
                    DateTime = nowTime,
                    Value = doubleValue2
                };
                var model4 = new MeasureModel
                {
                    DateTime = nowTime,
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

                   
                });
                count++;
                if (count % 24 == 0)
                {
                    sumPower += (powerTotal - 50000);
                    powerTotal = 50000;
                    MessageBox.Show(sumPower.ToString());
                    dateDictionary.Add(dates[count], sumPower);
                    

                }

            }
            
        }
        private void SetAxisLimits(DateTime now)
        {
            AxisMax = now.Ticks + TimeSpan.FromSeconds(5).Ticks; // lets force the axis to be 1 second ahead
            AxisMin = now.Ticks - TimeSpan.FromSeconds(24).Ticks; // and 8 seconds behind
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

        public void BasicColumn()
        {
            List<string> dateChart2 = new List<string>();
            List<double> powerChart2 = new List<double>();
            foreach (KeyValuePair<DateTime, double> item in dateDictionary)
            {
                dateChart2.Add(item.Key.ToString("yyyy-MM-dd"));
                powerChart2.Add(item.Value);
            }


            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "누적 전력량",
                    Values = new ChartValues<double>(powerChart2)
                }
            };

            //adding series will update and animate the chart automatically
            SeriesCollection.Add(new ColumnSeries
            {
                Title = "2016",
                Values = new ChartValues<double> { 11, 56, 42 }
            });

            //also adding values updates and animates the chart automatically


            Labels = dateChart2.ToArray();
            Formatter = value => value.ToString("N");

            DataContext = this;
        }

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }

    }
}

