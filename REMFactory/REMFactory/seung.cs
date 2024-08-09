using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

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
    }
}