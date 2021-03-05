using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coeus.Tests
{
    [TestClass]
    public class OperatorTests
    {
        [TestMethod]
        public void AddInts()
        {
            var json = new JObject();

            var output = JQ.EvalToToken("1 + 2", json);

            Assert.IsNotNull(output);
            Assert.AreEqual(new JValue(3), output);
        }

        [TestMethod]
        public void OperatorPrecedence()
        {
            var json = new JObject();

            var output = JQ.EvalToToken("1 + 2 * 4", json);

            Assert.IsNotNull(output);
            Assert.AreEqual(new JValue(9), output);
        }

        [TestMethod]
        public void ParensPrecedence()
        {
            var json = new JObject();

            var output = JQ.EvalToToken("(1 + 2) * 4", json);

            Assert.IsNotNull(output);
            Assert.AreEqual(new JValue(12), output);
        }

        [TestMethod]
        public void MultiplyStrings()
        {
            var json = new JObject();

            var output = JQ.EvalToToken("\"foo\" * 4", json);

            Assert.IsNotNull(output);
            Assert.AreEqual(new JValue("foofoofoofoo"), output);
        }

        [TestMethod]
        public void AddArrays()
        {
            var json = new JObject();

            var output = JQ.EvalToToken("[1,2] + [3,4]", json);

            Assert.IsNotNull(output);
            Assert.IsTrue(output.Type == JTokenType.Array);
            Assert.AreEqual(4, output.Count());
            Assert.AreEqual(1, output[0].Value<int>());
            Assert.AreEqual(2, output[1].Value<int>());
            Assert.AreEqual(3, output[2].Value<int>());
            Assert.AreEqual(4, output[3].Value<int>());
        }

        [TestMethod]
        public void Equality()
        {
            var json = new JObject
            {
                ["foo"] = 4
            };

            var output = JQ.EvalToToken(".foo == 4", json);

            Assert.IsNotNull(output);
            Assert.IsTrue(output.Type == JTokenType.Boolean);
            Assert.AreEqual(true, output.Value<bool>());
        }

        [TestMethod]
        public void GreaterThan()
        {
            var json = new JObject
            {
                ["foo"] = 4
            };

            var output = JQ.EvalToToken(".foo > 7", json);

            Assert.IsNotNull(output);
            Assert.IsTrue(output.Type == JTokenType.Boolean);
            Assert.AreEqual(false, output.Value<bool>());
        }

        [TestMethod]
        public void BooleanOperators()
        {
            var output = JQ.EvalToToken("1 and 2, null or null, [] and \"foo\" | not", new JObject());

            Assert.IsNotNull(output);
            Assert.IsTrue(output.Type == JTokenType.Array);
            Assert.AreEqual(3, output.Count());
            Assert.AreEqual(false, output[0].Value<bool>());
            Assert.AreEqual(true, output[1].Value<bool>());
            Assert.AreEqual(false, output[2].Value<bool>());
        }
    }
}
