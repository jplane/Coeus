using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coeus.Tests
{
    [TestClass]
    public class ArrayTests
    {
        [TestMethod]
        public void ArrayIndex()
        {
            var json = new JObject
            {
                ["foo"] = new JArray { 1, 2, 3 }
            };

            var output = JQ.EvalToToken(".foo[1]", json);

            Assert.IsNotNull(output);
            Assert.AreEqual(2, output.Value<int>());
        }

        [TestMethod]
        public void NegativeArrayIndex()
        {
            var json = new JObject
            {
                ["foo"] = new JArray { 1, 2, 3 }
            };

            var output = JQ.EvalToToken(".foo[-1]", json);

            Assert.IsNotNull(output);
            Assert.AreEqual(3, output.Value<int>());
        }

        [TestMethod]
        public void ArraySlice()
        {
            var json = new JObject
            {
                ["foo"] = new JArray { 1, 2, 3, 4, 5 }
            };

            var output = JQ.EvalToToken(".foo[1:-1]", json);

            Assert.IsNotNull(output);
            Assert.IsTrue(new JArray { 2, 3, 4 }.SequenceEqual(output.Value<JArray>()));
        }

        [TestMethod]
        public void ArraySliceImplicitStart()
        {
            var json = new JObject
            {
                ["foo"] = new JArray { 1, 2, 3, 4, 5 }
            };

            var output = JQ.EvalToToken(".foo[:-1]", json);

            Assert.IsNotNull(output);
            Assert.IsTrue(new JArray { 1, 2, 3, 4 }.SequenceEqual(output.Value<JArray>()));
        }

        [TestMethod]
        public void CreateEmptyArray()
        {
            var json = new JObject();

            var output = JQ.EvalToToken("[]", json);

            Assert.IsNotNull(output);
            Assert.AreEqual(JTokenType.Array, output.Type);
            Assert.AreEqual(0, output.Count());
        }

        [TestMethod]
        public void CreateArray()
        {
            var json = new JObject();

            var output = JQ.EvalToToken("[\"foo\", \"bar\"]", json);

            Assert.IsNotNull(output);
            Assert.IsTrue(new JArray { "foo", "bar" }.SequenceEqual(output.Value<JArray>()));
        }

        [TestMethod]
        public void CreateNestedArray()
        {
            var json = new JObject();

            var output = JQ.EvalToToken("[[\"foo\", \"bar\"]]", json);

            Assert.IsNotNull(output);
            Assert.IsTrue(output.Type == JTokenType.Array);
            Assert.AreEqual(1, output.Count());

            var inner = output[0];

            Assert.IsTrue(new JArray { "foo", "bar" }.SequenceEqual(inner.Value<JArray>()));
        }

        [TestMethod]
        public void NestedArray2()
        {
            var json = new JObject();

            var output = JQ.EvalToToken("[ 7, 8, [9] ]", json);

            Assert.IsNotNull(output);
            Assert.IsTrue(output.Type == JTokenType.Array);
            Assert.AreEqual(3, output.Count());
            Assert.AreEqual(7, output[0].Value<int>());
            Assert.AreEqual(8, output[1].Value<int>());
            Assert.IsTrue(output[2].Type == JTokenType.Array);
            Assert.AreEqual(1, output[2].Count());
            Assert.AreEqual(9, output[2][0].Value<int>());
        }

        [TestMethod]
        public void CommaArray()
        {
            var json = new JObject();

            var output = JQ.EvalToToken("[ 2, 4, 6 ]", json);

            Assert.IsNotNull(output);
            Assert.IsTrue(output.Type == JTokenType.Array);
            Assert.AreEqual(3, output.Count());
            Assert.AreEqual(2, output[0].Value<int>());
            Assert.AreEqual(4, output[1].Value<int>());
            Assert.AreEqual(6, output[2].Value<int>());
        }

        [TestMethod]
        public void PipeArray()
        {
            var json = new JObject();

            var output = JQ.EvalToToken("[ 2 | 4 | 6 ]", json);

            Assert.IsNotNull(output);
            Assert.IsTrue(output.Type == JTokenType.Array);
            Assert.AreEqual(1, output.Count());
            Assert.AreEqual(6, output[0].Value<int>());
        }

        [TestMethod]
        public void PipeThenCommaArray()
        {
            var json = new JObject();

            var output = JQ.EvalToToken("[ 2,4,6 | 1,3,5 | 7,8,9 ]", json);

            Assert.IsNotNull(output);
            Assert.IsTrue(output.Type == JTokenType.Array);
            Assert.AreEqual(27, output.Count());

            static T[][] Chunk<T>(IEnumerable<T> data, int size)
            {
                return data
                  .Select((x, i) => new { Index = i, Value = x })
                  .GroupBy(x => x.Index / size)
                  .Select(x => x.Select(v => v.Value).ToArray())
                  .ToArray();
            }

            foreach (var chunk in Chunk(output.Values<int>(), 3))
            {
                Assert.AreEqual(7, chunk[0]);
                Assert.AreEqual(8, output[1]);
                Assert.AreEqual(9, output[2]);
            }
        }

        [TestMethod]
        public void CommaThenPipeArray()
        {
            var json = new JObject();

            var output = JQ.EvalToToken("[ 2|4|6 , 1|3|5 , 7|8|9 ]", json);

            Assert.IsNotNull(output);
            Assert.IsTrue(output.Type == JTokenType.Array);
            Assert.AreEqual(4, output.Count());
            Assert.AreEqual(9, output[0].Value<int>());
            Assert.AreEqual(9, output[1].Value<int>());
            Assert.AreEqual(9, output[2].Value<int>());
            Assert.AreEqual(9, output[3].Value<int>());
        }
    }
}
