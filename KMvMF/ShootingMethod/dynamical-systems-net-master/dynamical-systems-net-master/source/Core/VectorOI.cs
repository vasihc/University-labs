
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
using System.Linq;
using System.Numerics;

namespace DynamicalSystemsNet.Core
{
    /// <summary>
    /// Helper methods for VectorOI.
    /// </summary>
    public static class VectorOIExt
    {
        public static VectorOI ToVectorOI<T>(this IEnumerable<T> source, Func<T, Object> keySelector, Func<T, Complex> valueSelector)
        {
            var dictionary = source.ToDictionary(keySelector, valueSelector);
            return new VectorOI(dictionary);
        }
    }

    /// <summary>
    /// An object-indexed vector. This is a vector
    /// type, composed of 0 or more complex-valued elements.
    /// Contrary to typical vector types, the VectorOI is
    /// indexed by object reference instead of by position #.
    /// </summary>
    public class VectorOI
    {
        public VectorOI()
        {
            _components = new Dictionary<object, Complex>();
        }

        public VectorOI(Dictionary<Object, Complex> sourceToAdopt)
        {
            _components = sourceToAdopt;
        }

        public VectorOI(IEnumerable<Tuple<Object, Complex>> values)
        {
            _components = values.ToDictionary(t => t.Item1, t => t.Item2);
        }

        /// <summary>
        /// Retrieves an element of the vector.
        /// </summary>
        public Complex this[Object index]
        {
            get
            {
                Complex value;
                if (_components.TryGetValue(index, out value))
                {
                    return value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
            set
            {
                _components[index] = value;
            }
        }

        /// <summary>
        /// Vector-vector addition.
        /// </summary>
        public static VectorOI operator +(VectorOI lhs, VectorOI rhs)
        {
            return lhs.AddNew(rhs);
        }

        /// <summary>
        /// Scalar-vector multiplication.
        /// </summary>
        public static VectorOI operator *(Complex lhs, VectorOI rhs)
        {
            return rhs.MultiplyNew(lhs);
        }

        /// <summary>
        /// The number of elements in this vector.
        /// </summary>
        public int Count
        {
            get
            {
                return _components.Count;
            }
        }

        /// <summary>
        /// Returns all values. Intended for debugging purposes only.
        /// </summary>
        public Complex[] Values
        {
            get
            {
                return _components.Select(kvp => kvp.Value).ToArray();
            }
        }

        public IEnumerator<KeyValuePair<Object, Complex>> GetEnumerator()
        {
            foreach (var kvp in this._components)
            {
                yield return kvp;
            }
        }

        public VectorOI AddSelf(VectorOI rhs)
        {
            VectorOI lhs = this;
            var keys = this._components.Keys.ToList();

            if (lhs.Count != rhs.Count)
            {
                throw new ArgumentException();
            }

            foreach (var key in keys)
            {
                var val1 = lhs[key];
                var val2 = rhs[key];
                var sum = val1 + val2;
                this[key] = sum;
            }

            return this;
        }

        public VectorOI AddNew(VectorOI rhs)
        {
            VectorOI lhs = this;
            var keys = this._components.Keys.ToList();

            if (lhs.Count != rhs.Count)
            {
                throw new ArgumentException();
            }

            VectorOI result = new VectorOI();
            foreach (var key in keys)
            {
                var val1 = lhs[key];
                var val2 = rhs[key];
                var sum = val1 + val2;
                result[key] = sum;
            }
            return result;
        }

        public VectorOI MultiplySelf(Complex factor)
        {
            var keys = this._components.Keys.ToList();

            foreach (var key in keys)
            {
                this[key] = this[key] * factor;
            }

            return this;
        }

        public VectorOI MultiplyNew(Complex rhs)
        {
            var keys = this._components.Keys.ToList();

            VectorOI result = new VectorOI();
            foreach (var key in keys)
            {
                Complex val1 = this[key];
                var product = val1 * rhs;
                result[key] = product;
            }
            return result;
        }


        Dictionary<Object, Complex> _components;
    }
}
