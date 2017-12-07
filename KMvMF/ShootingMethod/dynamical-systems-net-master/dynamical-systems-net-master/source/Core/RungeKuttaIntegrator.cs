
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

using System;
using System.Collections.Generic;

namespace DynamicalSystemsNet.Core
{
    /// <summary>
    /// A 4-th order Runge-Kutta integrator.
    /// </summary>
    public class RungeKuttaIntegrator : IIntegrator
    {
        /// <summary>
        /// The iteration number most recently completed.
        /// </summary>
        public long Iteration { get; set; }

        /// <summary>
        /// The current value of time. This is the ongoing
        /// summation of every time step passed to the integrator.
        /// </summary>
        public double Time { get; set; }

        /// <summary>
        /// A list of nodes known by this integrator.
        /// </summary>
        public IEnumerable<DynamicalNode> Nodes
        {
            get
            {
                return _nodes;
            }
            set
            {
                // Clear existing list.
                _nodes.Clear();

                // Incoming nodes, possibly only a partial list.
                var nodes = value;
                
                // Prepare a helper method.
                Action<DynamicalNode> fnAddParentsAndSelf = null;
                fnAddParentsAndSelf = delegate(DynamicalNode n)
                {
                    if (this._nodes.Contains(n))
                    {
                        return;
                    }
                    this._nodes.Add(n);
                    
                    // NOTE: Uncomment this to actually add parents too.
                    //foreach (var incomingEdge in n.IncomingNodes)
                    //{
                    //    fnAddParentsAndSelf(incomingEdge.From);
                    //}
                };

                // Add all known nodes to solver.
                this._nodes = new List<DynamicalNode>();
                foreach (var node in nodes)
                {
                    fnAddParentsAndSelf(node);
                }
            }
        }

        /// <summary>
        /// Constructs a new Runge-Kutta integrator.
        /// </summary>
        public RungeKuttaIntegrator()
        {
            _nodes = new List<DynamicalNode>();
        }

        /// <summary>
        /// Execute one iteration of integration, moving
        /// the dynamical system forward in time by
        /// the given delta time.
        /// </summary>
        public void Step(double deltaTime)
        {
            double h = deltaTime;
            double halfH = deltaTime * 0.5;
            double sixthH = deltaTime / 6.0;
            double t00 = this.Time;
            double t05 = this.Time + halfH;
            double t10 = this.Time + h;

            VectorOI y0 = _nodes.ToVectorOI(n => n, n => n.Value);

            VectorOI k1 = _nodes.ToVectorOI(n => n, n => n.F(t00, y0));

            VectorOI k1Blend = k1.MultiplyNew(halfH).AddSelf(y0);
            VectorOI k2 = _nodes.ToVectorOI(n => n, n => n.F(t05, k1Blend));

            VectorOI k2Blend = k2.MultiplyNew(halfH).AddSelf(y0);
            VectorOI k3 = _nodes.ToVectorOI(n => n, n => n.F(t05, k2Blend));

            VectorOI k3Blend = k3.MultiplyNew(h).AddSelf(y0);
            VectorOI k4 = _nodes.ToVectorOI(n => n, n => n.F(t10, k3Blend));

            
            // Perform final runge-kutta weighted average.
            
            // We mutate k1, k2 and k3 here.
            k2.MultiplySelf(2.0);
            k3.MultiplySelf(2.0);
            VectorOI y1 = k1.AddSelf(k2).AddSelf(k3).AddSelf(k4).MultiplySelf(sixthH).AddSelf(y0);
            
            
            // Store results back into each node.
            foreach (var kvp in y1)
            {
                DynamicalNode node = kvp.Key as DynamicalNode;
                node.Value = kvp.Value;
            }

            this.Time = t10;
            this.Iteration++;
        }

        /// <summary>
        /// The internal list of known dynamical nodes.
        /// </summary>
        private List<DynamicalNode> _nodes;
    }
}
