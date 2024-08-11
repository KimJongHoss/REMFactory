using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Globalization;
using System.Windows.Input;

namespace REMFactory
{

    public partial class MainWindow : Window
    {
        bool isBlinking = false;
        DispatcherTimer blinkTimer;
        private double efficiencyData1;
        private double efficiencyData2;
        private double efficiencyData3;
        private void buttonExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(); // WPF 애플리케이션 종료
        }
        private void OpenLink_Click(object sender, RoutedEventArgs e)
        {
            string url = "https://github.com/KimJongHoss/REMFactory";  // 여기에 원하는 URL을 입력하세요
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true  // 이 옵션은 .NET Core와 .NET Framework에서 URL을 열 때 필요합니다
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to open link: " + ex.Message);
            }
        }
        private void redsign()
        {
            if (doubleValue > 80)
            {
                boderLine1.Background = new SolidColorBrush(Colors.Red);
            }
            else
            {
                boderLine1.Background = new SolidColorBrush(Color.FromRgb(60, 60, 66)); // original color #FF3C3C42
            }

            if (doubleValue2 > 80)
            {
                boderLine2.Background = new SolidColorBrush(Colors.Red);
            }
            else
            {
                boderLine2.Background = new SolidColorBrush(Color.FromRgb(60, 60, 66)); // original color #FF3C3C42
            }

            if (doubleValue3 > 80)
            {
                boderLine3.Background = new SolidColorBrush(Colors.Red);
            }
            else
            {
                boderLine3.Background = new SolidColorBrush(Color.FromRgb(60, 60, 66)); // original color #FF3C3C42
            }

            if (doubleValue > 5 && doubleValue2 > 5 && doubleValue3 > 5)
            {
                StartBlinking(gridMain);
            }
            else
            {
                StopBlinking(gridMain);
            }
            void StartBlinking(Grid gridMain)
            {
                if (blinkTimer == null)
                {
                    blinkTimer = new DispatcherTimer();
                    blinkTimer.Interval = TimeSpan.FromMilliseconds(500); // 500ms 간격으로 깜빡임
                    blinkTimer.Tick += (s, e) =>
                    {
                        if (isBlinking)
                        {
                            gridMain.Background = new SolidColorBrush(Colors.Red);
                        }
                        else
                        {
                            gridMain.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255)); // original color #FF3C3C42
                        }
                        isBlinking = !isBlinking;
                    };
                }
                if (!blinkTimer.IsEnabled)
                {
                    blinkTimer.Start();
                }
            }

            void StopBlinking(Grid gridMain)
            {
                if (blinkTimer != null && blinkTimer.IsEnabled)
                {
                    blinkTimer.Stop();
                    gridMain.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255)); // original color #FF3C3C42
                    isBlinking = false;
                }
            }
        }
    }
}