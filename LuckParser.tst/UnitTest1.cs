using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LuckParser.Parser;
using System.IO;

namespace LuckParser.tst
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            ParsedLog log = TestHelper.ParseLog("");
        }
    }
}
