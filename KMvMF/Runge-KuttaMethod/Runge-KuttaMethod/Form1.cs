using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Runge_KuttaMethod
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            Func<double[], double> f1 = parameters => parameters[2];
            Func<double[], double> f2 = parameters => parameters[1];

            var functions = new[] { f1, f2 };

            var solver = new Solver(functions, -2, new double[] { 1, 0 }, 4.0 / 500, 501);
            var results = solver.Solve();

            for (var i = 0; i < 501; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    textBox1.Text += String.Format("{0:0.000}", results[i][j])+"; ";
                }
                textBox1.Text += Environment.NewLine;
            }
            
            chart1.Series.Add(new Series());
            var x = chart1.Series[0];
            x.Color = Color.Red;
            var y = chart1.Series[1];
            y.Color = Color.Blue;
            x.ChartType = SeriesChartType.Line;
            y.ChartType = SeriesChartType.Line;
            for (int i = 0; i < 501; i++)
            {
                x.Points.AddXY(results[i][0], results[i][1]);
                y.Points.AddXY(results[i][0], results[i][2]);
            }
        }
    }
}
