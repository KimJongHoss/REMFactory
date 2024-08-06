using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.Analysis;
using System.Diagnostics.Eventing.Reader;
using ScottPlot.AxisLimitManagers;

namespace REMFactory
{
    public partial class MainWindow : Window
    {
        string adminID = "admin123";
        string adminPW = "admin123";
        public async Task getPanel2Data()
        {
            // Define data path
            var dataPath = Path.GetFullPath(@"제주특별자치도개발공사_제주삼다수공장 시간별 전력사용량_20230930.csv");

            // Load the data into the data frame
            var df = DataFrame.LoadCsv(dataPath);

            var dateGroupedData = new Dictionary<DateTime, List<List<object>>>();

            foreach (var row in df.Rows)
            {

                DateTime date;
                if (DateTime.TryParse(row[0].ToString(), out date))
                {
                    date = (DateTime)row[0]; // 첫 번째 컬럼이 날짜 컬럼이라고 가정
                }
                else
                {
                    // date 변환 실패 - 적절한 예외 처리 또는 로그 출력
                    throw new InvalidCastException("첫 번째 컬럼을 DateTime 형식으로 변환할 수 없습니다.");
                }
                var rowData = new List<object>();

                foreach (var cell in row.Skip(1)) // 날짜 컬럼을 제외한 나머지 셀
                {
                    rowData.Add(cell);
                }

                // 날짜별로 리스트에 추가
                if (!dateGroupedData.ContainsKey(date))
                {
                    dateGroupedData[date] = new List<List<object>>();
                }
                dateGroupedData[date].Add(rowData);
            }

            // 결과 출력
            foreach (var date in dateGroupedData.Keys)
            {
                Console.WriteLine($"Date: {date.ToShortDateString()}");
                foreach (var row in dateGroupedData[date])
                {
                    foreach (var value in row)
                    {

                        if (Dispatcher.CheckAccess())
                        {
                            UpdateUI(value);//label value와 slider value를 바꾸는 메서드 
                        }
                        else
                        {
                            Dispatcher.Invoke(() => UpdateUI(value));
                        }

                        // 20ms 대기
                        await Task.Delay(500);
                    }
                }
            }
        }

        void UpdateUI(object value)
        {
            labelLine1.Content = value.ToString();
            labelLine2.Content = value.ToString();
            labelLine3.Content = value.ToString();

            if (double.TryParse(value.ToString(), out double doubleValue))
            {
                sliderLine1.Value = doubleValue;
                sliderLine2.Value = doubleValue;
                sliderLine3.Value = doubleValue;
            }
            else
            {
                // value를 double로 변환할 수 없는 경우에 대한 예외 처리 또는 로그 출력
                Console.WriteLine("double로 처리할 수 없습니다.");
            }
        }

        public void login()
        {
            if (idTextBox.Text == adminID)
            {
                if (pwTextBox.Password == adminPW)
                {
                    MessageBox.Show("로그인 완료!");
                    loginGrid.Visibility = Visibility.Collapsed;
                    managerPage.Visibility = Visibility.Visible;
                }
                else
                {
                    MessageBox.Show("잘못된 비밀번호입니다.");
                }
            }
            else
            {
                MessageBox.Show("잘못된 아이디입니다.");
            }
        }

        //public void SetManagerPage()
        //{
        //    // 값 설정
        //    if (double.TryParse(valueTextBox.Text, out sliderLine1.Maximum))
        //    {
        //        // 슬라이더 값 설정
        //        slider.Value = sliderLine1.Maximum;
        //    }
        //    else
        //    {
        //        MessageBox.Show("Invalid value.");
        //    }
        //}

    }
}
