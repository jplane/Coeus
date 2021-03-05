using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coeus.Tests
{
    [TestClass]
    public class PipeTests
    {
        [TestMethod]
        public void Pipe()
        {
            var json = new JObject
            {
                ["foo"] = new JArray
                {
                    new JObject
                    {
                        ["bar"] = 7
                    },
                    new JObject
                    {
                        ["bar"] = 8
                    }
                }
            };

            var output = JQ.EvalToToken(".foo[] | .bar", json);

            Assert.IsNotNull(output);
            Assert.IsTrue(new JArray { 7, 8 }.SequenceEqual(output.Value<JArray>()));
        }

        [TestMethod]
        public void ChainedPipe()
        {
            var json = new JObject
            {
                ["foo"] = new JObject
                {
                    ["bar"] = 7
                }
            };

            var output = JQ.EvalToToken(". | . | .foo.bar", json);

            Assert.IsNotNull(output);
            Assert.AreEqual(new JValue(7), output);
        }
    }
}
