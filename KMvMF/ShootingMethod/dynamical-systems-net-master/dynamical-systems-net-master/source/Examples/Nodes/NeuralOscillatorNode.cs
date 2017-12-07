
//    This file is part of Dynamical Systems Net.
//
//    Dynamical Systems Net is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Lesser General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    Dynamical Systems Netis distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public License
//    along with Dynamical Systems Net.  If not, see <http://www.gnu.org/licenses/>.

using DynamicalSystemsNet.Core;
using System;
using System.Numerics;

namespace DynamicalSystemsNet.Examples.Nodes
{
    public class NeuralOscillatorNode : DynamicalNode
    {
        public double Omega { get; set; }
        public double Alpha { get; set; }
        public double Beta1 { get; set; }
        public double Beta2 { get; set; }
        public double Delta1 { get; set; }
        public double Delta2 { get; set; }
        public Complex A
        {
            get
            {
                return new Complex(this.Alpha, this.Omega);
            }
        }
        public Complex B
        {
            get
            {
                return new Complex(this.Beta1, Delta1);
            }
        }
        public Complex D
        {
            get
            {
                return new Complex(this.Beta2, Delta2);
            }
        }


        /// <summary>
        /// The center (natural) frequency.
        /// Also written as 1 / τ.
        /// </summary>
        public double CenterFrequency { get; set; }

        /// <summary>
        /// The nonlinearity factor.
        /// Aslo written as ε (epsilon).
        /// </summary>
        public double Nonlinearity { get; set; }


        public NeuralOscillatorNode(
            Complex initialValue, 
            double centerFrequency, 
            double nonlinearity,
            double alpha,
            double beta1,
            double beta2,
            double delta,
            double omega)
            : base(0.0, initialValue)
        {
            this.CenterFrequency = centerFrequency;
            this.Nonlinearity = nonlinearity;
            this.Alpha = alpha;
            this.Beta1 = beta1;
            this.Beta2 = beta2;
            this.Delta1 = delta;
            this.Delta2 = delta;
            this.Omega = omega;

            _sqrtInvEpsilon = Math.Sqrt(1.0 / nonlinearity);
        }

        public override System.Numerics.Complex F(double t, VectorOI y)
        {
            // "A canonical model for gradient...". Large 2010. Equation 20.
            // tau * z-dot = z * ( 
            //                   a + b * |z|^2 + (d * eps * |z|^4) / (1 - eps * |z|^2)
            //                 ) 
            //           + x / (1 - x * (sqrt eps))   *   1 / (1 - z * (sqrt eps))
            //            

            // "Musical Tonality, Neural Resonance and Hebbian Learning. Large 2011. Equation 2.
            // z-dot = z * (
            //               alpha + i*omega + (beta1 + 
            //

            
            // Compute total stimulus.
            Complex x = this.X(t, y);

            // Grab current value of this node.
            Complex z = y[this];
            Complex zConj = Complex.Conjugate(z);

            //Debug.Assert(x.Magnitude < _sqrtInvEpsilon);
            //Debug.Assert(z.Magnitude < _sqrtInvEpsilon);

            // Compute sqrt(eps).
            double eps = this.Nonlinearity;
            double sqrtEps = Math.Sqrt(this.Nonlinearity);

            // Compute |z|.
            double zAbs = Complex.Abs(z);

            // Compute |z|^2 and |z|^4.
            double zAbs2 = zAbs * zAbs;
            double zAbs4 = zAbs2 * zAbs2;

            // Gather other constants.
            Complex a = this.A;
            Complex b = this.B;
            Complex d = this.D;
            double tau = 1.0 / this.CenterFrequency;
            //double tau = Math.PI * 2.0 / this.CenterFrequency;
            double tauInv = 1.0 / tau;
            
            // Compute parts.
            Complex part1 = z * (a + b * zAbs2 + (d * eps * zAbs4) / (1.0 - eps * zAbs2));
            Complex part2 = x / (1.0 - x * sqrtEps);
            Complex part3 = 1.0 / (1.0 - zConj * sqrtEps);

            // Combine.
            Complex zDot = tauInv * (part1 + part2 * part3);

            // Done.
            return zDot;
        }

        /// <summary>
        /// Computes the total "stimulus" contribution from
        /// incoming nodes.
        /// </summary>
        protected Complex X(double t, VectorOI y)
        {
            Complex sum = 0.0;

            foreach (var link in this.IncomingNodes)
            {
                var node = link.From;
                Complex weight = link.Weight;
                Complex nodeValue = y[node];

                Complex nodeTotal = nodeValue * weight;
                sum += nodeTotal;
            }

            return sum;
        }


        protected readonly double _sqrtInvEpsilon;
    }
}
