
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
using System.Diagnostics;
using System.Linq;

namespace DynamicalSystemsNet.Examples.Systems
{
    /// <summary>
    /// A gradient-frequency neural network.
    /// 2-layer, with no internal connectivity.
    /// Based on "A dynamical systems approach to musical tonality" (2010).
    /// </summary>
    public class GFNN2LayerSystem : DynamicalSystem
    {
        public IEnumerable<double> SIRs { get; protected set; }

        public bool EnableLearning { get; protected set; }


        public GFNN2LayerSystem(
            IIntegrator integrator, 
            ToneNode soundSource, 
            bool enableLearning, 
            double middleFrequency = ToneNode.MiddleC,
            int octaves = 2,
            int nodesPerOctave = 120)
            : base(integrator)
        {
            this.EnableLearning = enableLearning;

            // Initialize SIRs
            this.SIRs = new double[]
            {
                // Sub harmonics.
                1.0 * 1 / 1,
                1.0 * 1 / 2,
                //1.0 * 1 / 3,
                //1.0 * 2 / 3,
                //1.0 * 1 / 4,
                //1.0 * 3 / 4,
                //1.0 * 1 / 5,
                //1.0 * 2 / 5,
                //1.0 * 3 / 5,
                //1.0 * 4 / 5,

                // 12-tone ET.
                1.0 * 16 / 15,
                1.0 * 9 / 8,
                1.0 * 6 / 5,
                1.0 * 5 / 4,
                1.0 * 4 / 3,
                1.0 * 17 / 12,
                1.0 * 3 / 2,
                1.0 * 8 / 5,
                1.0 * 5 / 3,
                1.0 * 16 / 9,
                1.0 * 15 / 8,
                1.0 * 2 / 1,
            };


            _soundSource = soundSource;
            _layers = new List<List<NeuralOscillatorNode>>();

            var layer1 = GenerateLayer(
                "COC",
                middleFrequency,
                octaves,
                nodesPerOctave,
                -0.01,
                // omega=2 pi
                -1,
                -1,
                0,
                0.1,
                2.0 * Math.PI);

            var layer2 = GenerateLayer(
                "DCN",
                middleFrequency,
                octaves,
                nodesPerOctave,
                -0.4,
                1.2,
                -1,
                -0.01,
                0.75, 
                0);


            // Connect sound source to layer 1.
            foreach (var node in layer1)
            {
                node.AddIncomingNode(soundSource);
            }

            // Afferent layer 1 to layer 2.
            // Connect each node in layer 1 to its corresponding node in layer 2.
            double afferentWeightPlasticity = 0;
            double afferentWeightDecay = 0;

            foreach (var pair in layer1.Zip(layer2, (n1, n2) => new { L1N = n1, L2N = n2 }))
            {
                Debug.Assert(pair.L1N.CenterFrequency == pair.L2N.CenterFrequency);

                pair.L2N.AddIncomingNode(pair.L1N, 1.0, afferentWeightDecay, afferentWeightPlasticity);
            }

            // Internal layer 2 to layer 2.
            double internalWeight = 0.005;
            double sirMatchThreshold = 0.005;

            double internalWeightDecay = 1.0;
            double internalWeightPlasticity = 1.0;

            int connectionsAdded = 0;
            int iteration = -1;

            var outerJoin = layer2
                .Join(layer2, n => true, n => true, (n1, n2) => new { N1 = n1, N2 = n2 })
                .OrderBy(n => n.N1.CenterFrequency)
                .ThenBy(n => n.N2.CenterFrequency);
            foreach (var pair in outerJoin)
            {
                iteration++;

                if (pair.N1 == pair.N2)
                {
                    continue;
                }

                // Learn weights.
                if (this.EnableLearning)
                {
                    //if (iteration % 2 != 0)
                    //{
                    //    continue;
                    //}

                    pair.N1.AddIncomingNode(pair.N2, internalWeight, internalWeightDecay, internalWeightPlasticity);
                    pair.N2.AddIncomingNode(pair.N1, internalWeight, internalWeightDecay, internalWeightPlasticity);

                    connectionsAdded += 2;
                    
                }
                // Hardcoded weights.
                else
                {
                    // Connect layer 2 nodes to each other when they 
                    // match a (probably) learned SIR/eigenmultiple frequency.
                    double frequencyRatio = pair.N1.CenterFrequency / pair.N2.CenterFrequency;
                    double frequencyRatioAlt = 1.0 / frequencyRatio;

                    foreach (double sir in this.SIRs)
                    {
                        bool isMatch = MathHelpers.ApproximatelyEqual(frequencyRatio, sir, sirMatchThreshold)
                            || MathHelpers.ApproximatelyEqual(frequencyRatioAlt, sir, sirMatchThreshold);

                        if (!isMatch)
                        {
                            continue;
                        }

                        pair.N1.AddIncomingNode(pair.N2, internalWeight, 0, 0);
                        pair.N2.AddIncomingNode(pair.N1, internalWeight, 0, 0);

                        connectionsAdded += 2;

                        goto NextPair;
                    }
                }

            NextPair:
                continue;
            }


            // Assign layers to system.
            var allNodes = new DynamicalNode[] { soundSource }.Concat(layer1).Concat(layer2);
            this.Nodes = allNodes;

            // Remember layers.
            _layers.Add(layer1);
            _layers.Add(layer2);

            // Create hebbian learning system.
            if (this.EnableLearning)
            {
                _hebbianSystem = new ConnectionWeightSystem(new RungeKuttaIntegrator(), this);
            }
        }

        public override void Step(double deltaTime)
        {
            base.Step(deltaTime);

            if (this.EnableLearning && _hebbianSystem != null)
            {
                _hebbianSystem.Step(deltaTime);
            }
        }

        protected List<NeuralOscillatorNode> GenerateLayer(
            string namePrefix,
            double middleFrequency,
            int numOctaves,
            int numFrequenciesPerOctave,
            double alpha,
            double beta1,
            double beta2,
            double delta,
            double epsilon,
            double omega)
        {
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

            List<NeuralOscillatorNode> nodes = new List<NeuralOscillatorNode>();
            foreach (double frequency in frequencies)
            {
                NeuralOscillatorNode node = new NeuralOscillatorNode(
                    0.0,
                    frequency,
                    epsilon,
                    alpha,
                    beta1,
                    beta2,
                    delta,
                    omega);

                node.Name = string.Format("{1}:ω{0:F3}", frequency, namePrefix);
                node.HistorySize = 2;
                nodes.Add(node);
            }

            return nodes;
        }

        public override IEnumerable<DynamicalNode> SelectOutputLayer()
        {
            return _layers.Last();
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
            list = Enumerable.Repeat(this._soundSource, this._soundSource != null ? 1 : 0);
            list = list.Union(this.Nodes.OrderByDescending(n => n.AverageMagnitude).Take(5));
            return list;
        }


        ToneNode _soundSource;
        List<List<NeuralOscillatorNode>> _layers;
        ConnectionWeightSystem _hebbianSystem;
    }
}
