using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Coeus.Results
{
    public class AtomicResult : ParserResult
    {
        private readonly Func<JToken, JToken> _func;

        public AtomicResult(Func<JToken, JToken> func)
        {
            _func = func;
        }

        public override IEnumerable<JToken> Collect(JToken token)
        {
            yield return _func(token);
        }
    }
}
