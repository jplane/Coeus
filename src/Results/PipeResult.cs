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
