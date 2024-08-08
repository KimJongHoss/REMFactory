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
            LabelElec();
            //getPanel2Data();
        }

        private void slider_valueChanged(object sender, RoutedEventArgs e)
        {
            if (sliderLine1 != null && sliderLine2 != null && sliderLine3 != null &&
                labelLine1 != null && labelLine2 != null && labelLine3 != null)
            {
                //sliderTotal.Value = _trend1;
                //sliderLine1.Value = _trend2;
                //sliderLine2.Value = _trend3;
                //sliderLine3.Value = _trend4;

                //labelTotal.Text = sliderTotal.Value.ToString();
                labelLine1.Text = sliderLine1.Value.ToString();
                labelLine2.Text = sliderLine2.Value.ToString();
                labelLine3.Text = sliderLine3.Value.ToString();

                labelTotal.Text = _trend1.ToString();
                //labelLine1.Text = _trend2.ToString();
                //labelLine2.Text = _trend3.ToString();
                //labelLine3.Text = _trend4.ToString();

                UpdateTotalProgress(pathTotal, _trend1);
                if (_trend1 - (_trend2 + _trend3 + _trend4) > 0)
                {
                    UpdateProgress(pathLine1, _trend2);
                    UpdateProgress(pathLine2, _trend3);
                    UpdateProgress(pathLine3, _trend4);
                }
                
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