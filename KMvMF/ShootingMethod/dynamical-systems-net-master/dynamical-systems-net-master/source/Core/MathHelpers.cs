
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

namespace DynamicalSystemsNet.Core
{
    public static class MathHelpers
    {
        public static bool ApproximatelyEqual(double v1, double v2, double threshold = 0.00001, bool relative = false)
        {
            if (relative)
            {
                throw new NotImplementedException();
            }
            else
            {
                double delta = Math.Abs(v1 - v2);
                return delta <= threshold;
            }
        }
    }
}
