using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Runge_KuttaMethod
{
    public partial class Form1 : Form
    {
        private double[] _u;
        private double[] _r;
        private double[] _e;
        private double[] _m;
        private double _g;
        private double _gn;

        private Func<double[], double>[] _functions;

        //private double U(double x)
        //{
        //    return 0.91 * Math.Sin(x) * Math.Cos(x / 4.0);
        //}

        private double Q(double e, double x)
        {
            return 2.0 * (e - U(x));
        }

        private double U(double x)
        {
            return 800;
        }

        private double R(int i, double x)
        {
            return _u[i] * Math.Exp(23 - _e[i] / U(x));
        }

        private double P(double x)
        {
            return 5 - x / 60;
        }

        private double V(double x, double[] parameters)
        {
            var v = 5 - 509.209*P(x);
            var sum = 0.0;
            for (var i = 0; i < 6; i++)
            {
                var sumInternal = 0.0;
                for (var j = 1; j < 6; j++)
                {
                    sumInternal += _m[j] * parameters[j] / _m[0];
                }

                sum += _m[i] * parameters[i] / (U(x) * (_g + _gn * (sumInternal)));
            }

            return v * sum;
        }

        private void Init()
        {
            _u = new double[]
            {
                15.19,
                8.18,
                13.198,
                2.543,
                4723.7,
                423.7,
                204.41,
                1.466 * Math.Pow(10, -6),
                0.013,
                0.09,
                5.428 * Math.Pow(10, -6),
                0.024,
                5.92 * Math.Pow(10, -6)
            };

            _e = new double[13];
            _e[0] = _e[1] = _e[2] = _e[3] = _e[4] = 25 * Math.Pow(10, 3);
            _e[5] = _e[6] = _e[7] = 40 * Math.Pow(10, 3);
            _e[8] = _e[9] = _e[10] = _e[11] = _e[12] = _e[13] = 20 * Math.Pow(10, 3);

            _m = new double[] { 84, 56, 42, 28, 92, 16, 18 };
            _g = 1750;
            _gn = 3500;



            Func<double[], double> f1 = parameters => -(R);
            Func<double[], double> f2 = parameters => parameters[1];
        }

        public Form1()
        {
            InitializeComponent();

            Func<double[], double> f1 = parameters => -Q(0.89, parameters[0]) * parameters[2];
            Func<double[], double> f2 = parameters => parameters[1];

            _functions = new[] { f1, f2 };

            var solver = new Solver(functions, 2, new double[] { 1, 0 }, -4.0 / 500, 501);
            var results = solver.Solve();

            for (var i = 0; i < 501; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    textBox1.Text += String.Format("{0:0.000}", results[i][j]) + "; ";
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
