
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

using System.Collections.Generic;

namespace DynamicalSystemsNet.Core
{
    /// <summary>
    /// Defines the supported properties and methods of a numerical integrator.
    /// </summary>
    public interface IIntegrator
    {
        /// <summary>
        /// Total accumulation of deltaTime steps thus far.
        /// </summary>
        double Time { get; set; }

        /// <summary>
        /// The nodes which this integrator is aware of.
        /// </summary>
        IEnumerable<DynamicalNode> Nodes { get; set; }

        /// <summary>
        /// Instructs the integrator to advance the system.
        /// </summary>
        void Step(double deltaTime);
    }
}
