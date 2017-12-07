
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
using System.Numerics;

namespace DynamicalSystemsNet.Examples.Nodes
{
    /// <summary>
    /// Special subclass of DynamicalNode, for Hebbian learning for neural oscillators.
    /// </summary>
    public class NeuralOscillatorConnectionNode : DynamicalNode
    {
        public NodeLink Connection { get; protected set; }

        public NeuralOscillatorConnectionNode(
            double initialTime, 
            NodeLink connection)
            : base(0.0, 0.0)
        {
            this.Connection = connection;
        }

        public override System.Numerics.Complex F(double t, VectorOI y)
        {
            // A Dynamical Systems Approach to Musical Tonality. Equation 11.
            // Musical Tonality, Neural Resonance, and Hebbian Learning. Equation 3 (w/fix to assumed typo).


            NeuralOscillatorNode @in = Connection.From as NeuralOscillatorNode;
            NeuralOscillatorNode @out = Connection.To as NeuralOscillatorNode;
            Complex z_i = @in.Value;
            Complex z_j = @out.Value;
            Complex z_j_conj = Complex.Conjugate(z_j);

            double epsilon = (@in.Nonlinearity + @out.Nonlinearity) * 0.5;

            Complex c_dot;

            Complex c = y[this];
            double delta = this.Connection.Decay;
            double k = this.Connection.Plasticity;
            

            Complex sqrtEpsZi = Complex.Sqrt(epsilon * z_i);
            Complex sqrtEpsZjConj = Complex.Sqrt(epsilon * z_j_conj);

            Complex term1 = -delta * c;
            Complex term2 = k;
            Complex term3 = z_i / (1 - sqrtEpsZi);
            Complex term4 = z_j_conj / (1 - sqrtEpsZjConj);

            c_dot = term1 + term2 * term3 * term4;

            return c_dot;
        }
        
    }
}
