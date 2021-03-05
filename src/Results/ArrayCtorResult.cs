using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Coeus.Results
{
    public class ArrayCtorResult : ParserResult
    {
        private readonly ParserResult _result;

        public ArrayCtorResult(ParserResult result)
        {
            _result = result;
        }

        public override IEnumerable<JToken> Collect(JToken token)
        {
            if (_result != null)
            {
                yield return new JArray(_result.Collect(token));
            }
            else
            {
                yield return new JArray();
            }
        }
    }
}
