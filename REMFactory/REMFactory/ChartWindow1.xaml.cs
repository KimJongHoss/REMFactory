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
        public SeriesCollection SeriesCollection { get; set; }
        public Func<double, string> XFormatter { get; set; }
        public Func<double, string> YFormatter { get; set; }
        //private List<DateTime> _dates;
        public ChartWindow1(ChartValues<MeasureModel> chartValues6, ChartValues<MeasureModel> chartValues7,
                            ChartValues<MeasureModel> chartValues8, List<DateTime> dates)
        {

            InitializeComponent();

            var _dates = dates;

            var series1 = new LineSeries
            {
                Title = "태양광 1",
                Values = chartValues6,
                LineSmoothness = 0,

            };
            var series2 = new LineSeries
            {
                Title = "태양광 2",
                Values = chartValues7,
                LineSmoothness = 0,
            };
            var series3 = new LineSeries
            {
                Title = "태양광 3",
                Values = chartValues8,
                LineSmoothness = 0,
            };


            Chart.Series = new SeriesCollection { series1, series2, series3 };

            XFormatter = value =>
            {
                DateTime dateTime = DateTime.FromOADate(value);
                return dateTime.ToString("yy-MM-dd HH:mm"); // Format DateTime as needed
            };

            DataContext = this;
        }
    }
}