using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coeus.Tests
{
    [TestClass]
    public class ObjectTests
    {
        [TestMethod]
        public void CreateEmptyObject()
        {
            var json = new JObject();

            var output = JQ.EvalToToken("{}", json);

            Assert.IsNotNull(output);
            Assert.IsTrue(output.Type == JTokenType.Object);
        }

        [TestMethod]
        public void CreateObject()
        {
            var json = new JObject();

            var output = JQ.EvalToToken("{ \"foo\": \"bar\" }", json);

            Assert.IsNotNull(output);
            Assert.IsTrue(output.Type == JTokenType.Object);
            Assert.AreEqual("bar", output.Value<string>("foo"));
        }

        [TestMethod]
        public void DereferenceObject()
        {
            var json = new JObject();

            var output = JQ.EvalToToken("({ \"foo\": \"bar\" }) | .foo", json);

            Assert.IsNotNull(output);
            Assert.IsTrue(output.Type == JTokenType.String);
            Assert.AreEqual("bar", output.Value<string>());
        }

        [TestMethod]
        public void DereferenceObject2()
        {
            var json = new JObject();

            var output = JQ.EvalToToken("({ \"foo\": \"bar\" }).foo", json);

            Assert.IsNotNull(output);
            Assert.IsTrue(output.Type == JTokenType.String);
            Assert.AreEqual("bar", output.Value<string>());
        }

        [TestMethod]
        public void CreateObjectFromLookupValue()
        {
            var json = new JObject
            {
                ["foo"] = new JArray { 1, 2, 3 }
            };

            var output = JQ.EvalToToken("{ \"bar\": .foo[2] }", json);

            Assert.IsNotNull(output);
            Assert.IsTrue(output.Type == JTokenType.Object);
            Assert.AreEqual(3, output.Value<int>("bar"));
        }
    }
}
