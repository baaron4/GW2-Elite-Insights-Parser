using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LuckParser.Parser;
using System.IO;
using LuckParser.Exceptions;
using System.Reflection;

namespace LuckParser.tst
{
    [TestClass]
    public class StabilityTestEvtc
    {
        private bool CheckException(CancellationException ex)
        {
            return ex.InnerException == null || (ex.InnerException.GetType() != typeof(SkipException) && ex.InnerException.GetType() != typeof(TooShortException));
        }

        [TestMethod]
        public void TestEvtc()
        {
            string testLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "../../../EvtcLogs/StabilityTest";

            Assert.IsTrue(Directory.Exists(testLocation), "Test Directory missing");

            foreach (string file in Directory.EnumerateFiles(testLocation, "*.evtc", SearchOption.AllDirectories))
            {
                try
                {
                    ParsedLog log = TestHelper.ParseLog(file);
                    try
                    {
                        TestHelper.JsonString(log);
                    }
                    catch (CancellationException ex)
                    {
                        if (CheckException(ex))
                        {
                            Assert.Fail(file + " has failed to generate json");
                        }
                    }
                    try
                    {
                        TestHelper.HtmlString(log);
                    }
                    catch (CancellationException ex)
                    {
                        if (CheckException(ex))
                        {
                            Assert.Fail(file + " has failed to generate html");
                        }
                    }
                }
                catch (CancellationException ex)
                {
                    if (CheckException(ex))
                    {
                        Assert.Fail(file + " has failed to parse properly");
                    }
                }
            }

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestEvtcZip()
        {
            string testLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "../../../EvtcLogs/StabilityTest";

            Assert.IsTrue(Directory.Exists(testLocation), "Test Directory missing");

            foreach (string file in Directory.EnumerateFiles(testLocation, "*.evtc.zip", SearchOption.AllDirectories))
            {
                try
                {
                    ParsedLog log = TestHelper.ParseLog(file);
                    try
                    {
                        TestHelper.JsonString(log);
                    }
                    catch (CancellationException ex)
                    {
                        if (CheckException(ex))
                        {
                            Assert.Fail(file + " has failed to generate json");
                        }
                    }
                    try
                    {
                        TestHelper.HtmlString(log);
                    }
                    catch (CancellationException ex)
                    {
                        if (CheckException(ex))
                        {
                            Assert.Fail(file + " has failed to generate html");
                        }
                    }
                }
                catch (CancellationException ex)
                {
                    if (CheckException(ex))
                    {
                        Assert.Fail(file + " has failed to parse properly");
                    }
                }
            }

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestZevtc()
        {
            string testLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "../../../EvtcLogs/StabilityTest";

            Assert.IsTrue(Directory.Exists(testLocation), "Test Directory missing");

            foreach (string file in Directory.EnumerateFiles(testLocation, "*.zevtc", SearchOption.AllDirectories))
            {
                try
                {
                    ParsedLog log = TestHelper.ParseLog(file);
                    try
                    {
                        TestHelper.JsonString(log);
                    }
                    catch (CancellationException ex)
                    {
                        if (CheckException(ex))
                        {
                            Assert.Fail(file + " has failed to generate json");
                        }
                    }
                    try
                    {
                        TestHelper.HtmlString(log);
                    }
                    catch (CancellationException ex)
                    {
                        if (CheckException(ex))
                        {
                            Assert.Fail(file + " has failed to generate html");
                        }
                    }
                }
                catch (CancellationException ex)
                {
                    if (CheckException(ex))
                    {
                        Assert.Fail(file + " has failed to parse properly");
                    }
                }
            }

            Assert.IsTrue(true);
        }
    }
}
