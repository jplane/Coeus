using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coeus.Tests
{
    [TestClass]
    public class ErrorTests
    {
        [TestMethod]
        public void Errors()
        {
            Assert.ThrowsException<InvalidOperationException>(() => JQ.EvalToToken("7 | .foo", new JObject()));
        }
    }
}
