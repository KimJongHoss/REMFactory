using LiveCharts;
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
        public ChartWindow1(ChartValues<MeasureModel> chartValues1, ChartValues<MeasureModel> chartValues2,
                           ChartValues<MeasureModel> chartValues3, ChartValues<MeasureModel> chartValues4)
        {
            InitializeComponent();

            // Create and configure series for the chart
            var series1 = new LineSeries
            {
                Title = "Series 1",
                Values = chartValues1
            };
            var series2 = new LineSeries
            {
                Title = "Series 2",
                Values = chartValues2
            };
            var series3 = new LineSeries
            {
                Title = "Series 3",
                Values = chartValues3
            };
            var series4 = new LineSeries
            {
                Title = "Series 4",
                Values = chartValues4
            };

            // Add series to chart
            Chart.Series = new SeriesCollection { series1};

            // Optionally configure axes, labels, etc.
            Chart.AxisX.Add(new Axis
            {
                Title = "Time",
                LabelFormatter = value => new DateTime((long)value).ToString("mm:ss")
            });
            Chart.AxisY.Add(new Axis
            {
                Title = "Value"
            });
        }
    }
}
