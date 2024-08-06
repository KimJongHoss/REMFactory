﻿using Microsoft.Win32;
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
using System.Collections;

namespace REMFactory
{
    public partial class MainWindow : Window
    {
        private string usingDataPath = Path.GetFullPath(@"제주특별자치도개발공사_제주삼다수공장_시간별_전력사용량_20230930.csv");//전력 사용량 csv 파일 path
        private double efficiency = 10000;//각 라인당 max 전력량
        private string adminID = "admin123";//관리자 아이디
        private string adminPW = "admin123";//관리자 비밀번호

        //사용량 데이터 가져와서 그래프로 띄우기
        private DataFrame LoadUsingDataFrame(string path)
        {
            // Check if file exists
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("The specified data file does not exist.", path);
            }

            // Load the data into the DataFrame
            return DataFrame.LoadCsv(path);
        }

        public async Task getPanel2Data()
        {
            var df = LoadUsingDataFrame(usingDataPath);//데이터를 가져와서 dataFrame으로 반환

            var dateGroupedData = new Dictionary<DateTime, List<List<object>>>();

            usingDataToListDictionary(df, dateGroupedData);//데이터프레임 리스트로 만들어서 딕셔너리에 정리

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
                            UpdateLabelAndSlider(value);//label value와 slider value를 바꾸는 메서드 
                        }
                        else
                        {
                            Dispatcher.Invoke(() => UpdateLabelAndSlider(value));
                        }

                        // 20ms 대기
                        await Task.Delay(500);
                    }
                }
            }
        }

        void UpdateLabelAndSlider(object value)
        {
            labelLine1.Text = value.ToString();
            labelLine2.Text = value.ToString();
            labelLine3.Text = value.ToString();

            if (double.TryParse(value.ToString(), out double doubleValue))//파싱 성공하면 value를 doubleValue에 넣는다
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

        public void getUsingData()
        {
            var dataPath = Path.GetFullPath(@"제주특별자치도개발공사_제주삼다수공장_시간별_전력사용량_20230930.csv");

            // Load the data into the data frame
            var df = DataFrame.LoadCsv(dataPath);
        }

        public void usingDataToListDictionary(DataFrame df, Dictionary<DateTime, List<List<object>>> dateGroupedData)
        {
            foreach (var row in df.Rows)
            {

                DateTime date;
                if (DateTime.TryParse(row[0].ToString(), out date))
                {
                    date = (DateTime)row[0]; // 첫 번째 컬럼이 날짜 컬럼이라고 가정

                    if (date.Month <= 6) //6월까지만 출력
                    {
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
                }
                else
                {
                    // date 변환 실패 - 적절한 예외 처리 또는 로그 출력
                    throw new InvalidCastException("첫 번째 컬럼을 DateTime 형식으로 변환할 수 없습니다.");
                }
            }
        }

        //각 라인당 전력 사용 효율

        public async Task getefficiencyData()
        {
            var df = LoadUsingDataFrame(usingDataPath);//데이터를 가져와서 dataFrame으로 반환

            var dateGroupedData = new Dictionary<DateTime, List<List<object>>>();

            usingDataToListDictionary(df, dateGroupedData);//데이터프레임 리스트로 만들어서 딕셔너리에 정리

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
                            howefficiencyData(value);//label value와 slider value를 바꾸는 메서드 
                        }
                        else
                        {
                            Dispatcher.Invoke(() => howefficiencyData(value));
                        }

                        // 20ms 대기
                        await Task.Delay(500);
                    }
                }
            }
        }

        void howefficiencyData(object value)
        {
            if (double.TryParse(value.ToString(), out double doubleValue))//파싱 성공하면 value를 doubleValue에 넣는다
            {
                double efficiencyData = doubleValue / efficiency * 100;
                labelEfficiencyLine1.Text = efficiencyData.ToString();
                labelEfficiencyLine2.Text = efficiencyData.ToString();
                labelEfficiencyLine3.Text = efficiencyData.ToString();
            }
            else
            {
                // value를 double로 변환할 수 없는 경우에 대한 예외 처리 또는 로그 출력
                Console.WriteLine("double로 처리할 수 없습니다.");
            }
        }

        //관리자 모드
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
