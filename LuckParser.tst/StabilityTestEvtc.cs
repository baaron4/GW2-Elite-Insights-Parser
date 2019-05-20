using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LuckParser.Parser;
using System.IO;
using LuckParser.Exceptions;
using System.Reflection;
using System.Collections.Generic;

namespace LuckParser.tst
{
    [TestClass]
    public class StabilityTestEvtc
    {
        [TestMethod]
        public void TestEvtc()
        {
            string testLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "../../../EvtcLogs/StabilityTest";

            Assert.IsTrue(Directory.Exists(testLocation), "Test Directory missing");

            List<string> failed = new List<string>();
            foreach (string file in Directory.EnumerateFiles(testLocation, "*.evtc", SearchOption.AllDirectories))
            {
                try
                {
                    ParsedLog log = TestHelper.ParseLog(file);
                    TestHelper.JsonString(log);
                    TestHelper.HtmlString(log);
                }
                catch (Exception ex)
                {
                    if (!(ex is TooShortException || ex is SkipException))
                    {
                        failed.Add(file);
                    }
                }
            }

            Assert.IsTrue(failed.Count == 0, failed.ToString());
        }

        [TestMethod]
        public void TestEvtcZip()
        {
            string testLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "../../../EvtcLogs/StabilityTest";

            Assert.IsTrue(Directory.Exists(testLocation), "Test Directory missing");
            List<string> failed = new List<string>();
            foreach (string file in Directory.EnumerateFiles(testLocation, "*.evtc.zip", SearchOption.AllDirectories))
            {
                try
                {
                    ParsedLog log = TestHelper.ParseLog(file);
                    TestHelper.JsonString(log);
                    TestHelper.HtmlString(log);
                }
                catch (Exception ex)
                {
                    if (!(ex is TooShortException || ex is SkipException))
                    {
                        failed.Add(file);
                    }
                }
            }

            Assert.IsTrue(failed.Count == 0, failed.ToString());
        }

        [TestMethod]
        public void TestZevtc()
        {
            string testLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "../../../EvtcLogs/StabilityTest";

            Assert.IsTrue(Directory.Exists(testLocation), "Test Directory missing");

            List<string> failed = new List<string>();
            foreach (string file in Directory.EnumerateFiles(testLocation, "*.zevtc", SearchOption.AllDirectories))
            {
                try
                {
                    ParsedLog log = TestHelper.ParseLog(file);
                    TestHelper.JsonString(log);
                    TestHelper.HtmlString(log);
                }
                catch (Exception ex)
                {
                    if (!(ex is TooShortException || ex is SkipException))
                    {
                        failed.Add(file);
                    }
                }
            }

            Assert.IsTrue(failed.Count == 0, failed.ToString());
        }
    }
}
