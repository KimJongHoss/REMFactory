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
            getPanel2Data();
        }

        private void slider_valueChanged(object sender, RoutedEventArgs e)
        {
            if (sliderLine1 != null && sliderLine2 != null && sliderLine3 != null &&
                labelLine1 != null && labelLine2 != null && labelLine3 != null)
            {
                labelLine1.Content = sliderLine1.Value;
                labelLine2.Content = sliderLine2.Value;
                labelLine3.Content = sliderLine3.Value;

                UpdateProgress(progressPath1, sliderLine1.Value);
                UpdateProgress(progressPath2, sliderLine2.Value);
                UpdateProgress(progressPath3, sliderLine3.Value);
            }
        }
        private void UpdateProgress(Path path, double value)
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
        }

        private void managerLoginButton_Click(object sender, RoutedEventArgs e)
        {
            login();
        }
    }
}