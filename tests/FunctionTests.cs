using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Coeus.Tests
{
    [TestClass]
    public class FunctionTests
    {
        [TestMethod]
        public void Length()
        {
            var output = JQ.EvalToToken("[1,2], \"string\", {\"a\":2}, null | length", new JObject());

            Assert.IsNotNull(output);
            Assert.IsTrue(output.Type == JTokenType.Array);
            Assert.AreEqual(4, output.Count());
            Assert.AreEqual(2, output[0].Value<int>());
            Assert.AreEqual(6, output[1].Value<int>());
            Assert.AreEqual(1, output[2].Value<int>());
            Assert.AreEqual(0, output[3].Value<int>());
        }

        [TestMethod]
        public void Select()
        {
            var jArr = new JArray();
            var person = new JObject
            {
                { "age", 43 }
            };
            var secondPerson = new JObject
            {
                { "age", 30 }
            };
            jArr.Add(person);
            jArr.Add(secondPerson);
            var people = new JObject
            {
                { "people", jArr }
            };
            var output = JQ.EvalToToken("{ \"people\" : (.people | select(.[].age > 40)) }", people);
            Assert.IsTrue(output.Type == JTokenType.Object);
            Assert.AreEqual(1, output.SelectToken("people").Children().Count());
        }

        [TestMethod]
        public void HasObject()
        {
            var output = JQ.EvalToToken("{ \"foo\": 7 } | has(\"foo\")", new JObject());

            Assert.IsNotNull(output);
            Assert.IsTrue(output.Type == JTokenType.Boolean);
            Assert.AreEqual(true, output.Value<bool>());
        }

        [TestMethod]
        public void HasArray()
        {
            var output = JQ.EvalToToken("[1,2,3] | has(0)", new JObject());

            Assert.IsNotNull(output);
            Assert.IsTrue(output.Type == JTokenType.Boolean);
            Assert.AreEqual(true, output.Value<bool>());
        }

        [TestMethod]
        public void KeysObject()
        {
            var output = JQ.EvalToToken("{ \"foo\": 7, \"bar\": \"hello\" } | keys", new JObject());

            Assert.IsNotNull(output);
            Assert.IsTrue(output.Type == JTokenType.Array);
            Assert.AreEqual(2, output.Count());
            Assert.AreEqual("bar", output[0].Value<string>());
            Assert.AreEqual("foo", output[1].Value<string>());
        }

        [TestMethod]
        public void KeysArray()
        {
            var output = JQ.EvalToToken("[1,2,3] | keys", new JObject());

            Assert.IsNotNull(output);
            Assert.IsTrue(output.Type == JTokenType.Array);
            Assert.AreEqual(3, output.Count());
            Assert.AreEqual(0, output[0].Value<int>());
            Assert.AreEqual(1, output[1].Value<int>());
            Assert.AreEqual(2, output[2].Value<int>());
        }
    }
}
