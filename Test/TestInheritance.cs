using Kw.Combinatorics;
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kw.CombinatoricsTest
{
    public class ComboSub : Combination
    {
        static string[] chars = new string[] { "A", "B", "C", "D" };

        public ComboSub (int picks) : base (chars.Length, picks)
        { }

        public ComboSub () : base (chars.Length)
        { }

        public override string ToString ()
        {
            string result = "";
            foreach (int element in this)
                result += chars[element];
            return result;
        }
    }


    [TestClass]
    public partial class TestInheritance
    {
        static string[] expectedCn1 = new string[] { "ABC", "ABD", "ACD", "BCD" };

        static string[] expectedCn2 = new string[]
        {
            "A", "B", "C", "D",
            "AB", "AC", "AD", "BC", "BD", "CD",
            "ABC", "ABD", "ACD", "BCD",
            "ABCD"
        };


        [TestMethod]
        public void Test_CombinationInheritanceGetRows ()
        {
            int position = 0;
            foreach (ComboSub row in new ComboSub (3).GetRows ())
            {
                string rowText = row.ToString ();
                Assert.AreEqual (expectedCn1[position], rowText);
                ++position;
            }
            Assert.AreEqual (expectedCn1.Length, position);
        }


        [TestMethod]
        public void Test_CombinationInheritanceGetRowsForAllPicks ()
        {
            int position = 0;
            foreach (ComboSub row in new ComboSub ().GetRowsForAllPicks ())
            {
                string rowText = row.ToString ();
                Assert.AreEqual (expectedCn2[position], rowText);
                ++position;
            }
            Assert.AreEqual (expectedCn2.Length, position);
        }
    }

    // ---- ---- ---- ----

    public class MulticomboSub : Multicombination
    {
        public static string[] chars = new string[] { "A", "B" };

        public MulticomboSub (int picks) : base (chars.Length, picks)
        { }

        public override string ToString ()
        {
            string result = "";
            foreach (int element in this)
                result += chars[element];
            return result;
        }
    }


    public partial class TestInheritance
    {
        static string[] expectedMc1
            = new string[] { "AAAA", "AAAB", "AABB", "ABBB", "BBBB" };

        static string[] expectedMc2
            = new string[] { "A", "B", "AA", "AB", "BB", "AAA", "AAB", "ABB", "BBB" };


        [TestMethod]
        public void Test_MulticombinationInheritanceGetRows ()
        {
            int position = 0;
            foreach (MulticomboSub row in new MulticomboSub (4).GetRows ())
            {
                string rowText = row.ToString ();
                Assert.AreEqual (expectedMc1[position], rowText);
                ++position;
            }
            Assert.AreEqual (expectedMc1.Length, position);
        }


        [TestMethod]
        public void TestMulticombinationInheritanceGetRowsForPicks ()
        {
            int position = 0;
            foreach (MulticomboSub row in new MulticomboSub (2).GetRowsForPicks (1, 3))
            {
                string rowText = row.ToString ();
                Assert.AreEqual (expectedMc2[position], rowText);
                ++position;
            }
            Assert.AreEqual (expectedMc2.Length, position);
        }
    }

    // ---- ---- ---- ----

    public class PermutationSub : Permutation
    {
        static string[] chars = new string[] { "A", "B", "C" };

        public PermutationSub ()
            : base (chars.Length)
        { }

        public override string ToString ()
        {
            string result = "";
            foreach (int element in this)
                result += chars[element];
            return result;
        }
    }


    public partial class TestInheritance
    {
        static string[] expectedPn1
            = new string[] { "ABC", "ACB", "BAC", "BCA", "CAB", "CBA" };

        static string[] expectedPn2
            = new string[] { "A", "AB", "BA", "ABC", "ACB", "BAC", "BCA", "CAB", "CBA" };


        [TestMethod]
        public void Test_PermutationInheritanceGetRows ()
        {
            int position = 0;
            foreach (PermutationSub row in new PermutationSub ().GetRows ())
            {
                string rowText = row.ToString ();
                Assert.AreEqual (expectedPn1[position], rowText);
                ++position;
            }
            Assert.AreEqual (expectedPn1.Length, position);
        }


        [TestMethod]
        public void Test_PermutationInheritanceGetRowsForAllChoices ()
        {
            int position = 0;
            foreach (PermutationSub row in new PermutationSub ().GetRowsForAllChoices ())
            {
                string rowText = row.ToString ();
                Assert.AreEqual (expectedPn2[position], rowText);
                ++position;
            }
            Assert.AreEqual (expectedPn2.Length, position);
        }
    }

    // ---- ---- ---- ----

    public class ProductSub : Product
    {
         static string[][] chars
             = new string[][] { new string[] { "A", "B", "C" }, new string[] { "Y", "Z" } };

         static int[] ranges = new int[] { chars[0].Length, chars[1].Length };


        public ProductSub () : base (ranges)
        { }

        public override string ToString ()
        {
            string result = "";
            int position = 0;
            foreach (int element in this)
            {
                result += chars[position][element];
                ++position;
            }
            return result;
        }
    }


    public partial class TestInheritance
    {
        static string[] expectedPt
            = new string[] { "AY", "AZ", "BY", "BZ", "CY", "CZ" };


        [TestMethod]
        public void TestProductInheritanceRows ()
        {
            int position = 0;
            foreach (ProductSub row in new ProductSub ().GetRows ())
            {
                string rowText = row.ToString ();
                Assert.AreEqual (expectedPt[position], rowText);
                ++position;
            }

            Assert.AreEqual (expectedPt.Length, position);
        }
    }
}
