using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coeus.Tests
{
    [TestClass]
    public class RecursiveDescentTests
    {
        [TestMethod]
        public void RecursiveDescentScalar()
        {
            var json = new JObject();

            var output = JQ.EvalToToken("45 | ..", json);

            Assert.IsNotNull(output);
            Assert.IsTrue(output.Type == JTokenType.Integer);
            Assert.AreEqual(45, output.Value<int>());
        }

        [TestMethod]
        public void RecursiveDescentEmptyObject()
        {
            var json = new JObject();

            var output = JQ.EvalToToken("{} | ..", json);

            Assert.IsNotNull(output);
            Assert.IsTrue(output.Type == JTokenType.Object);
        }

        [TestMethod]
        public void RecursiveDescentObject()
        {
            var json = new JObject();

            var output = JQ.EvalToToken("{ \"foo\": 7 } | ..", json);

            Assert.IsNotNull(output);
            Assert.IsTrue(output.Type == JTokenType.Array);
            Assert.AreEqual(2, output.Count());
            Assert.IsTrue(output[0].Type == JTokenType.Object);
            Assert.IsTrue(output[1].Type == JTokenType.Integer);
        }

        [TestMethod]
        public void RecursiveDescentEmptyArray()
        {
            var json = new JObject();

            var output = JQ.EvalToToken("[] | ..", json);

            Assert.IsNotNull(output);
            Assert.IsTrue(output.Type == JTokenType.Array);
            Assert.AreEqual(0, output.Count());
        }

        [TestMethod]
        public void RecursiveDescentArray()
        {
            var json = new JObject();

            var output = JQ.EvalToToken("[ 7, 8, 9 ] | ..", json);

            Assert.IsNotNull(output);
            Assert.IsTrue(output.Type == JTokenType.Array);
            Assert.AreEqual(4, output.Count());
            Assert.IsTrue(output[0].Type == JTokenType.Array);
            Assert.AreEqual(7, output[1].Value<int>());
            Assert.AreEqual(8, output[2].Value<int>());
            Assert.AreEqual(9, output[3].Value<int>());
        }

        [TestMethod]
        public void RecursiveDescentNestedArray()
        {
            var json = new JObject();

            var output = JQ.EvalToToken("[ 7, 8, [9] ] | ..", json);

            Assert.IsNotNull(output);
            Assert.IsTrue(output.Type == JTokenType.Array);
            Assert.AreEqual(5, output.Count());
            Assert.IsTrue(output[0].Type == JTokenType.Array);
            Assert.AreEqual(7, output[1].Value<int>());
            Assert.AreEqual(8, output[2].Value<int>());
            Assert.IsTrue(output[3].Type == JTokenType.Array);
            Assert.AreEqual(1, output[3].Count());
            Assert.AreEqual(9, output[4].Value<int>());
        }
    }
}
