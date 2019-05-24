using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace A027_EcgPpg
{
  public partial class Form1 : Form
  {
    double[] ecg = new double[50000];
    double[] ppg = new double[50000];
    int ecgCount;
    int ppgCount;
    Timer t = new Timer();
    int cursorX; //로컬 변수는 초기화 X -> 0으로 만들어줘야된다.
                 // 현재 디스플레이되는 데이터의 시작점

    private bool scrolling; // 필드는 시작이 소문자, 메서드는 대문자

        public Form1()
    {
      InitializeComponent();

      this.Text = "ECG/PPG";
      this.WindowState = FormWindowState.Maximized;

      EcgRead();
      PpgRead();

      t.Interval = 10;
      t.Tick += T_Tick;     
         
        }

        private void T_Tick(object sender, EventArgs e)
        {
      
            if (cursorX + 500 <= ecgCount)
                chart1.ChartAreas["Draw"].AxisX.ScaleView.Zoom
                     (cursorX, cursorX + 500);
            else
                t.Stop();
            cursorX += 2;
        }

        private void PpgRead()
    {
      string fileName = "../../Data/ppg.txt";
      string[] lines = File.ReadAllLines(fileName);

      double min = double.MaxValue;
      double max = double.MinValue;

      int i = 0;
      foreach (var line in lines)
      {
        ppg[i] = double.Parse(line);
        if (min > ppg[i])
          min = ppg[i];
        if (max < ppg[i])
          max = ppg[i];
        i++;
      }
      ppgCount = i;
      string s = string.Format(
        "PPG: Count = {0}, min = {1}, max = {2}",
        ppgCount, min, max);
      MessageBox.Show(s);
    }

    private void EcgRead()
    {
      string fileName = "../../Data/ecg.txt";
      string[] lines = File.ReadAllLines(fileName);

      double min = double.MaxValue;
      double max = double.MinValue;

      int i = 0;
      foreach(var line in lines)
      {
        ecg[i] = double.Parse(line) + 3;
        if (min > ecg[i])
          min = ecg[i];
        if (max < ecg[i])
          max = ecg[i];
        i++;
      }
      ecgCount = i;
      string s = string.Format(
        "ECG: Count = {0}, min = {1}, max = {2}",
        ecgCount, min, max);
      MessageBox.Show(s);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);

      chart1.ChartAreas.Clear();
      chart1.Series.Clear();

      chart1.ChartAreas.Add("Draw");

      chart1.ChartAreas["Draw"].BackColor = Color.Black;
      chart1.ChartAreas["Draw"].AxisX.Minimum = 0;                                           //최소값
      chart1.ChartAreas["Draw"].AxisX.Maximum = ecgCount;
      chart1.ChartAreas["Draw"].AxisX.Interval = 50;
      chart1.ChartAreas["Draw"].AxisX.MajorGrid.LineColor = Color.Gray;                     // 선 색
      chart1.ChartAreas["Draw"].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;  //모눈선 스타일

      chart1.ChartAreas["Draw"].AxisY.Minimum = -2;
      chart1.ChartAreas["Draw"].AxisY.Maximum = 6;
      chart1.ChartAreas["Draw"].AxisY.Interval = 0.5;
      chart1.ChartAreas["Draw"].AxisY.MajorGrid.LineColor = Color.Gray;
      chart1.ChartAreas["Draw"].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;

      chart1.Series.Add("ECG");
      chart1.Series["ECG"].ChartType = SeriesChartType.Line;
      chart1.Series["ECG"].Color = Color.LightGreen;
      chart1.Series["ECG"].BorderWidth = 2;
      chart1.Series["ECG"].LegendText = "ECG";

      chart1.Series.Add("PPG");
      chart1.Series["PPG"].ChartType = SeriesChartType.Line;
      chart1.Series["PPG"].Color = Color.Orange;
      chart1.Series["PPG"].BorderWidth = 2;
      chart1.Series["PPG"].LegendText = "PPG";

      foreach (var v in ecg)
      {
        chart1.Series["ECG"].Points.Add(v);
      }

      foreach (var v in ppg)
      {
        chart1.Series["PPG"].Points.Add(v);
      }

    }

        //Auto Scroll 메뉴
        private void autoScrollToolStripMenuItem_Click(object sender, EventArgs e)
        {
            t.Start();
            scrolling = true;
        }

        private void chart1_Click(object sender, EventArgs e)
        {
            if(scrolling == true)
            {
                t.Stop();
                scrolling = false;
            }
            else
            {
                t.Start();
                scrolling = true;
            }
        }

        // View All 메뉴
        private void viewAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            t.Stop();
            scrolling = false;
            chart1.ChartAreas["Draw"].AxisX.ScaleView.Zoom
                     (0,ecgCount);
        }
    }
}
