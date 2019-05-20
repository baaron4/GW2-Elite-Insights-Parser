using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LuckParser.Parser;
using System.IO;
using LuckParser.Exceptions;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

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
            List<string> toCheck = Directory.EnumerateFiles(testLocation, "*.evtc", SearchOption.AllDirectories).ToList();
            foreach (string file in toCheck)
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
            List<string> toCheck = Directory.EnumerateFiles(testLocation, "*.evtc", SearchOption.AllDirectories).ToList();
            foreach (string file in toCheck)
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
            List<string> toCheck = Directory.EnumerateFiles(testLocation, "*.evtc", SearchOption.AllDirectories).ToList();
            foreach (string file in toCheck)
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
