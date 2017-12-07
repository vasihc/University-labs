
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
    public class LinearNode : DynamicalNode
    {
        /// <summary>
        /// The coefficient for the 1st degree term.
        /// </summary>
        public Complex Coef1 { get; set; }
        /// <summary>
        /// The coefficient for the 0th degree term.
        /// </summary>
        public Complex Coef0 { get; set; }


        /// <summary>
        /// A linear node, summing the results of (coef1 * u + coef2), 
        /// for each incoming node u. Incoming weights are ignored.
        /// </summary>
        public LinearNode(Complex initialValue, Complex Coef1, Complex Coef2)
            : base(0, initialValue)
        {
            this.Coef1 = Coef1;
            this.Coef0 = Coef0;
        }

        public override Complex F(double t, VectorOI y)
        {
            Complex result = 0;
            foreach (var in_link in this.IncomingNodes)
            {
                var in_node = in_link.From;
                Complex in_value = y[in_node];
                Complex subtotal = in_value * this.Coef1 + this.Coef0;
                result += subtotal;
            }

            return result;
        }
    }
}
