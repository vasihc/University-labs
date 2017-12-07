
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

using System.Diagnostics;
using System.Numerics;

namespace DynamicalSystemsNet.Core
{
    /// <summary>
    /// Represents a link (dependency) of on node (variable) on another.
    /// </summary>
    [DebuggerDisplay("{From.Name} -> {To.Name}")]
    public class NodeLink
    {
        /// <summary>
        /// A reference to the destination node.
        /// </summary>
        public DynamicalNode To
        {
            get;
            protected set;
        }

        /// <summary>
        /// A reference to the source node.
        /// </summary>
        public DynamicalNode From
        {
            get;
            protected set;
        }

        /// <summary>
        /// The weight for this connection.
        /// Often called 'c' or 'c_ij' in
        /// mathematical notation.
        /// </summary>
        public Complex Weight
        {
            get;
            set;
        }

        /// <summary>
        /// The plasticity of this connection's weight.
        /// </summary>
        public double Plasticity
        {
            get;
            set;
        }

        /// <summary>
        /// The decay of this connection's weight.
        /// </summary>
        public double Decay
        {
            get;
            set;
        }

        /// <summary>
        /// Constructs a new NodeLink.
        /// </summary>
        public NodeLink(DynamicalNode to, DynamicalNode from, Complex weight, double decay, double plasticity)
        {
            this.To = to;
            this.From = from;
            this.Weight = weight;
            this.Decay = decay;
            this.Plasticity = plasticity;
        }
    }
}
