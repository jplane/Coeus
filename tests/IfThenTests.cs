using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coeus.Tests
{
    [TestClass]
    public class IfThenTests
    {
        [TestMethod]
        public void IfThenTrue()
        {
            var output = JQ.EvalToToken("if true then 7 end", new JObject());

            Assert.IsNotNull(output);
            Assert.IsTrue(output.Type == JTokenType.Integer);
            Assert.AreEqual(7, output.Value<int>());
        }

        [TestMethod]
        public void IfThenFalse()
        {
            var output = JQ.EvalToToken("if false then 7 end", new JObject());

            Assert.IsNotNull(output);
            Assert.IsTrue(output.Type == JTokenType.Object);
        }

        [TestMethod]
        public void IfThenElse()
        {
            var output = JQ.EvalToToken("if true, false then 7 else 8 end", new JObject());

            Assert.IsNotNull(output);
            Assert.IsTrue(output.Type == JTokenType.Array);
            Assert.AreEqual(2, output.Count());
            Assert.AreEqual(7, output[0].Value<int>());
            Assert.AreEqual(8, output[1].Value<int>());
        }

        [TestMethod]
        public void IfThenElifElse()
        {
            var output = JQ.EvalToToken("1,3,5 | if . > 4 then . * 2 elif . > 2 then . else 0 end", new JObject());

            Assert.IsNotNull(output);
            Assert.IsTrue(output.Type == JTokenType.Array);
            Assert.AreEqual(3, output.Count());
            Assert.AreEqual(0, output[0].Value<int>());
            Assert.AreEqual(3, output[1].Value<int>());
            Assert.AreEqual(10, output[2].Value<int>());
        }
    }
}
