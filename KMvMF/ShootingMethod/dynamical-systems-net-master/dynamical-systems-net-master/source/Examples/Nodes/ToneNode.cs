
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
using System;
using System.Collections.Generic;
using System.Numerics;

namespace DynamicalSystemsNet.Examples.Nodes
{
    public class ToneNode : DynamicalNode
    {
        public const double MiddleC = 261.626;
        public const double A440 = 440.000;

        public class Note
        {
            /// <summary>
            /// The fundamental frequency.
            /// </summary>
            public double FundamentalFrequency { get; set; }

            public double Amplitude { get; set; }

            public ISet<short> Harmonics { get; protected set; }

            public Note(double fundamentalFrequency, double amplitude, params short[] harmonics)
            {
                this.FundamentalFrequency = fundamentalFrequency;
                this.Amplitude = amplitude;
                this.Harmonics = new SortedSet<short>();
                this.Harmonics.Add(1);
                if (harmonics != null)
                {
                    foreach (short h in harmonics)
                    {
                        this.Harmonics.Add(h);
                    }
                }
            }
        }

        public IList<Note> Notes
        {
            get;
            protected set;
        }

        public ToneNode()
            : base(0.0, 0.0)
        {
            Notes = new List<Note>();
        }

        public void ResetValueAndForcingFunction()
        {
            this.Value = 0;
            this._doResetForcingFunctionTime = true;
        }

        public override Complex F(double t, VectorOI y)
        {
            if (_doResetForcingFunctionTime)
            {
                _doResetForcingFunctionTime = false;
                _timeOffset = -t;
            }

            double timeForForcingFunction = t + _timeOffset;

            double derivative = 0.0;
            const double displacementDecay = 1.0;

            foreach (Note note in Notes)
            {
                foreach (short harmonicRatio in note.Harmonics)
                {
                    double amplitude = note.Amplitude / (double)harmonicRatio;
                    double frequency = note.FundamentalFrequency * (double)harmonicRatio;

                    double twoPiFrequency = 2.0 * Math.PI * frequency;
                    derivative += amplitude * twoPiFrequency * Math.Cos(twoPiFrequency * timeForForcingFunction);
                }
            }

            double normalizingTerm = Notes.Count;

            derivative = derivative / normalizingTerm - this.Value.Real * displacementDecay;
            return derivative;
        }

        private double _timeOffset;
        private bool _doResetForcingFunctionTime;
    }
}
