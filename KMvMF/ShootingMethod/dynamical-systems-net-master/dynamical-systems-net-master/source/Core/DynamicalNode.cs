
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
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace DynamicalSystemsNet.Core
{
    /// <summary>
    /// Represents a variable in a system of differential equations.
    /// </summary>
    [DebuggerDisplay("{Name}")]
    public class DynamicalNode
    {
        /// <summary>
        /// An arbtirary name for this node.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// The amount of history to keep for this node.
        /// </summary>
        public int HistorySize
        {
            get;
            set;
        }

        /// <summary>
        /// Read-only access to this nodes history.
        /// </summary>
        public IEnumerable<Complex> History
        {
            get
            {
                if (_history == null)
                {
                    return new Complex[] { this.Value };
                }
                else
                {
                    return _history;
                }
            }
        }

        /// <summary>
        /// The average magnitude over the history of this node.
        /// </summary>
        public double AverageMagnitude
        {
            get
            {
                return this.History.Average(v => (double?)v.Magnitude) ?? 0.0;
            }
        }

        /// <summary>
        /// Mutable access to the list of incoming nodes i.e. the list of other
        /// variables in the system on which this node depends.
        /// </summary>
        public IList<NodeLink> IncomingNodes
        {
            get;
            protected set;
        }

        /// <summary>
        /// The current value of this node.
        /// </summary>
        public Complex Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                if (this.HistorySize > 0)
                {
                    if (_history == null)
                    {
                        _history = new Queue<Complex>();
                    }

                    while (this.HistorySize <= _history.Count)
                    {
                        _history.Dequeue();
                    }

                    _history.Enqueue(value);
                }
            }
        }

        /// <summary>
        /// Creates a new node with given initial time, initial value, and name.
        /// </summary>
        public DynamicalNode(double initialTime, Complex initialValue, string name = null)
        {
            this.Name = name ?? "Anonymous Node";
            this.IncomingNodes = new List<NodeLink>();
            this.Value = initialValue;
        }

        /// <summary>
        /// Addes a new incoming node to this node.
        /// </summary>
        public bool AddIncomingNode(DynamicalNode node)
        {
            return AddIncomingNode(node, 1.0, 0, 0);
        }
        
        /// <summary>
        /// Addes a new incoming node to this node, with given decay and plasticity values.
        /// </summary>
        public bool AddIncomingNode(DynamicalNode node, Complex weight, double decay, double plasticity)
        {
            if (this.IncomingNodes.Any(link => link.From == node))
            {
                // Exists.
                return false;
            }

            this.IncomingNodes.Add(new NodeLink(this, node, weight, decay, plasticity));
            return true;
        }

        /// <summary>
        /// Function F for only this node (scalar return value).
        /// </summary>
        public virtual Complex F(double t, VectorOI y)
        {
            return 0.0;
        }


        private Complex _value;
        private Queue<Complex> _history;
    }
}
