
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
using DynamicalSystemsNet.Examples.Nodes;

namespace DynamicalSystemsNet.Examples.Systems
{
    public class LinearCoupleSystem : DynamicalSystem
    {
        public LinearCoupleSystem(IIntegrator integrator)
            : base(integrator)
        {
            LinearNode linear1 = new LinearNode(1, 2, 0);
            linear1.Name = "u";
            LinearNode linear2 = new LinearNode(0, -1, 0);
            linear2.Name = "v";

            linear1.AddIncomingNode(linear2);
            linear2.AddIncomingNode(linear1);

            this.Nodes = new[] { linear1, linear2 };
        }
    }
}
