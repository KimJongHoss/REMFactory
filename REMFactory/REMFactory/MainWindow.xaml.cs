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
            
            BasicColumn();
            //getPanel2Data();
        }

        private void slider_valueChanged(object sender, RoutedEventArgs e)
        {
        }

        private void slider_valueChanged1(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sliderLine1 != null && labelLine1 != null)
            {
                labelLine1.Text = sliderLine1.Value.ToString();
                double efficiencySlider1Value = sliderLine1.Value / efficiency * 100;
                labelEfficiencyLine1.Text = efficiencySlider1Value.ToString();
                UpdateProgress(pathLine1, usePower1);
            }
        }

        private void slider_valueChanged2(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sliderLine2 != null && labelLine2 != null)
            {
                labelLine2.Text = sliderLine2.Value.ToString();
                double efficiencySlider2Value = sliderLine2.Value / efficiency * 100;
                labelEfficiencyLine2.Text = efficiencySlider2Value.ToString();
                UpdateProgress(pathLine2, usePower2);
            }
        }

        private void slider_valueChanged3(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sliderLine3 != null && labelLine3 != null)
            {
                labelLine3.Text = sliderLine3.Value.ToString();
                double efficiencySlider3Value = sliderLine3.Value / efficiency * 100;
                labelEfficiencyLine3.Text = efficiencySlider3Value.ToString();
                UpdateProgress(pathLine3, usePower3);
            }
        }

        private void UpdateProgress(Path path, double value)
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
        }

        private void UpdateTotalProgress(Path path, double value)
        {
            double angle = value / 20000000 * 360;
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
        }

        private void managerLoginButton_Click(object sender, RoutedEventArgs e)
        {
            login();
        }

        private void lineAtext_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (ChartValues1 == null || ChartValues2 == null || ChartValues3 == null || ChartValues4 == null)
            {
                MessageBox.Show("No data available to display.");
                return;
            }

            // Open the new chart window
            var chartWindow = new ChartWindow1(ChartValues1, ChartValues2, ChartValues3, ChartValues4);
            chartWindow.Show();
        }
    }
}