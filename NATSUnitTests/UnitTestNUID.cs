﻿// Copyright 2015 Apcera Inc. All rights reserved.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NATS.Client;
using System.Threading;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

namespace NATS.Client
{
    /// <summary>
    /// Run these tests with the gnatsd auth.conf configuration file.
    /// </summary>
    [TestClass]
    public class TestNUID
    {
        [TestMethod]
        public void TestGlobalNUID()
        {
            NUID n = NUID.Instance;
            Assert.IsNotNull(n);
            Assert.IsNotNull(n.Pre);
            Assert.AreNotEqual(0, n.Seq);
        }

        [TestMethod]
        public void TestNUIDRollover()
        {
            NUID gnuid = NUID.Instance;
            gnuid.Seq = NUID.MAXSEQ;

            byte[] prefix = new byte[gnuid.Pre.Length];
            Array.Copy(gnuid.Pre, prefix, gnuid.Pre.Length);

            string nextvalue = gnuid.Next;

            bool areEqual = true;
            for (int i = 0; i < gnuid.Pre.Length; i++)
            {
                if (prefix[i] != gnuid.Pre[i])
                    areEqual = false;
            }

            Assert.IsFalse(areEqual);
        }

        [TestMethod]
        public void TestNUIDLen()
        {
            string nuid = new NUID().Next;
            Assert.IsTrue(nuid.Length == NUID.LENGTH);
        }

        static void printElapsedTime(long operations, Stopwatch sw)
        {
            System.Console.WriteLine("Elapsed Ticks = " + sw.ElapsedTicks);
            System.Console.WriteLine("Stopwatch freq = " + Stopwatch.Frequency);
            double nanoseconds = ((double)sw.ElapsedTicks /
                ((double)Stopwatch.Frequency) * (double)1000000000);
            System.Console.WriteLine("Performed {0} operations in {1} nanos.  {2} ns/op",
                operations, nanoseconds, (long)(nanoseconds /(double)operations));
        }

        private void runNUIDSpeedTest(NUID n)
        {
            long count = 10000000;

            Stopwatch sw = Stopwatch.StartNew();

            for (long i = 0; i < count; i++)
            {
                string nuid = n.Next;
            }

            sw.Stop();

            printElapsedTime(count, sw);
        }

        [TestMethod]
        public void TestNUIDSpeed()
        {
            runNUIDSpeedTest(NUID.Instance);
        }

        [TestMethod]
        public void TestGlobalNUIDSpeed()
        {
            runNUIDSpeedTest(new NUID());
        }

        [TestMethod]
        public void TestNuidBasicUniqueess()
        {
            int count = 1000000;
            IDictionary<string, bool> m = new Dictionary<string, bool>(count);

            for (int i = 0; i < count; i++)
            {
                String n = NUID.NextGlobal;
                if (m.ContainsKey(n))
                {
                    Assert.Fail("Duplicate NUID found: " + n);
                }
                else
                {
                    m.Add(n, true);
                }
            }
        }

    } // class

} // namespace

