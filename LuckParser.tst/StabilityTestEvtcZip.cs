using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LuckParser.Parser;
using System.IO;
using LuckParser.Exceptions;
using System.Reflection;

namespace LuckParser.tst
{
    [TestClass]
    public class StabilityTestEvtcZip
    {
        [TestMethod]
        public void TestMethod1()
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
                        if (ex.InnerException == null || (ex.InnerException.GetType() == typeof(SkipException) || ex.InnerException.GetType() == typeof(TooShortException)))
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
                        if (ex.InnerException == null || (ex.InnerException.GetType() == typeof(SkipException) || ex.InnerException.GetType() == typeof(TooShortException)))
                        {
                            Assert.Fail(file + " has failed to generate html");
                        }
                    }
                } catch (CancellationException ex)
                {
                    if (ex.InnerException == null || (ex.InnerException.GetType() == typeof(SkipException) || ex.InnerException.GetType() == typeof(TooShortException)))
                    {
                        Assert.Fail(file + " has failed to parse properly");
                    }
                }
            }

            Assert.IsTrue(true);
        }
    }
}
