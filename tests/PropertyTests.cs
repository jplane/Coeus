using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coeus.Tests
{
    [TestClass]
    public class PropertyTests
    {
        [TestMethod]
        public void NonExistentProperty()
        {
            var json = new JObject
            {
                ["foo"] = 4
            };

            var output = JQ.EvalToToken(".bar", json);

            Assert.IsNotNull(output);
            Assert.IsTrue(output.Type == JTokenType.Null);
        }

        [TestMethod]
        public void Property()
        {
            var json = new JObject
            {
                ["foo"] = 4
            };

            var output = JQ.EvalToToken(".foo", json);

            Assert.IsNotNull(output);
            Assert.AreEqual(4, output.Value<int>());
        }

        [TestMethod]
        public void NestedIdentity()
        {
            var json = new JObject
            {
                ["foo"] = new JObject
                {
                    ["bar"] = 7
                }
            };

            var output = JQ.EvalToToken(".foo.bar", json);

            Assert.IsNotNull(output);
            Assert.AreEqual(new JValue(7), output);
        }

        [TestMethod]
        public void OptionalProperty()
        {
            var output = JQ.EvalToToken("7 | .foo?", new JObject());

            Assert.IsNotNull(output);
            Assert.IsTrue(output.Type == JTokenType.Array);
            Assert.AreEqual(0, output.Count());
        }
    }
}
