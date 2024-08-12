﻿using Microsoft.Data.Analysis;
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
                labelLine1.Text = "라인 A 전력 : " + doubleValue.ToString("F0");
                double efficiencySlider1Value = sliderLine1.Value / efficiency * 100;
                labelEfficiencyLine1.Text = efficiencySlider1Value.ToString("F2");
                UpdateProgress(pathLine1, doubleValue);
                UpdateShoeValues();
            }
        }

        private void slider_valueChanged2(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sliderLine2 != null && labelLine2 != null && efficiency != 0)
            {
                doubleValue2 = sliderLine2.Value;
                labelLine2.Text = "라인 B 전력 : " + doubleValue2.ToString("F0");
                double efficiencySlider2Value = sliderLine2.Value / efficiency * 100;
                labelEfficiencyLine2.Text = efficiencySlider2Value.ToString("F2");
                UpdateProgress(pathLine2, doubleValue2);
                UpdateShoeValues();
            }
        }

        private void slider_valueChanged3(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sliderLine3 != null && labelLine3 != null && efficiency != 0)
            {
                doubleValue3 = sliderLine3.Value;
                labelLine3.Text = "라인 C 전력 : " + doubleValue3.ToString("F0");
                double efficiencySlider3Value = sliderLine3.Value / efficiency * 100;
                labelEfficiencyLine3.Text = efficiencySlider3Value.ToString("F2");
                UpdateProgress(pathLine3, doubleValue3);
                UpdateShoeValues();
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

            ChartWindow1 chartWindow = new ChartWindow1(this);
            chartWindow.Show();
        }
    }
}