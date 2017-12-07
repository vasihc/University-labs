using System;

namespace Runge_KuttaMethod
{
    public class Solver
    {
        private readonly Func<double[], double>[] _functions;
        double _startPoint;
        double[] _startValues;
        private double[][] _nodes;
        private int _equationsCount;
        private int _nodesCount;
        private int _parametersCount;
        private double _h;

        private double K1(double[] parameters, int i)
        {
            return _functions[i](parameters);
        }

        private double K2(double[] parameters, int i)
        {
            var newParameters = new double[_parametersCount];
            parameters.CopyTo(newParameters, 0);
            newParameters[0] += _h / 2;
            for (var k = 1; k <= _equationsCount; k++)
            {
                newParameters[k] += K1(parameters, k - 1) * _h / 2;
            }

            return _functions[i](newParameters);
        }

        private double K3(double[] parameters, int i)
        {
            var newParameters = new double[_parametersCount];
            parameters.CopyTo(newParameters, 0);
            newParameters[0] += _h / 2;
            for (var k = 1; k <= _equationsCount; k++)
            {
                newParameters[k] += K2(parameters, k - 1) * _h / 2;
            }

            return _functions[i](newParameters);
        }

        private double K4(double[] parameters, int i)
        {
            var newParameters = new double[_parametersCount];
            parameters.CopyTo(newParameters, 0);
            newParameters[0] += _h;
            for (var k = 1; k <= _equationsCount; k++)
            {
                newParameters[k] += _h * K3(parameters, k - 1);
            }

            return _functions[i](newParameters);
        }

        private double yNext(double[] parameters, int i)
        {
            return parameters[i + 1] + _h / 6.0 * (K1(parameters, i) + 2 * K2(parameters, i) + 2 * K3(parameters, i) + K4(parameters, i));
        }

        public Solver(Func<double[], double>[] equations, double startPoint, double[] startValues, double h, int nodesCount)
        {
            _equationsCount = equations.Length;
            _parametersCount = _equationsCount + 1;
            _nodesCount = nodesCount;
            _h = h;
            _functions = equations;
            _startPoint = startPoint;
            _startValues = startValues;
            
            _nodes = new double[nodesCount][];
        }

        public double[][] Solve()
        {
            _nodes[0] = new double[_parametersCount];
            _nodes[0][0] = _startPoint;

            for (int i = 1; i < _parametersCount; i++)
            {
                _nodes[0][i] = _startValues[i - 1];
            }

            for (int k = 1; k < _nodesCount; k++)
            {
                _nodes[k] = new double[_parametersCount];
                _nodes[k][0] = _startPoint + k * _h;
                for (int j = 0; j < _equationsCount; j++)
                {
                    _nodes[k][j + 1] = yNext(_nodes[k - 1], j);
                }
            }

            return _nodes;
        }
    }
}
