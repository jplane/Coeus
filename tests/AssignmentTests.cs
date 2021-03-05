using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coeus.Tests
{
    [TestClass]
    public class AssignmentTests
    {
        [TestMethod]
        public void PropAssign()
        {
            var output = JQ.EvalToToken(".foo.bar = (2, 3)", new JObject());

            Assert.IsNotNull(output);
            Assert.IsTrue(output.Type == JTokenType.Array);
            Assert.AreEqual(2, output[0]["foo"]["bar"].Value<int>());
            Assert.AreEqual(3, output[1]["foo"]["bar"].Value<int>());
        }

        [TestMethod]
        public void Assign()
        {
            var output = JQ.EvalToToken(". = (2, 3)", new JObject());

            Assert.IsNotNull(output);
            Assert.IsTrue(output.Type == JTokenType.Array);
            Assert.AreEqual(2, output.Count());
            Assert.AreEqual(2, output[0].Value<int>());
            Assert.AreEqual(3, output[1].Value<int>());
        }
    }
}
