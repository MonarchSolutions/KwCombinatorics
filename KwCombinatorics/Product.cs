﻿//
// KwCombinatorics v3.0.x/v4.0.x
// Copyright © 2009-2012 Kasey Osborn (Kasewick@gmail.com)
// Ms-PL - Use and redistribute freely
//
// File: Product.cs
//

using System;
using System.Collections.Generic;
using System.Text;

namespace Kw.Combinatorics
{
    /// <summary>
    /// Represents a join of values taken from a supplied array of ranges.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A cartesian product is a set of sets where each subset is constructed by picking
    /// 1 element from each of a given number of sets. This process of joining elements to
    /// form new sets is repeated until all possible distinct joins are made.
    /// </para>
    /// <para>
    /// The <see cref="Product"/> class uses an array of integers as input where each integer
    /// is the size of each of the composing sets. The joined sets are represented as rows in
    /// a table where each element is a value in the range of these supplied sizes. Rows are
    /// constructed by looping thru the rightmost ranges fastest so that the resulting table
    /// is lexicographically ordered.
    /// </para>
    /// <para>
    /// Use the <see cref="Width"/> property to get the number of joined elements.
    /// 
    /// Use the <see cref="RowCount"/> property to get the number of distinct joins
    /// in the <see cref="Product"/> table.
    ///
    /// Use the <see cref="P:Kw.Combinatorics.Product.Item(System.Int32)">indexer</see>
    /// to get an element of the row.
    /// 
    /// Use the <see cref="GetEnumerator">default enumerator</see> to iterate thru
    /// the elements of a <see cref="Product"/> row.
    ///
    /// Use the <see cref="Permute">Permute</see>
    /// method to rearrange a supplied list based on the values in a row.
    /// </para>
    /// <para>
    /// Use the <see cref="Rank"/> property to get or set the row index in the ordered
    /// <see cref="Product"/> table of joins.
    /// 
    /// Use <see cref="GetRows"/> to iterate thru all possible joins
    ///  of the<see cref="Product"/> ordered by <see cref="Rank"/>.
    /// </para>
    /// <para>
    /// The default appearance of a <see cref="Product"/> row is a list of integers
    /// (starting at 0) enclosed in braces. The appearance may be tailored 3 ways:
    /// <ul>
    ///   <li>
    ///     Map each element to a different value using the
    ///     <see cref="GetEnumerator">default enumerator</see> or the
    ///     <see cref="P:Kw.Combinatorics.Product.Item(System.Int32)">indexer</see>.
    ///   </li>
    ///   <li>
    ///     Use the <see cref="Permute">Permute</see> method and output the rearranged values.
    ///   </li>
    ///   <li>
    ///     Define a subclass of <see cref="Product"/> and override
    ///     <see cref="System.Object.ToString">ToString()</see>.
    ///   </li>
    /// </ul>
    /// </para>
    /// <para>
    /// For more information about cartesian products, see:
    /// </para>
    /// <para>
    /// <em>http://en.wikipedia.org/wiki/Cartesian_product</em>
    /// </para>
    /// </remarks>
    /// <example>
    /// <para>
    /// Iterating thru <c>new Product (new int[] { 2, 3, 2 }).GetRows()</c> produces:
    /// </para>
    /// <para>
    /// <c>{ 0, 0, 0 }</c><br/>
    /// <c>{ 0, 0, 1 }</c><br/>
    /// <c>{ 0, 1, 0 }</c><br/>
    /// <c>{ 0, 1, 1 }</c><br/>
    /// <c>{ 0, 2, 0 }</c><br/>
    /// <c>{ 0, 2, 1 }</c><br/>
    /// <c>{ 1, 0, 0 }</c><br/>
    /// <c>{ 1, 0, 1 }</c><br/>
    /// <c>{ 1, 1, 0 }</c><br/>
    /// <c>{ 1, 1, 1 }</c><br/>
    /// <c>{ 1, 2, 0 }</c><br/>
    /// <c>{ 1, 2, 1 }</c>
    /// </para>
    /// </example>
    public class Product :
        IComparable,
        System.Collections.IEnumerable,
        IComparable<Product>,
        IEquatable<Product>,
        IEnumerable<int>
    {
        private int[] sizes;    // Size of each column.
        private long[] factors; // Running multiple of sizes.
        private long rowCount;  // Row count of the table of products.
        private long rank;      // Row index.

        #region Constructors

        /// <summary>
        /// Make an empty <see cref="Product"/>.
        /// </summary>
        public Product ()
        {
            this.sizes = new int[0];
            this.factors = new long[0];
            this.rowCount = 0;
            this.rank = 0;
        }


        /// <summary>
        /// Make a copy of a <see cref="Product"/>.
        /// </summary>
        /// <param name="source">Instance to copy.</param>
        /// <exception cref="ArgumentNullException">When <em>source</em> is <b>null</b>.</exception>
        public Product (Product source)
        {
            if (source == null)
                throw new ArgumentNullException ("source");

            this.sizes = new int[source.sizes.Length];
            this.factors = new long[source.factors.Length];

            source.sizes.CopyTo (this.sizes, 0);
            source.factors.CopyTo (this.factors, 0);
            this.rowCount = source.rowCount;
            this.rank = source.rank;
        }


        /// <summary>
        /// Make a new <see cref="Product"/> from the supplied
        /// column <em>sizes</em> of <see cref="Rank"/> 0.
        /// </summary>
        /// <param name="sizes">Size of each column.</param>
        /// <example>
        /// <code source="Examples\Product\PtExample01\PtExample01.cs" lang="cs" />
        /// </example>
        /// <exception cref="ArgumentNullException">
        /// When <em>sizes</em> is <b>null</b>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// When any column size less than 0.
        /// </exception>
        /// <exception cref="OverflowException">
        /// When product is too big.
        /// </exception>
        public Product (int[] sizes)
        {
            if (sizes == null)
                throw new ArgumentNullException ("sizes");

            this.sizes = new int[sizes.Length];
            sizes.CopyTo (this.sizes, 0);

            this.factors = new long[this.sizes.Length];
            this.rowCount = sizes.Length == 0 ? 0 : 1;

            for (int ei = this.sizes.Length - 1; ei >= 0; --ei)
            {
                if (this.sizes[ei] < 0)
                    throw new ArgumentOutOfRangeException ("sizes", "Value is less than zero.");

                this.factors[ei] = this.rowCount;
                this.rowCount = checked (this.rowCount * this.sizes[ei]);
            }
        }


        /// <summary>
        /// Make a new <see cref="Product"/> from the supplied column <em>sizes</em> of the
        /// supplied <em>rank</em>.
        /// </summary>
        /// <remarks>
        /// If the supplied <em>rank</em> is out of the range (0..<see cref="RowCount"/>-1),
        /// it will be normalized to the valid range. For example, a value of -1 will
        /// produce the last row in the ordered table.
        /// </remarks>
        /// <param name="sizes">Size of each column.</param>
        /// <param name="rank">Initial row index.</param>
        /// <exception cref="ArgumentNullException"
        /// >When <em>sizes</em> is <b>null</b>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException"
        /// >When any column size less than 0.
        /// </exception>
        public Product (int[] sizes, long rank)
            : this (sizes)
        {
            Rank = rank;
        }


        /// <summary>
        /// Make a new <see cref="Product"/> of the supplied column <em>sizes</em>
        /// from the supplied values.
        /// </summary>
        /// <param name="sizes">Size of each column.</param>
        /// <param name="source">Integer values for the columns.</param>
        /// <example>
        /// <code source="Examples\Product\PtExample04\PtExample04.cs" lang="cs" />
        /// </example>
        /// <exception cref="ArgumentNullException">
        /// When <em>sizes</em> or <em>source</em> is <b>null</b>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// When <em>source</em> length does not match <em>sizes</em> length.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// When any column size less than 0. When <em>source</em> data is not valid.
        /// </exception>
        public Product (int[] sizes, int[] source)
            : this (sizes)
        {
            if (source == null)
                throw new ArgumentNullException ("source");

            if (sizes.Length != source.Length)
                throw new ArgumentException ("Length is not valid.", "source");

            for (int si = 0; si < source.Length; ++si)
            {
                if (source[si] < 0 || source[si] >= sizes[si])
                    throw new ArgumentOutOfRangeException ("source", "Element is out of range.");

                this.rank = this.rank * sizes[si] + source[si];
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Row index of the join in the lexicographically ordered <see cref="Product"/> table.
        /// </summary>
        /// <remarks>
        /// Any assigned value out of range will be normalized to (0..<see cref="RowCount"/>-1).
        /// </remarks>
        /// <example>
        /// <code source="Examples\Product\PtExample04\PtExample04.cs" lang="cs" />
        /// </example>
        public long Rank
        {
            get
            {
                return rank;
            }
            set
            {
                if (RowCount == 0)
                    rank = 0;
                else
                {
                    // Normalize the new rank.
                    if (value < 0)
                    {
                        rank = value % RowCount;
                        if (rank < 0)
                            rank += RowCount;
                    }
                    else
                        rank = value < RowCount? value : value % RowCount;
                }
            }
        }


        /// <summary>
        /// Count of distinct joins in the <see cref="Product"/> table.
        /// </summary>
        public long RowCount
        {
            get { return rowCount; }
        }


        /// <summary>
        /// Number of columns in the <see cref="Product"/>.
        /// </summary>
        public int Width
        {
            get { return sizes.Length; }
        }


        /// <summary>
        /// Get an element of the <see cref="Product"/> at the supplied column.
        /// </summary>
        /// <param name="index">Index value.</param>
        /// <returns>Sequence value at <em>index</em>.</returns>
        /// <example>
        /// <code source="Examples\Product\PtExample05\PtExample05.cs" lang="cs" />
        /// </example>
        /// <exception cref="IndexOutOfRangeException">
        /// When <em>index</em> not in range (0..<see cref="Width"/>-1).
        /// </exception>
        /// <exception cref="DivideByZeroException">
        /// When <see cref="RowCount"/> is 0.
        /// </exception>
        public int this[int index]
        {
            get
            {
                long rankToElement = this.rank;
                if (index > 0)
                    rankToElement %= factors[index - 1];

                return (int) (rankToElement / factors[index]);
            }
        }

        #endregion

        #region Instance methods

        /// <summary>Compare 2 <see cref="Product"/>s.</summary>
        /// <param name="obj">Target of the comparison.</param>
        /// <returns>
        /// A signed integer indicating the sort order of this instance to <em>obj</em>.
        /// </returns>
        public int CompareTo (object obj)
        { return CompareTo (obj as Product); }


        /// <summary>Compare 2 <see cref="Product"/>s.</summary>
        /// <param name="other">Target of the comparison.</param>
        /// <returns>
        /// A signed integer indicating the sort order of this instance to <em>other</em>.
        /// </returns>
        public int CompareTo (Product other)
        {
            if ((object) other == null)
                return 1;

            int result = this.Width - other.Width;

            if (result == 0)
                if (this.Rank > other.Rank)
                    result = 1;
                else if (this.Rank < other.Rank)
                    result = -1;

            return result;
        }


        /// <summary>
        /// Copy the entire sequence to the supplied destination.
        /// </summary>
        /// <param name="array">Destination of copy.</param>
        /// <exception cref="ArgumentNullException">When <em>array</em> is <b>null</b>.</exception>
        /// <exception cref="ArgumentException">When not enough space in <em>array</em>.</exception>
        public void CopyTo (int[] array)
        {
            if (array == null)
                throw new ArgumentNullException ("array");

            if (array.Length < Width)
                throw new ArgumentException ("Destination array is not long enough.");

            for (int ei = 0; ei < Width; ++ei)
                array[ei] = this[ei];
        }


        /// <summary>
        /// Indicate whether 2 <see cref="Product"/>s have the same value.
        /// </summary>
        /// <param name="obj">Target of the comparison.</param>
        /// <returns>
        /// <b>true</b> if <em>obj</em> has the same value as this object; otherwise, <b>false</b>.
        /// </returns>
        public override bool Equals (object obj)
        { return Equals (obj as Product); }


        /// <summary>
        /// Indicate whether 2 <see cref="Product"/>s have the same value.
        /// </summary>
        /// <param name="other">Target of the comparison.</param>
        /// <returns>
        /// <b>true</b> if <em>other</em> has the same value as this object;
        /// otherwise, <b>false</b>.
        /// </returns>
        public bool Equals (Product other)
        { return (object) other != null && other.Rank == Rank && other.Width == Width; }


        /// <summary>Get an object-based enumerator of the elements.</summary>
        /// <returns>Object-based elemental enumerator.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
        { return GetEnumerator(); }


        /// <summary>Enumerate all elements of a <see cref="Product"/>.</summary>
        /// <returns>
        /// An <c>IEnumerator&lt;int&gt;</c> for this <see cref="Product"/>.
        /// </returns>
        /// <example>
        /// <code source="Examples\Product\PtExample05\PtExample05.cs" lang="cs" />
        /// </example>
        public IEnumerator<int> GetEnumerator ()
        {
            for (int ei = 0; ei < Width; ++ei)
                yield return this[ei];
        }


        /// <summary>Get the hash oode of the <see cref="Product"/>.</summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode ()
        { return unchecked ((int) rank); }


        /// <summary>
        /// Iterate thru all rows of the <see cref="Product"/> table for 
        /// for every value of <see cref="Rank"/> ascending.
        /// </summary>
        /// <returns>An iterator for a <see cref="Product"/> table.</returns>
        /// <remarks>
        /// If the start row is not of <see cref="Rank"/> 0, the iteration will wrap around
        /// so that <see cref="RowCount"/> items are always produced.
        /// </remarks>
        /// <example>
        /// <code source="Examples\Product\PtExample01\PtExample01.cs" lang="cs" />
        /// </example>
        public IEnumerable<Product> GetRows ()
        {
            if (RowCount > 0)
            {
                long startRank = Rank;
                for (Product current = (Product) MemberwiseClone();;)
                {
                    yield return current;
                    current.Rank = current.Rank + 1;
                    if (current.Rank == startRank)
                        break;
                }
            }
        }


        /// <summary>
        /// Get the size of a column.
        /// </summary>
        /// <param name="column">Column index.</param>
        /// <returns>Number of distinct values (starting at 0) that a column may take.</returns>
        /// <exception cref="IndexOutOfRangeException">
        /// When <em>column</em> not in range (0..<see cref="Width"/>-1).
        /// </exception>
        public int Size (int column)
        { return this.sizes[column]; }


        /// <summary>
        /// Provide readable form of the <see cref="Product"/> row.
        /// </summary>
        /// <returns>A <c>string</c> that represents the sequence.</returns>
        /// <remarks>Result is enclosed in braces and separated by commas.</remarks>
        /// <example>
        /// <code source="Examples\Product\PtExample04\PtExample04.cs" lang="cs" />
        /// </example>
        public override string ToString ()
        {
            if (RowCount == 0)
                return ("{ }");

            StringBuilder result = new StringBuilder ("{ ");

            for (int ei = 0; ; )
            {
                result.Append (this[ei]);

                ++ei;
                if (ei >= Width)
                    break;

                result.Append (", ");
            }

            result.Append (" }");

            return result.ToString();
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Apply a <see cref="Product"/> sequence to select from the supplied lists or arrays.
        /// </summary>
        /// <typeparam name="T">Type of items to rearrange.</typeparam>
        /// <param name="arrangement">New arrangement for items.</param>
        /// <param name="source">List of List of Items or arrays to rarrange.</param>
        /// <returns>List of rearranged items.</returns>
        /// <example>
        /// <code source="Examples\Product\PtExample03\PtExample03.cs" lang="cs" />
        /// </example>
        /// <exception cref="ArgumentNullException">When <em>arrangement</em> or <em>source</em> is <b>null</b>.</exception>
        /// <exception cref="ArgumentException">When not enough source sets.</exception>
        /// <exception cref="IndexOutOfRangeException">
        /// When supplied source list is too small.
        /// </exception>
        public static List<T> Permute<T> (Product arrangement, IList<IList<T>> source)
        {
            if (arrangement == null)
                throw new ArgumentNullException ("arrangement");

            if (source == null)
                throw new ArgumentNullException ("source");

            if (source.Count < arrangement.Width)
                throw new ArgumentException ("Not enough supplied values.", "source");

            List<T> result = new List<T> (arrangement.Width);

            for (int ai = 0; ai < arrangement.Width; ++ai)
                result.Add (source[ai][arrangement[ai]]);

            return result;
        }


        /// <summary>Indicate whether 2 <see cref="Product"/>s are equal.</summary>
        /// <param name="param1">A <see cref="Product"/> sequence.</param>
        /// <param name="param2">A <see cref="Product"/> sequence.</param>
        /// <returns><b>true</b> if supplied sequences are equal;
        /// otherwise, <b>false</b>.</returns>
        public static bool operator == (Product param1, Product param2)
        {
            if ((object) param1 == null)
                return (object) param2 == null;
            else
                return param1.Equals (param2);
        }


        /// <summary>Indicate whether 2 <see cref="Product"/>s are not equal.</summary>
        /// <param name="param1">A <see cref="Product"/> sequence.</param>
        /// <param name="param2">A <see cref="Product"/> sequence.</param>
        /// <returns><b>true</b> if supplied sequences are not equal;
        /// otherwise, <b>false</b>.</returns>
        public static bool operator != (Product param1, Product param2)
        { return !(param1 == param2); }


        /// <summary>Indicate whether the left <see cref="Product"/> is less than
        /// the right <see cref="Product"/>.</summary>
        /// <param name="param1">A <see cref="Product"/> sequence.</param>
        /// <param name="param2">A <see cref="Product"/> sequence.</param>
        /// <returns><b>true</b> if the left sequence is less than
        /// the right sequence otherwise, <b>false</b>.</returns>
        public static bool operator < (Product param1, Product param2)
        {
            if ((object) param1 == null)
                return (object) param2 != null;
            else
                return param1.CompareTo (param2) < 0;
        }


        /// <summary>Indicate whether the left <see cref="Product"/> is greater than
        /// or equal to the right <see cref="Product"/>.</summary>
        /// <param name="param1">A <see cref="Product"/> sequence.</param>
        /// <param name="param2">A <see cref="Product"/> sequence.</param>
        /// <returns><b>true</b> if the left sequence is greater than or
        /// equal to the right sequence otherwise, <b>false</b>.</returns>
        public static bool operator >= (Product param1, Product param2)
        { return !(param1 < param2); }


        /// <summary>Indicate whether the left <see cref="Product"/> is greater than
        /// the right <see cref="Product"/>.</summary>
        /// <param name="param1">A <see cref="Product"/> sequence.</param>
        /// <param name="param2">A <see cref="Product"/> sequence.</param>
        /// <returns><b>true</b> if the left sequence is greater than
        /// the right sequence otherwise, <b>false</b>.</returns>
        public static bool operator > (Product param1, Product param2)
        {
            if ((object) param1 == null)
                return false;
            else
                return param1.CompareTo (param2) > 0;
        }


        /// <summary>Indicate whether the left <see cref="Product"/> is less than or equal to
        /// the right <see cref="Product"/>.</summary>
        /// <param name="param1">A <see cref="Product"/> sequence.</param>
        /// <param name="param2">A <see cref="Product"/> sequence.</param>
        /// <returns><b>true</b> if the left sequence is less than or equal to
        /// the right sequence otherwise, <b>false</b>.</returns>
        public static bool operator <= (Product param1, Product param2)
        { return !(param1 > param2); }

        #endregion
    }
}
