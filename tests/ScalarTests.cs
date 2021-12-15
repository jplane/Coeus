using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Coeus.Tests
{
    [TestClass]
    public class ScalarTests
    {
        [TestMethod]
        public void ScalarBool()
        {
            var json = new JObject();

            var output = JQ.EvalToToken("false", json);

            Assert.IsNotNull(output);
            Assert.AreEqual(new JValue(false), output);
        }

        // [TestMethod]
        // public void ScalarFloatLeadingDecimal()
        // {
        //     var json = new JObject();
        // 
        //     var output = JQ.EvalToToken("-.44", json);
        // 
        //     Assert.IsNotNull(output);
        //     Assert.AreEqual(new JValue(-0.44d), output);
        // }

        [TestMethod]
        public void ScalarString()
        {
            var json = new JObject();

            var output = JQ.EvalToToken("\"foo\" | \"bar\"", json);

            Assert.IsNotNull(output);
            Assert.AreEqual(new JValue("bar"), output);
        }

        [TestMethod]
        public void ParensScalar()
        {
            var json = new JObject();

            var output = JQ.EvalToToken("(\"bar\")", json);

            Assert.IsNotNull(output);
            Assert.AreEqual(new JValue("bar"), output);
        }
    }
}
