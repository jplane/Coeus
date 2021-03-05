using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coeus.Tests
{
    [TestClass]
    public class UpdateAssignmentTests
    {
        [TestMethod]
        public void UpdateAssign()
        {
            var output = JQ.EvalToToken(".foo |= . + 1", new JObject());

            Assert.IsNotNull(output);
            Assert.IsTrue(output.Type == JTokenType.Object);
            Assert.AreEqual(1, output["foo"].Value<int>());
        }

        [TestMethod]
        public void UpdateAssignComma()
        {
            var output = JQ.EvalToToken("(.foo, .bar) |= . + 1", new JObject());

            Assert.IsNotNull(output);
            Assert.IsTrue(output.Type == JTokenType.Object);
            Assert.AreEqual(1, output["foo"].Value<int>());
            Assert.AreEqual(1, output["bar"].Value<int>());
        }

        [TestMethod]
        public void UpdateAssignPipe()
        {
            var output = JQ.EvalToToken("(.foo | .bar) |= . + 1", new JObject());

            Assert.IsNotNull(output);
            Assert.IsTrue(output.Type == JTokenType.Object);
            Assert.IsTrue(output["foo"].Type == JTokenType.Object);
            Assert.AreEqual(1, output["foo"]["bar"].Value<int>());
        }

        [TestMethod]
        public void UpdateAssignAdd()
        {
            var output = JQ.EvalToToken(".foo.bar += 2", new JObject());

            Assert.IsNotNull(output);
            Assert.IsTrue(output.Type == JTokenType.Object);
            Assert.AreEqual(2, output["foo"]["bar"].Value<int>());
        }

        [TestMethod]
        public void UpdateAssignObjectToScalar()
        {
            var output = JQ.EvalToToken(". |= 7", new JObject());

            Assert.IsNotNull(output);
            Assert.IsTrue(output.Type == JTokenType.Integer);
            Assert.AreEqual(7, output.Value<int>());
        }
    }
}
