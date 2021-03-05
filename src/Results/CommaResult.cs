using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Coeus.Results
{
    public class CommaResult : ParserResult
    {
        private readonly ParserResult[] _results;

        public CommaResult(ParserResult lhs, ParserResult rhs)
        {
            _results = new[] { lhs, rhs };
        }

        public CommaResult(IEnumerable<ParserResult> results)
        {
            _results = results.ToArray();
        }

        public override IEnumerable<JToken> Collect(JToken token)
        {
            foreach (var resultToken in _results.SelectMany(result => result.Collect(token)))
            {
                yield return resultToken;
            }
        }
    }
}
