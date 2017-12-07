using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using MathNet.Numerics;
using MathNet.Numerics.Interpolation;
using MathNet.Numerics.Statistics;
using Runge_KuttaMethod;

    /// <summary>
    /// Электрон находится в потенциальном поле U(x), x=[-L,L].
    /// Найти энергии, нормированные волновые функции, плотности вероятности 
    /// для основного и 1 возбужденного состояний.
    /// Вычислить квантовомеханимечские средние x, x*x в этих состояниях
    /// </summary> 
    
namespace ShootingMethod
{
    public partial class Form1 : Form
    {

        private double _v0; //1 эВ = 0,0367493 Eh
        private double _l; //1 А = 10e-10 м
        private double[] _conditionValuesForward;
        private double[] _conditionValuesBackward;
        private double[] _psi;
        private double[] _phi;
        private double[] _xQuantAvg;
        private double[] _xSqrQuantAvg;
        private double[] _t; 
        private int _sewNode;
        private int _nodesCount;
        private double _h;
        private Func<double[], double>[] _systemForward;
        private Func<double[], double>[] _systemBackward;
        private CubicSpline _psiSpline;
        private CubicSpline _phiSpline;
        private Dictionary<int, double[]> _shootingValues;
        private List<double[]> _eigenvalues;
        private List<double> _probalilityDensities;
        private List<double[]> _waveFunctions;
        
        private double U(double x)
        {
            return Math.Abs(x)<_l?_v0 * Math.Sin(x) * Math.Cos(x / 4.0): 1;
        }
        
        private double Q(double e, double x)
        {
            return 2.0 * (e - U(x));
        }

        private void InitSystems(double e)
        {
            Func<double[], double> f1 = parameters => -Q(e, parameters[0]) * parameters[2];
            Func<double[], double> f = parameters => parameters[1];

            _conditionValuesForward = new double[] { 1, 0 };
            _conditionValuesBackward = new double[] { 1, 0 };

            _systemForward = new[] { f1, f };
            _systemBackward = new[] { f1, f };
        }

        private double GetUmin()
        {
            var u = new double[_nodesCount];
            for (var i = 0; i < _nodesCount; i++)
            {
                u[i] = U(-_l + i * _h);
            }
            return u.Min();
        }

        private double GetStepX()
        {
            return _l * 2 / (Constants.NodesCount - 1);
        }

        private int GetSewNode()
        {
            return (_nodesCount - 1) * 3 / 4;
        }

        private void InitData()
        {
            _v0 = Constants.V0 * Constants.EhFactor; 
            _l = Constants.L; 
            _nodesCount = Constants.NodesCount;
            _h = GetStepX();
            _sewNode = GetSewNode();

            var step = Constants.StepBisection;
            var e = new double[Constants.EnergiesCount];

            _xQuantAvg = new double[_nodesCount];
            _xSqrQuantAvg = new double[_nodesCount];
            _shootingValues = new Dictionary<int, double[]>();
            _eigenvalues = new List<double[]>();
            _probalilityDensities = new List<double>();
            _waveFunctions = new List<double[]>();
        }

        private double CountWaveFunctions(double e)
        {
            InitSystems(e);

            _t = new double[_nodesCount];
            _psi = new double[_nodesCount];
            _phi = new double[_nodesCount];

            var solver = new Solver(_systemForward, -_l, _conditionValuesForward, _h, _nodesCount);
            var solutionPsi = solver.Solve();

            for (int i = 0; i < _nodesCount; i++)
            {
                _t[i] = solutionPsi[i][0];
                _psi[i] = solutionPsi[i][2];
            }

            solver = new Solver(_systemBackward, _l, _conditionValuesBackward, -_h, _nodesCount);
            var solutionFi = solver.Solve();

            for (int i = 0; i < _nodesCount; i++)
            {
                _phi[i] = solutionFi[_nodesCount - 1 - i][2];
            }
            
            Scale(_psi);
            NormalizeMath();
            NormalizeQuant();

            _psiSpline = CubicSpline.InterpolateAkima(_t, _psi);
            _phiSpline = CubicSpline.InterpolateAkima(_t, _phi);

            return _psiSpline.Differentiate(_t[_sewNode]) - _phiSpline.Differentiate(_t[_sewNode]);
        }

        private void NormalizeMath()
        {
            var factor = _psi[_sewNode] / _phi[_sewNode];
            for (var i = 0; i < _nodesCount; i++)
            {
                _phi[i] *= factor;
            }
        }

        private void NormalizeQuant()
        {
            
            var factorPsi = GetProbalilityDensity(_psi);
            var factorPhi = GetProbalilityDensity(_phi);
            
            for (var i = 0; i < _nodesCount; i++)
            {
                _psi[i] /= Math.Sqrt(factorPsi);
                _phi[i] /= Math.Sqrt(factorPhi);
            }
        }

        private double GetProbalilityDensity(double[] function)
        {
            var functionSquare = new double[_nodesCount];

            for (var i = 0; i < _nodesCount; i++)
            {
                functionSquare[i] = function[i] * function[i];
            }

            var functionSquareSpline = CubicSpline.InterpolateAkima(_t, functionSquare);

            var x = functionSquareSpline.Integrate(-_l, _l);
            return functionSquareSpline.Integrate(-_l, _l);
        }

        private void Scale(double[] function)
        {
            var max = function.MaximumAbsolute();
            for (var i = 0; i < _nodesCount; i++)
            {
                function[i] /= max;
            }
        }

        private void DrawGraphs()
        {
            chart1.Series.Clear();

            var psiSeries = new Series() { ChartType = SeriesChartType.Line, Color = Color.MediumVioletRed, BorderWidth = 3, LegendText = "ψ(x)" };
            var phiSeries = new Series() { ChartType = SeriesChartType.Line, Color = Color.LightSkyBlue, BorderWidth = 1, BorderDashStyle = ChartDashStyle.Dot, LegendText = "φ(x)" };
            var uSeries = new Series { ChartType = SeriesChartType.Line, Color = Color.LightSeaGreen, BorderWidth  = 4,  LegendText = "U(x)"};
            var uVerticalLeft = new Series { ChartType = SeriesChartType.Line, Color = Color.LightSeaGreen, IsVisibleInLegend = false};
            var uVerticalRight = new Series { ChartType = SeriesChartType.Line, Color = Color.LightSeaGreen, IsVisibleInLegend = false };

            var diffSeries = new Series { ChartType = SeriesChartType.Line, Color = Color.Pink };
            
            var nodesOfSewingSeries = new Series { ChartType = SeriesChartType.Point, Color = Color.Yellow, LegendText = "Узел сшивки" };

            nodesOfSewingSeries.Points.AddXY(_t[_sewNode], _psi[_sewNode]);

            for (var i = 0; i < _nodesCount; i++)
            {
                psiSeries.Points.AddXY(_t[i], _psi[i]);
                phiSeries.Points.AddXY(_t[i], _phi[i]);
                uSeries.Points.AddXY(_t[i], U(_t[i]));
                diffSeries.Points.AddXY(_t[i], _psiSpline.Differentiate(_t[i]) - _phiSpline.Differentiate(_t[i]));
            }

            uVerticalLeft.Points.AddXY(-5, U(-5));
            uVerticalLeft.Points.AddXY(-5, 1);

            uVerticalRight.Points.AddXY(5, 1);
            uVerticalRight.Points.AddXY(5, U(-5));

            chart1.Series.Add(psiSeries);
            chart1.Series.Add(phiSeries);
            chart1.Series.Add(nodesOfSewingSeries);
            chart1.Series.Add(uSeries);
            chart1.Series.Add(diffSeries);
            chart1.Series.Add(uVerticalLeft);
            chart1.Series.Add(uVerticalRight);
        }

        private double BisectionMethod(double e1, double e2)
        {
            while (Math.Abs(e2 - e1) > Constants.StepBisection)
            {
                var c = (e1 + e2) / 2;
                if (CountWaveFunctions(e2) * CountWaveFunctions(c) < 0)
                {
                    e1 = c;
                }
                else
                {
                    e2 = c;
                }

                if (CountWaveFunctions(e1) * CountWaveFunctions(c) < 0)
                {
                    e2 = c;
                }
                else
                {
                    e1 = c;
                }
            }
            return (e1 + e2) / 2;
        }

        private void Shoot()
        {
            var energyStartValue = GetUmin() + Constants.OffsetEnergy;
            var energies = new double[_nodesCount];

            for (var i = 0; i < Constants.EnergiesCount; i++)
            {
                energies[i] = energyStartValue + i * Constants.StepEnergy;
            }

            _shootingValues.Add(0, new double[] { energies[0], CountWaveFunctions(energies[0]) });

            for (int i = 1; i < Constants.EnergiesCount; i++)
            {

                var probabilityDensity = GetProbalilityDensity(_psi);
                var diffCurr = CountWaveFunctions(energies[i]);
                var diffPrev = CountWaveFunctions(energies[i - 1]);
                _shootingValues.Add(i, new [] { energies[i], diffCurr });
                
                if (diffCurr * diffPrev < 0)
                {
                    var diff = Math.Abs(diffCurr - diffPrev);
                    _eigenvalues.Add(diff < Constants.Threshold
                        ? new []
                        {
                            diffPrev, diffCurr, diff,
                            BisectionMethod(energies[i], energies[i - 1]),
                            probabilityDensity,
                            CountXQuantAvg(),
                            CountXSquareQuantAvg()
                        }
                        : new [] {diffPrev, diffCurr, Math.Abs(diffCurr - diffPrev), 0, 0,0,0});
                    _waveFunctions.Add(_psi);
                }
            }
        }

        public Form1()
        {
            InitializeComponent();
            InitData();
            Shoot();

            FillGrid();
            CountWaveFunctions(-0.41846348822043);
            DrawGraphs();
        }

        private void FillGrid()
        {
            for (var i = 0; i < 3; i++)
            {
                var column = new DataGridViewColumn {CellTemplate = new DataGridViewTextBoxCell()};
                dgEnergy.Columns.Add(column);
            }

            var headers = new[] {"e[i-1]", "e[i]", "|f(i)-f(i-1)|", "Ek", "Плотность вероятности", "<x>", "<x*x>"};

            for (var i = 0; i < 7; i++)
            {
                var column = new DataGridViewColumn {CellTemplate = new DataGridViewTextBoxCell(), HeaderText = headers[i]};

                dgEigenvalues.Columns.Add(column);
            }

            //var column2 = new DataGridViewColumn();

            //column2.CellTemplate = new DataGridViewTextBoxCell();
            //dgEigenvalues.Columns.Add(column2);
            dgEigenvalues.CellClick += DgEigenvalues_CellClick;

            for (var i = 0; i < _shootingValues.Count; i++)
            {
                dgEnergy.Rows.Add(new DataGridViewRow());
            }

            for (var i = 0; i < _eigenvalues.Count; i++)
            {
                dgEigenvalues.Rows.Add(new DataGridViewRow());
            }

            for (var i = 0; i < _shootingValues.Count; i++)
            {
                dgEnergy[0, i].Value = i;

                for (var j = 1; j < 3; j++)
                {
                    dgEnergy[j, i].Value = string.Format("{0:0.000}", _shootingValues[i][j - 1]);
                    if (j == 2)
                    {
                        Color color;
                        if (_shootingValues[i][j - 1] < Math.Pow(10, -3) && _shootingValues[i][j - 1] > 0 ||
                            _shootingValues[i][j - 1] > -Math.Pow(10, -3) && _shootingValues[i][j - 1] < 0)
                        {
                            color = Color.LightGreen;
                        }
                        else if (_shootingValues[i][j - 1] > 0)
                        {
                            color = Color.LightSkyBlue;
                        }
                        else
                        {
                            color = Color.Pink;
                        }
                        dgEnergy[j, i].Style = new DataGridViewCellStyle {BackColor = color};
                    }
                }
            }

            for (var i = 0; i < _eigenvalues.Count; i++)
            for (var j = 0; j < 7; j++)
            {
                dgEigenvalues[j, i].Value = j > 2 && _eigenvalues[i][j] == 0 ? string.Empty
                    : String.Format("{0:0.000}",_eigenvalues[i][j].ToString());
                if (j == 2)
                {
                    dgEigenvalues[j, i].Style =
                        new DataGridViewCellStyle {BackColor = _eigenvalues[i][j] > Constants.Threshold ? Color.Pink : Color.LightGreen};
                }
                if (j == 3)
                {
                    dgEigenvalues[j, i].Style = new DataGridViewCellStyle
                    {
                        BackColor = Color.LightYellow
                    };
                }
            }
        }

        private double CountXQuantAvg()
        {
            for (var i = 0; i < _nodesCount; i++)
            {
                _xQuantAvg[i] = _psi[i] * _t[i] * _psi[i];
            }
            var xQiantAvgSpline = CubicSpline.InterpolateAkima(_t, _xQuantAvg);
            return xQiantAvgSpline.Integrate(-_l, _l);
        }

        private double CountXSquareQuantAvg()
        {
            for (var i = 0; i < _nodesCount; i++)
            {
                _xQuantAvg[i] = _psi[i] * _t[i]*_t[i] * _psi[i];
            }
            var xQiantAvgSpline = CubicSpline.InterpolateAkima(_t, _xQuantAvg);
            return xQiantAvgSpline.Integrate(-_l, _l);
        }

        private void DgEigenvalues_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 3)
            {
                return;
            }
            CountWaveFunctions(double.Parse(dgEigenvalues[e.ColumnIndex, e.RowIndex].Value.ToString()));
            CountXQuantAvg();
            DrawGraphs();
        }

    }


}
