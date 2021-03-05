using Coeus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Coeus.Tests
{
    [TestClass]
    public class JqTests
    {
        [TestMethod]
        public void Identity()
        {
            var json = new JObject
            {
                ["foo"] = 4
            };

            var output = JQ.EvalToToken(".", json);

            Assert.IsNotNull(output);
            Assert.IsNotNull(output["foo"]);
            Assert.AreEqual(4, output["foo"].Value<int>());
        }
    }
}
