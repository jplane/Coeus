using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Coeus.Results
{
    public class PipeResult : ParserResult
    {
        private readonly ParserResult[] _results;

        public PipeResult(ParserResult lhs, ParserResult rhs)
        {
            _results = new[] { lhs, rhs };
        }

        public PipeResult(IEnumerable<ParserResult> results)
        {
            _results = results.ToArray();
        }

        public EmptyObject CreateEmptyObject()
        {
            var result = new JObject();
            JObject child = null;
            var propertyNames = _results.Where(r => r is PropertyResult).Cast<PropertyResult>().Select(p => p.PropertyName);
            for(int i = 0; i < propertyNames.Count(); i++)
            {
                var name = propertyNames.ElementAt(i);
                if (child == null)
                {
                    child = new JObject();
                    result.Add(name, child);
                }
                else
                {
                    var record = new JObject();
                    child.Add(name, record);
                    child = record;
                }
            }

            return new EmptyObject(result, string.Join(".", propertyNames));
        }

        public override IEnumerable<JToken> Collect(JToken token)
        {
            var tokens = new List<JToken>
            {
                token
            };

            foreach (var result in _results)
            {
                var inner = new List<JToken>();

                foreach (var innerToken in tokens)
                {
                    inner.AddRange(result.Collect(innerToken));
                }

                tokens = inner;
            }

            return tokens;
        }
    }
}
