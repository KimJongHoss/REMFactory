using LiveCharts.Defaults;
using LiveCharts.Wpf;
using LiveCharts;
using Microsoft.Data.Analysis;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace REMFactory
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            chart1();

            //getPanel2Data();
        }

        private void slider_valueChanged(object sender, RoutedEventArgs e)
        {

        }

        private void slider_valueChanged1(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sliderLine1 != null && labelLine1 != null && efficiency != 0)
            {
                doubleValue = sliderLine1.Value;
                labelLine1.Text = "라인 A 전력 : " + doubleValue.ToString("F0") + " KW";
                double efficiencySlider1Value = doubleValue / efficiency * 100;
                labelEfficiencyLine1.Text = efficiencySlider1Value.ToString("F2");
                UpdateProgress1(pathLine1, doubleValue);
            }
        }

        private void slider_valueChanged2(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sliderLine2 != null && labelLine2 != null && efficiency != 0)
            {
                doubleValue2 = sliderLine2.Value;
                labelLine2.Text = "라인 B 전력 : " + doubleValue2.ToString("F0") + " KW";
                double efficiencySlider2Value = doubleValue2 / efficiency * 100 / 1.5;
                labelEfficiencyLine2.Text = efficiencySlider2Value.ToString("F2");
                UpdateProgress2(pathLine2, doubleValue2);
            }
        }

        private void slider_valueChanged3(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sliderLine3 != null && labelLine3 != null && efficiency != 0)
            {
                doubleValue3 = sliderLine3.Value;
                labelLine3.Text = "라인 C 전력 : " + doubleValue3.ToString("F0") + " KW";
                double efficiencySlider3Value = doubleValue3 / efficiency * 100 / 2;
                labelEfficiencyLine3.Text = efficiencySlider3Value.ToString("F2");
                UpdateProgress3(pathLine3, doubleValue3);
            }
        }
        private void UpdateProgress1(Path path, double value)
        {
            double angle = value / 10000 * 360;
            double radius = 90;
            double center = 100;

            PathFigure pathFigure = new PathFigure();
            pathFigure.StartPoint = new Point(center, center - radius);

            ArcSegment arcSegment = new ArcSegment();
            arcSegment.Point = new Point(center + radius * Math.Sin(angle * Math.PI / 180), center - radius * Math.Cos(angle * Math.PI / 180));
            arcSegment.Size = new Size(radius, radius);
            arcSegment.IsLargeArc = angle > 180;
            arcSegment.SweepDirection = SweepDirection.Clockwise;

            pathFigure.Segments.Add(arcSegment);

            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures.Add(pathFigure);

            path.Data = pathGeometry;

            UpdateBorderColor(value, 10000, boderLine1);
        }
        private void UpdateProgress2(Path path, double value)
        {
            double angle = value / 15000 * 360;
            double radius = 90;
            double center = 100;

            PathFigure pathFigure = new PathFigure();
            pathFigure.StartPoint = new Point(center, center - radius);

            ArcSegment arcSegment = new ArcSegment();
            arcSegment.Point = new Point(center + radius * Math.Sin(angle * Math.PI / 180), center - radius * Math.Cos(angle * Math.PI / 180));
            arcSegment.Size = new Size(radius, radius);
            arcSegment.IsLargeArc = angle > 180;
            arcSegment.SweepDirection = SweepDirection.Clockwise;

            pathFigure.Segments.Add(arcSegment);

            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures.Add(pathFigure);

            path.Data = pathGeometry;

            UpdateBorderColor(value, 15000, boderLine2);
        }
        private void UpdateProgress3(Path path, double value)
        {
            double angle = value / 20000 * 360;
            double radius = 90;
            double center = 100;

            PathFigure pathFigure = new PathFigure();
            pathFigure.StartPoint = new Point(center, center - radius);

            ArcSegment arcSegment = new ArcSegment();
            arcSegment.Point = new Point(center + radius * Math.Sin(angle * Math.PI / 180), center - radius * Math.Cos(angle * Math.PI / 180));
            arcSegment.Size = new Size(radius, radius);
            arcSegment.IsLargeArc = angle > 180;
            arcSegment.SweepDirection = SweepDirection.Clockwise;

            pathFigure.Segments.Add(arcSegment);

            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures.Add(pathFigure);

            path.Data = pathGeometry;

            UpdateBorderColor(value, 20000, boderLine3);
        }

        private void managerLoginButton_Click(object sender, RoutedEventArgs e)
        {
            login();
        }

        private void lineAtext_Click(object sender, RoutedEventArgs e)
        {
            OpenChartWindow();
        }
        private void OpenChartWindow()
        {
            // Ensure dateDictionary has data before opening the chart window
            //if (dateDictionary.Count == 0)
            //{
            //    MessageBox.Show("데이터가 없습니다.");
            //    return;
            //}

            var chartWindow = new ChartWindow1(ChartValues6, ChartValues7, ChartValues8, dates);
            chartWindow.Show();
        }

       
    }
}
