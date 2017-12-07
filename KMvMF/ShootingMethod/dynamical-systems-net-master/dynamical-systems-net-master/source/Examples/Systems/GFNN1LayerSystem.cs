
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
using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicalSystemsNet.Examples.Systems
{
    /// <summary>
    /// A gradient-frequency neural network.
    /// 1-layer, with no internal connectivity.
    /// Based on "A dynamical systems approach to musical tonality" (2010).
    /// </summary>
    public class GFNN1LayerSystem : DynamicalSystem
    {
        public ToneNode SoundSource { get; protected set; }

        public GFNN1LayerSystem(IIntegrator integrator, double middleFrequency = 440.0, int octaves = 4, int nodesPerOctave = 120, ToneNode soundSource = null)
            : base(integrator)
        {
            // "A dynamical systems approach to musical tonality.". Large 2010. Section 4 "Predicting Tonality".

            // Generate list of natural frequencies. We step 
            // linearly through the log-frequency space.

            int numOctaves = octaves;
            int numFrequenciesPerOctave = nodesPerOctave;
            SortedSet<double> frequencies = new SortedSet<double>();

            double xMiddle = Math.Log(middleFrequency);
            double numOctavesHalfPow2 = Math.Pow(2.0, numOctaves / 2.0);

            double xLowerBound = Math.Log(middleFrequency / numOctavesHalfPow2);
            double xUpperBound = Math.Log(middleFrequency * numOctavesHalfPow2);

            double xStepSize = (xUpperBound - xLowerBound) / (numOctaves * numFrequenciesPerOctave);


            // Some variables.
            double x;

            // Some methods
            Func<double, double> logFrequencyToFrequency = (lf) => Math.Exp(lf);

            // Add midpoint.
            x = xMiddle;
            frequencies.Add(logFrequencyToFrequency(x));

            // Add lower frequencies.
            while (x >= xLowerBound)
            {
                x -= xStepSize;
                frequencies.Add(logFrequencyToFrequency(x));
            }

            // Add upper frequencies.
            x = xMiddle;
            while (x <= xUpperBound)
            {
                x += xStepSize;
                frequencies.Add(logFrequencyToFrequency(x));
            }

            // Create nodes.

            double alpha;
            double beta;
            double delta;
            double epsilon;
            double omega;

            // Configuration 1
            {
                // "A canonical model for gradient frequency neural networks.
                // Equation 20 test parameters.
                alpha = 0.0;
                beta = -10.0;
                delta = -9.0;
                epsilon = 0.3;
                omega = 2.0 * Math.PI;
            }

            List<DynamicalNode> nodes = new List<DynamicalNode>();
            foreach (double frequency in frequencies)
            {
                NeuralOscillatorNode node = new NeuralOscillatorNode(
                    0.0,
                    frequency,
                    epsilon,
                    alpha,
                    beta,
                    beta,
                    delta,
                    omega);

                node.Name = string.Format("ω{0:F3}", frequency);
                node.HistorySize = 2;
                nodes.Add(node);
            }

            // Add audio source node if present.
            if (soundSource != null)
            {
                foreach (var node in nodes)
                {
                    node.AddIncomingNode(soundSource, 1.0, 0.0, 0.0);
                }

                nodes.Add(soundSource);

                this.SoundSource = soundSource;
            }

            // Store nodes.
            this.Nodes = nodes;
        }

        public override IEnumerable<DynamicalNode> SelectOutputLayer()
        {
            return this.Nodes.OfType<NeuralOscillatorNode>();
        }

        public override string FormatNodeValue(DynamicalNode node)
        {
            if (node is NeuralOscillatorNode)
            {
                double averageMagnitude = node.AverageMagnitude;
                return averageMagnitude.ToString("F4");
            }
            else
            {
                return base.FormatNodeValue(node);
            }
        }

        public override IEnumerable<DynamicalNode> SelectNodesForToString()
        {
            var list = this.Nodes.Where(x => false);
            list = Enumerable.Repeat(this.SoundSource, this.SoundSource != null ? 1 : 0);
            list = list.Union(this.Nodes.OrderByDescending(n => n.AverageMagnitude).Take(5));
            return list;
        }
    }
}
