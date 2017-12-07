
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DynamicalSystemsNet.Examples.Systems
{
    public class ConnectionWeightSystem : DynamicalSystem
    {
        public DynamicalSystem SourceSystem { get; protected set; }


        public ConnectionWeightSystem(IIntegrator integrator, DynamicalSystem existingSystem)
            : base(integrator)
        {
            this.SourceSystem = existingSystem;

            // Generate connection weight nodes for all connections in the incoming system.
            var links = existingSystem
                .Nodes
                .SelectMany(n => n.IncomingNodes)
                .Where(c => c.From is NeuralOscillatorNode && c.To is NeuralOscillatorNode)
                .Where(c => c.Plasticity > 0 || c.Decay > 0);
            Debug.Assert(links.Count() == links.Distinct().Count());

            List<NeuralOscillatorConnectionNode> weightNodes = new List<NeuralOscillatorConnectionNode>();
            foreach (var link in links)
            {
                NeuralOscillatorConnectionNode weightNode = new NeuralOscillatorConnectionNode(0, link);
                weightNodes.Add(weightNode);
            }

            _weightNodes = weightNodes;
            this.Nodes = weightNodes;
        }

        public override void Step(double deltaTime)
        {
            base.Step(deltaTime);

            // Copy weights back.
            foreach (NeuralOscillatorConnectionNode node in _weightNodes)
            {
                node.Connection.Weight = node.Value;
            }
        }


        private List<NeuralOscillatorConnectionNode> _weightNodes;
    }
}
