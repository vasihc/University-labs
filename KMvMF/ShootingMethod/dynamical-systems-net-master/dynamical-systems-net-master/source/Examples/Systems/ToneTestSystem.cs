
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
    public class ToneTestSystem : DynamicalSystem
    {
        public ToneTestSystem(IIntegrator integrator, double rootFrequency)
            : base(integrator)
        {
            //
            // Prepare notes
            //
            ToneNode.Note note440 = new ToneNode.Note(
                rootFrequency, 
                1.0, 
                1, 3, 5, 8, 12);

            ToneNode.Note noteMajorThird = new ToneNode.Note(
                rootFrequency * 5.0 / 4.0, 
                1.0,
                1, 3, 5, 8, 12);

            //
            // Prepare source node.
            //
            ToneNode source = new ToneNode();
            source.Name = "audio_src";

            source.Notes.Add(note440);
            source.Notes.Add(noteMajorThird);


            //
            // Add nodes to system.
            //
            this.Nodes = new[] { source };
        }
    }
}
