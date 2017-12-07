
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
using System.Linq;
using System.Text;

namespace DynamicalSystemsNet.Core
{
    /// <summary>
    /// Represents a dynamical system e.g. a system of differential equations.
    /// </summary>
    public class DynamicalSystem
    {
        /// <summary>
        /// The currently active integrator for this system.
        /// </summary>
        public IIntegrator Integrator { get; protected set; }
        
        /// <summary>
        /// Read-only access to the nodes of this system.
        /// </summary>
        public IEnumerable<DynamicalNode> Nodes
        {
            get
            {
                return _nodes;
            }
            set
            {
                this._nodes = value.ToList();
                this.Integrator.Nodes = this._nodes;
            }
        }


        /// <summary>
        /// Creates a new dynamical system using the provided integrator.
        /// </summary>
        /// <param name="integrator"></param>
        public DynamicalSystem(IIntegrator integrator)
        {
            this.Integrator = integrator;
        }

        /// <summary>
        /// Advances the system forward in time by the given delta time. 
        /// </summary>
        public virtual void Step(double deltaTime)
        {
            this.Integrator.Step(deltaTime);
        }

        /// <summary>
        /// Overridable method for selecting the nodes of the system
        /// to use during printing the state of this system.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<DynamicalNode> SelectNodesForToString()
        {
            return this.Nodes;
        }

        /// <summary>
        /// Overridable method for printing the value of a node in this system.
        /// </summary>
        public virtual string FormatNodeValue(DynamicalNode node)
        {
            return node.Value.ToString("F3");
        }

        /// <summary>
        /// Overridable method for selecting the nominal "output" nodes of this system.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<DynamicalNode> SelectOutputLayer()
        {
            return this.Nodes;
        }

        /// <summary>
        /// Overridable method for printing the state of this system.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            RungeKuttaIntegrator rki = Integrator as RungeKuttaIntegrator;

            string description = string.Empty;

            if (rki != null)
            {
                StringBuilder buffer = new StringBuilder();
                buffer.AppendFormat("#: {1} \tτ: {0:F4}", rki.Time, rki.Iteration);
                foreach (var node in SelectNodesForToString())
                {
                    string nodeValueAsString = FormatNodeValue(node);
                    buffer.AppendFormat(" \t{0}: {1}", node.Name, nodeValueAsString);
                }
                description = buffer.ToString();
            }

            return description;
        }


        List<DynamicalNode> _nodes;
    }
}
