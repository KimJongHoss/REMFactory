using ProductionLines;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace REMFactory
{
    public partial class MainWindow : Window
    {
        private double[] hourlyElectricity; // 하루의 전력 데이터
        private int currentHour = 0; // 현재 몇 번째 전력 데이터를 처리하는지

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void OnRunProductionClick(object sender, RoutedEventArgs e)
        {
            // Parameters
            int numLines = 3; // Number of production lines
            double initialProduction = 1; // Initial production per unit of electricity
            int[] thresholds = { 2000, 4000, 6000, 8000, 10000 }; // Thresholds where MPL decreases

            // Create production lines
            ProductionLine[] productionLines = new ProductionLine[numLines];
            for (int i = 0; i < numLines; i++)
            {
                productionLines[i] = new ProductionLine(initialProduction, thresholds);
            }

            // Read electricity data from CSV
            string filePath = @"제주특별자치도개발공사_제주삼다수공장 시간별 전력사용량_20230930.csv";
            MessageBox.Show($"Attempting to read file from path: {filePath}");

            try
            {
                hourlyElectricity = ReadElectricityFromCsv(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading file: {ex.Message}");
                return;
            }

            // Loop through each hour's electricity data
            for (currentHour = 0; currentHour < hourlyElectricity.Length; currentHour++)
            {
                double electricity = hourlyElectricity[currentHour];
                if (electricity > 0)
                {
                    AllocateElectricity(productionLines, electricity); // Allocate to the line with highest MPL first
                }

                // Create message content
                StringBuilder messageContent = new StringBuilder();
                messageContent.AppendLine($"Hour {currentHour + 1} Electricity Allocation:");

                for (int i = 0; i < numLines; i++)
                {
                    double totalProduction = productionLines[i].TotalProduction;
                    messageContent.AppendLine($"Production Line {i + 1}: Allocated Electricity = {electricity}, Total Production = {Math.Floor(totalProduction)}");
                }

                // Show message box and wait for user confirmation
                MessageBox.Show(messageContent.ToString());

                // Awaiting user confirmation before proceeding to the next hour
                await Task.Delay(100); // This ensures that UI can refresh properly after each message box
            }

            MessageBox.Show("All 24 hours of electricity data have been processed.");
        }

        static double[] ReadElectricityFromCsv(string filePath)
        {
            var electricityData = new List<double>();

            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    string line;
                    bool isFirstLine = true;
                    while ((line = reader.ReadLine()) != null)
                    {
                        // 첫 번째 행을 무시
                        if (isFirstLine)
                        {
                            isFirstLine = false;
                            continue;
                        }

                        var values = line.Split(',');

                        // 첫 번째 열을 무시하고 나머지 값을 읽음
                        for (int i = 1; i < values.Length; i++)
                        {
                            if (double.TryParse(values[i], NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
                            {
                                electricityData.Add(value);
                            }
                        }

                        // 한 번 실행할 때 한 줄의 데이터만 사용
                        break;
                    }
                }
            }
            catch (FileNotFoundException fnfe)
            {
                MessageBox.Show($"File not found: {fnfe.Message}");
                throw;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
                throw;
            }

            // 사용된 데이터의 개수가 24개가 아닌 경우 예외 처리
            if (electricityData.Count != 24)
            {
                throw new Exception("Each row in the CSV file must contain exactly 24 values.");
            }

            return electricityData.ToArray();
        }

        static void AllocateElectricity(ProductionLine[] lines, double electricity)
        {
            // Create a copy of the lines array sorted by current MPL in descending order
            var sortedLines = lines.OrderByDescending(line => line.CurrentMPL).ToList();

            while (electricity > 0)
            {
                // Allocate electricity in small units (e.g., 1 unit at a time)
                foreach (var line in sortedLines)
                {
                    if (electricity <= 0) break;
                    double allocatable = Math.Min(electricity, 1.0); // Allocate up to 1 unit at a time
                    line.AllocateElectricity(allocatable);
                    electricity -= allocatable;
                }

                // Sort again after allocation to ensure the next unit goes to the line with the highest MPL
                sortedLines = lines.OrderByDescending(line => line.CurrentMPL).ToList();
            }
        }
    }
}