﻿//
// KwCombinatorics v3.0.x/v4.0.x
// Copyright © 2009-2012 Kasey Osborn (Kasewick@gmail.com)
// Ms-PL - Use and redistribute freely
//
// File: Multicombination.cs
//

using System;
using System.Collections.Generic;
using System.Text;

namespace Kw.Combinatorics
{
    /// <summary>
    /// Represents an ascending sequence of repeatable picks from a supplied number of choices.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <em>k</em>-multicombinations are also known as pick-multicombinations or
    /// <em>k</em>-combinations with repetitions.
    /// 
    /// The defining variables are <em>n</em> which is the number of possible choices and
    /// <em>k</em> which is the number of repeatable picks from those choices.
    /// 
    /// This is contrasted to <em>k</em>-combinations where the picks may not repeat.
    /// </para>
    /// <para>
    /// The <see cref="Multicombination"/> class produces <em>k</em>-multicombinations with
    /// ascending elements that may repeat as many as <em>k</em> times. 
    /// 
    /// While sequence order of the elements is not a requirement of <em>k</em>-multicombinations,
    /// producing an ascending sequence allows ranking the rows into an ordered table.
    /// </para>
    /// <para>
    /// Use the <see cref="Picks"/> property to get the number of elements (<em>k</em>)
    /// of a <see cref="Multicombination"/> taken from a possible number of
    /// <see cref="Choices"/> (<em>n</em>).
    /// 
    /// Use the <see cref="RowCount"/> property to get the number of distinct possible
    /// <see cref="Multicombination"/> sequences.
    /// 
    /// Use the <see cref="P:Kw.Combinatorics.Multicombination.Item(System.Int32)">indexer</see>
    /// to get a specified element of the sequence.
    ///
    /// Use the <see cref="GetEnumerator">default enumerator</see> to iterate thru
    /// the elements of a <see cref="Multicombination"/>.
    /// 
    /// Use the <see cref="Permute">Permute</see> method to pick objects from a supplied array
    /// of choices based on the current sequence.
    /// </para>
    /// <para>
    /// Use the <see cref="Rank"/> property to get or set the row index in a lexicographically
    /// ordered <see cref="Multicombination"/> table of all possible non-descending sequences.
    /// 
    /// Use <see cref="GetRows"/> to iterate thru all possible sequences of the
    /// <see cref="Multicombination"/> ordered by <see cref="Rank"/>.
    /// 
    /// Use <see cref="GetRowsForPicks">GetRowsForPicks (startPick, stopPick)</see> to iterate
    /// thru every table of all picks in the range (<em>startPick</em>..<em>stopPick</em>).
    /// </para>
    /// <para>
    /// The default appearance of a <see cref="Multicombination"/> row is a list of
    /// integers (starting at 0) enclosed in braces. The appearance may be tailored 3 ways:
    /// <ul>
    ///   <li>
    ///     Map each element to a different value using the
    ///     <see cref="GetEnumerator">default enumerator</see> or the
    ///     <see cref="P:Kw.Combinatorics.Multicombination.Item(System.Int32)">indexer</see>.
    ///   </li>
    ///   <li>
    ///     Use the <see cref="Permute">Permute</see> method and output the rearranged values.
    ///   </li>
    ///   <li>
    ///     Define a subclass of <see cref="Multicombination"/> and override
    ///     <see cref="System.Object.ToString">ToString()</see>.
    ///     (See the <see cref="GetRowsForPicks">GetRowsForPicks</see> method
    ///     for an example.)
    ///   </li>
    /// </ul>
    /// </para>
    /// <para>
    /// For more information about <em>k</em>-multicombinations, see:
    /// </para>
    /// <para>
    /// <em>http://en.wikipedia.org/wiki/Combination</em>
    /// </para>
    /// </remarks>
    /// <example>
    /// <para>
    /// Iterating thru <c>new Multicombination (4, 3).GetRows()</c> produces:
    /// </para>
    /// <para>
    /// <c>{ 0, 0, 0 }</c><br/>
    /// <c>{ 0, 0, 1 }</c><br/>
    /// <c>{ 0, 0, 2 }</c><br/>
    /// <c>{ 0, 0, 3 }</c><br/>
    /// <c>{ 0, 1, 1 }</c><br/>
    /// <c>{ 0, 1, 2 }</c><br/>
    /// <c>{ 0, 1, 3 }</c><br/>
    /// <c>{ 0, 2, 2 }</c><br/>
    /// <c>{ 0, 2, 3 }</c><br/>
    /// <c>{ 0, 3, 3 }</c><br/>
    /// <c>{ 1, 1, 1 }</c><br/>
    /// <c>{ 1, 1, 2 }</c><br/>
    /// <c>{ 1, 1, 3 }</c><br/>
    /// <c>{ 1, 2, 2 }</c><br/>
    /// <c>{ 1, 2, 3 }</c><br/>
    /// <c>{ 1, 3, 3 }</c><br/>
    /// <c>{ 2, 2, 2 }</c><br/>
    /// <c>{ 2, 2, 3 }</c><br/>
    /// <c>{ 2, 3, 3 }</c><br/>
    /// <c>{ 3, 3, 3 }</c>
    /// </para>
    /// </example>
    public class Multicombination :
        IComparable,
        System.Collections.IEnumerable,
        IComparable<Multicombination>,
        IEquatable<Multicombination>,
        IEnumerable<int>
    {
        private int[] data;     // The picks for the current rank. Length is 'k'.
        private int choices;    // Number of possible values 'n'.
        private long rowCount;  // Row count of the table of k-multicombinations.
        private long rank;      // Row index.

        #region Constructors

        /// <summary>
        /// Make an empty <see cref="Multicombination"/>.
        /// </summary>
        public Multicombination ()
        {
            this.data = new int[0];
            this.choices = 0;
            this.rowCount = 0;
            this.rank = 0;
        }

        
        /// <summary>
        /// Make a copy of a <see cref="Multicombination"/>.
        /// </summary>
        /// <param name="source">Instance to copy.</param>
        /// <exception cref="ArgumentNullException">When <em>source</em> is <b>null</b>.</exception>
        public Multicombination (Multicombination source)
        {
            if (source == null)
                throw new ArgumentNullException ("source");

            this.data = new int[source.data.Length];
            source.data.CopyTo (this.data, 0);

            this.choices = source.Choices;
            this.rowCount = source.RowCount;
            this.rank = source.rank;
        }


        /// <summary>
        /// Make a new <see cref="Multicombination"/> from the supplied
        /// <em>choices</em> of the same <em>Picks</em>.
        /// </summary>
        /// <param name="choices">Number of elements in the sequence.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// When <em>choices</em> less than 0.
        /// </exception>
        public Multicombination (int choices)
        {
            if (choices < 0)
                throw new ArgumentOutOfRangeException ("choices", "Value is less than zero.");

            this.data = new int[choices];
            this.choices = choices;
            this.rowCount = choices == 0? 0 : Combinatoric.BinomialCoefficient (Picks + choices - 1, Picks);
            this.rank = 0;
        }


        /// <summary>
        /// Make a new <see cref="Multicombination"/> from the supplied
        /// <em>choices</em> and <em>picks</em> of <see cref="Rank"/> 0.
        /// </summary>
        /// <param name="choices">Number of values to pick from.</param>
        /// <param name="picks">Number of elements in the sequence.</param>
        /// <example>
        /// <code source="Examples\Multicombination\McExample01\McExample01.cs" lang="cs" />
        /// </example>
        /// <exception cref="ArgumentOutOfRangeException">
        /// When negative value supplied; when <em>choices</em> is zero and <em>picks</em> is nonzero.
        /// </exception>
        /// <exception cref="OverflowException">When the numbers are just too big.</exception>
        public Multicombination (int choices, int picks)
        {
            if (choices < 0)
                throw new ArgumentOutOfRangeException ("choices", "Value is less than zero.");

            if (picks < 0)
                throw new ArgumentOutOfRangeException ("picks", "Value is less than zero.");

            if (choices == 0 && picks > 0)
                throw new ArgumentOutOfRangeException ("choices", "Value is zero and picks is nonzero.");

            this.data = new int[picks];
            this.choices = choices;
            this.rowCount = picks == 0? 0 : Combinatoric.BinomialCoefficient (picks + choices - 1, picks);
            this.rank = 0;
        }


        /// <summary>
        /// Make a new <see cref="Multicombination"/> from the supplied
        /// <em>choices</em> and <em>picks</em> of the supplied <em>rank</em>.
        /// </summary>
        /// <remarks>
        /// If the supplied <em>rank</em> is out of the range (0..<see cref="RowCount"/>-1),
        /// it will be normalized to the valid range. For example, a value of -1 will
        /// produce the last row in the ordered table.
        /// </remarks>
        /// <param name="choices">Number of values to pick from.</param>
        /// <param name="picks">Number of elements in the sequence.</param>
        /// <param name="rank">Initial row index in the ordered <see cref="Multicombination"/> table.</param>
        /// <example>
        /// <code source="Examples\Multicombination\McExample05\McExample05.cs" lang="cs" />
        /// </example>
        /// <exception cref="ArgumentOutOfRangeException">
        /// When negative value supplied; when <em>choices</em> is 0 and <em>picks</em> is not 0.
        /// </exception>
        /// <exception cref="OverflowException">When too many <em>choices</em>.</exception>
        public Multicombination (int choices, int picks, long rank)
        {
            if (choices < 0)
                throw new ArgumentOutOfRangeException ("choices", "Value is less than zero.");

            if (picks < 0)
                throw new ArgumentOutOfRangeException ("picks", "Value is less than zero.");

            if (choices == 0 && picks > 0)
                throw new ArgumentOutOfRangeException ("choices", "Value is zero and picks is nonzero.");

            this.data = new int[picks];
            this.choices = choices;
            this.rowCount = picks == 0? 0 : Combinatoric.BinomialCoefficient (picks + choices - 1, picks);
            Rank = rank;
        }


        /// <summary>
        /// Make a new <see cref="Multicombination"/> from the supplied elements.
        /// </summary>
        /// <param name="choices">Number of values to pick from.</param>
        /// <param name="source">Array of integers.</param>
        /// <example>
        /// <code source="Examples\Multicombination\McExample04\McExample04.cs" lang="cs" />
        /// </example>
        /// <exception cref="ArgumentNullException">When <em>source</em> is <b>null</b>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// When <em>source</em> contains invalid data;
        /// when <em>choices</em> is 0 and <em>source</em> is not empty.
        /// </exception>
        public Multicombination (int choices, int[] source)
        {
            if (source == null)
                throw new ArgumentNullException ("source");

            if (choices < 0)
                throw new ArgumentOutOfRangeException ("choices", "Value is less than zero.");

            if (choices == 0 && source.Length > 0)
                throw new ArgumentOutOfRangeException ("choices", "Value is zero and picks is nonzero.");

            this.data = new int[source.Length];
            source.CopyTo (this.data, 0);
            Array.Sort (this.data);

            this.choices = choices;
            this.rowCount = Picks == 0? 0 : Combinatoric.BinomialCoefficient (Picks + choices - 1, Picks);

            for (int ki = 0; ki < Picks; ++ki)
                if (this.data[ki] < 0 || this.data[ki] >= choices)
                    throw new ArgumentOutOfRangeException ("source", "Element is out of range.");

            //
            // Perform ranking:
            //

            this.rank = 0;
            if (RowCount == 0)
                return;

            int comboElement = this.data[0];
            int ji = 0;
            for (int ki = 0;;)
            {
                for (; ji < comboElement; ++ji)
                    rank += Combinatoric.BinomialCoefficient (Choices + Picks - ji - 2, Picks - ki - 1);

                ++ki;
                if (ki >= Picks)
                    break;

                ji = comboElement + 1;
                comboElement = this.data[ki] - this.data[ki-1] + ji;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// The available number of integers to choose from.
        /// </summary>
        /// <remarks>
        /// Also known as <em>n</em>.
        /// </remarks>
        public int Choices
        {
            get { return choices; }
        }


        /// <summary>
        /// Number of elements in the <see cref="Multicombination"/>.
        /// </summary>
        /// <remarks>
        /// Also known as <em>k</em>.
        /// </remarks>
        public int Picks
        {
            get { return data.Length; }
        }


        /// <summary>
        /// Row index in the ordered <see cref="Multicombination"/> table.
        /// </summary>
        /// <remarks>
        /// Any assigned value out of range will be normalized to (0..<see cref="RowCount"/>-1).
        /// </remarks>
        /// <example>
        /// <code source="Examples\Multicombination\McExample04\McExample04.cs" lang="cs" />
        /// </example>
        public long Rank
        {
            get { return rank; }
            set
            {
                if (RowCount == 0)
                    return;

                // Normalize the new rank.
                if (value < 0)
                {
                    rank = value % RowCount;
                    if (rank < 0)
                        rank += RowCount;
                }
                else
                    rank = value < RowCount? value : value % RowCount;

                //
                // Perform unranking:
                //

                long diminishingRank = RowCount - rank - 1;
                int combinaticAtom = Choices + Picks - 1;

                for (int ki = Picks; ki > 0; --ki)
                    for (;;)
                    {
                        --combinaticAtom;

                        long trialCount = Combinatoric.BinomialCoefficient (combinaticAtom, ki);
                        if (trialCount <= diminishingRank)
                        {
                            diminishingRank -= trialCount;
                            data[Picks - ki] = Choices - combinaticAtom + ki - 2;
                            break;
                        }
                    }
            }
        }


        /// <summary>
        /// Count of distinct sequences in the <see cref="Multicombination"/> table.
        /// </summary>
        public long RowCount
        {
            get { return rowCount; }
        }


        /// <summary>
        /// Get a element of the <see cref="Multicombination"/> at the supplied column.
        /// </summary>
        /// <param name="index">Zero-based index value.</param>
        /// <returns>Sequence value at <em>index</em>.</returns>
        /// <example>
        /// <code source="Examples\Multicombination\McExample05\McExample05.cs" lang="cs" />
        /// </example>
        /// <exception cref="IndexOutOfRangeException">
        /// When <em>index</em> not in range (0..<see cref="Picks"/>-1).
        /// </exception>
        public int this[int index]
        {
            get { return data[index]; }
        }

        #endregion

        #region Instance methods

        /// <summary>Compare two <see cref="Multicombination"/>s.</summary>
        /// <param name="obj">Target of the comparison.</param>
        /// <returns>
        /// A signed integer indicating the sort order of this instance to <em>obj</em>.
        /// </returns>
        public int CompareTo (object obj)
        { return CompareTo (obj as Multicombination); }


        /// <summary>Compare two <see cref="Multicombination"/>s.</summary>
        /// <param name="other">Target of the comparison.</param>
        /// <returns>
        /// A signed integer indicating the sort order of this instance to <em>other</em>.
        /// </returns>
        public int CompareTo (Multicombination other)
        {
            if ((object) other == null)
                return 1;

            int result = this.Picks - other.Picks;
            if (result == 0)
            {
                result = this.Choices - other.Choices;
                if (result == 0)
                {
                    long rankDiff = this.Rank - other.Rank;

                    if (rankDiff == 0)
                        result = 0;
                    else
                        result = rankDiff < 0 ? -1 : 1;
                }
            }

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

            if (array.Length < this.data.Length)
                throw new ArgumentException ("Destination array is not long enough.");

            this.data.CopyTo (array, 0);
        }


        /// <summary>
        /// Indicate whether two <see cref="Multicombination"/>s have the same value.
        /// </summary>
        /// <param name="obj">Target of the comparison.</param>
        /// <returns>
        /// <b>true</b> if <em>obj</em> has the same value as this object; otherwise, <b>false</b>.
        /// </returns>
        public override bool Equals (object obj)
        { return Equals (obj as Multicombination); }

        
        /// <summary>
        /// Indicate whether two <see cref="Multicombination"/>s have the same value.
        /// </summary>
        /// <param name="other">Target of the comparison.</param>
        /// <returns>
        /// <b>true</b> if <em>other</em> has the same value as this instance;
        /// otherwise, <b>false</b>.
        /// </returns>
        public bool Equals (Multicombination other)
        {
            return (object) other != null
                && other.Rank == Rank && other.Choices == Choices && other.Picks == Picks;
        }

        
        /// <summary>Get an object-based enumerator of the elements.</summary>
        /// <returns>Object-based elemental enumerator.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
        { return this.GetEnumerator(); }


        /// <summary>Enumerate all elements of a <see cref="Multicombination"/>.</summary>
        /// <returns>
        /// An <c>IEnumerator&lt;int&gt;</c> for this <see cref="Multicombination"/>.
        /// </returns>
        /// <example>
        /// <code source="Examples\Multicombination\McExample05\McExample05.cs" lang="cs" />
        /// </example>
        public IEnumerator<int> GetEnumerator ()
        {
            for (int ei = 0; ei < Picks; ++ei)
                yield return this.data[ei];
        }


        /// <summary>Get the hash oode of the <see cref="Multicombination"/>.</summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode ()
        { return unchecked ((int) Rank); }


        /// <summary>
        /// Iterate thru all rows of the <see cref="Multicombination"/> table
        /// for every value of <see cref="Rank"/> ascending.
        /// </summary>
        /// <returns>An iterator for a <see cref="Multicombination"/> table.</returns>
        /// <remarks>
        /// If the start row is not of <see cref="Rank"/> 0, the iteration will wrap around
        /// so that <see cref="RowCount"/> items are always produced.
        /// </remarks>
        /// <example>
        /// <code source="Examples\Multicombination\McExample01\McExample01.cs" lang="cs" />
        /// </example>
        public IEnumerable<Multicombination> GetRows ()
        {
            if (RowCount > 0)
            {
                long startRank = rank;
                for (Multicombination current = (Multicombination) MemberwiseClone();;)
                {
                    yield return current;
                    current.Rank = current.Rank + 1;
                    if (current.Rank == startRank)
                        break;
                }
            }
        }


        /// <summary>
        /// Iterate thru all rows of all <see cref="Multicombination"/> tables for every
        /// pick in the range (<em>startPicks</em>..<em>stopPicks</em>).
        /// </summary>
        /// <returns>An iterator for a series of <see cref="Multicombination"/> tables.</returns>
        /// <remarks>
        /// Unlike <see cref="Combination"/>, <see cref="Picks"/> may exceed <see cref="Choices"/>.
        /// </remarks>
        /// <param name="startPicks">Number of picks for first table.</param>
        /// <param name="stopPicks">Number of picks for last table.</param>
        /// <example>
        /// <code source="Examples\Multicombination\McExample02\McExample02.cs" lang="cs" />
        /// </example>
        /// <exception cref="ArgumentOutOfRangeException">
        /// When <em>startPicks</em> is less than 0 or greater than <em>stopPicks</em>.
        /// </exception>
        public IEnumerable<Multicombination> GetRowsForPicks (int startPicks, int stopPicks)
        {
            if (startPicks < 0 || startPicks > stopPicks)
                throw new ArgumentOutOfRangeException ("startPicks", "Pick range is not valid.");

            if (Choices == 0)
                yield break;

            if (startPicks == 0)
                startPicks = 1;

            for (int k = startPicks; k <= stopPicks; ++k)
            {
                Multicombination current = (Multicombination) MemberwiseClone();
                current.data = new int[k];
                current.rowCount = Combinatoric.BinomialCoefficient (k + choices - 1, k);
                current.rank = 0;

                for (;;)
                {
                    yield return current;
                    
                    current.Rank = current.Rank + 1;
                    if (current.Rank == 0)
                        break;
                }
            }
        }


        /// <summary>
        /// Provide a readable form of the <see cref="Multicombination"/> sequence.
        /// </summary>
        /// <returns>A <c>string</c> that represents the sequence.</returns>
        /// <remarks>Result is enclosed in braces and separated by commas.</remarks>
        /// <example>
        /// <code source="Examples\Multicombination\McExample04\McExample04.cs" lang="cs" />
        /// </example>
        public override string ToString ()
        {
            if (RowCount == 0)
                return ("{ }");

            StringBuilder result = new StringBuilder ("{ ");

            for (int ei = 0;;)
            {
                result.Append (this.data[ei]);

                ++ei;
                if (ei >= Picks)
                    break;

                result.Append (", ");
            }

            result.Append (" }");

            return result.ToString();
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Apply a <see cref="Multicombination"/> sequence to rearrange the supplied list or array.
        /// </summary>
        /// <typeparam name="T">Type of items to rearrange.</typeparam>
        /// <param name="arrangement">New arrangement for items.</param>
        /// <param name="source">List of items to rearrange.</param>
        /// <returns>List of rearranged items.</returns>
        /// <example>
        /// <code source="Examples\Multicombination\McExample03\McExample03.cs" lang="cs" />
        /// </example>
        /// <exception cref="ArgumentNullException">
        /// When <em>arrangement</em> or <em>source</em> is <b>null</b>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// When length of <em>source</em> is less than <see cref="Choices"/>.
        /// </exception>
        public static List<T> Permute<T> (Multicombination arrangement, IList<T> source)
        {
            if (arrangement == null)
                throw new ArgumentNullException ("arrangement");

            if (source == null)
                throw new ArgumentNullException ("source");

            if (source.Count < arrangement.Choices)
                throw new ArgumentException ("Not enough supplied values.", "source");

            List<T> result = new List<T> (arrangement.Picks);

            for (int ei = 0; ei < arrangement.Picks; ++ei)
                result.Add (source[arrangement[ei]]);

            return result;
        }

        
        /// <summary>Indicate whether 2 <see cref="Multicombination"/>s are equal.</summary>
        /// <param name="param1">A <see cref="Multicombination"/> sequence.</param>
        /// <param name="param2">A <see cref="Multicombination"/> sequence.</param>
        /// <returns><b>true</b> if supplied sequences are equal;
        /// otherwise, <b>false</b>.</returns>
        public static bool operator == (Multicombination param1, Multicombination param2)
        {
            if ((object) param1 == null)
                return (object) param2 == null;
            else
                return param1.Equals (param2);
        }


        /// <summary>Indicate whether 2 <see cref="Multicombination"/>s are not equal.</summary>
        /// <param name="param1">A <see cref="Multicombination"/> sequence.</param>
        /// <param name="param2">A <see cref="Multicombination"/> sequence.</param>
        /// <returns><b>true</b> if supplied sequences are not equal;
        /// otherwise, <b>false</b>.</returns>
        public static bool operator != (Multicombination param1, Multicombination param2)
        { return !(param1 == param2); }


        /// <summary>Indicate whether the left <see cref="Multicombination"/> is less than
        /// the right <see cref="Multicombination"/>.</summary>
        /// <param name="param1">A <see cref="Multicombination"/> sequence.</param>
        /// <param name="param2">A <see cref="Multicombination"/> sequence.</param>
        /// <returns><b>true</b> if the left sequence is less than
        /// the right sequence; otherwise, <b>false</b>.</returns>
        public static bool operator < (Multicombination param1, Multicombination param2)
        {
            if ((object) param1 == null)
                return (object) param2 != null;
            else
                return param1.CompareTo (param2) < 0;
        }


        /// <summary>Indicate whether the left <see cref="Multicombination"/> is greater than
        /// or equal to the right <see cref="Multicombination"/>.</summary>
        /// <param name="param1">A <see cref="Multicombination"/> sequence.</param>
        /// <param name="param2">A <see cref="Multicombination"/> sequence.</param>
        /// <returns><b>true</b> if the left sequence is greater than or equal to
        /// the right sequence; otherwise, <b>false</b>.</returns>
        public static bool operator >= (Multicombination param1, Multicombination param2)
        { return !(param1 < param2); }


        /// <summary>Indicate whether the left <see cref="Multicombination"/> is greater than
        /// the right <see cref="Multicombination"/>.</summary>
        /// <param name="param1">A <see cref="Multicombination"/> sequence.</param>
        /// <param name="param2">A <see cref="Multicombination"/> sequence.</param>
        /// <returns><b>true</b> if the left sequence is greater than
        /// the right sequence; otherwise, <b>false</b>.</returns>
        public static bool operator > (Multicombination param1, Multicombination param2)
        {
            if ((object) param1 == null)
                return false;
            else
                return param1.CompareTo (param2) > 0;
        }


        /// <summary>Indicate whether the left <see cref="Multicombination"/> is less than
        /// or equal to the right <see cref="Multicombination"/>.</summary>
        /// <param name="param1">A <see cref="Multicombination"/> sequence.</param>
        /// <param name="param2">A <see cref="Multicombination"/> sequence.</param>
        /// <returns><b>true</b> if the left sequence is less than or equal to
        /// the right sequence; otherwise, <b>false</b>.</returns>
        public static bool operator <= (Multicombination param1, Multicombination param2)
        { return !(param1 > param2); }

        #endregion
    }
}
