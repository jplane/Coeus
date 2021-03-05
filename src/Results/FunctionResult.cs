using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coeus.Results
{
    public class FunctionResult : ParserResult
    {
        private readonly Func<JToken, IEnumerable<JToken>> _func;

        public FunctionResult(Func<JToken, IEnumerable<JToken>> func)
        {
            _func = func;
        }

        public override IEnumerable<JToken> Collect(JToken token)
        {
            return _func(token);
        }
    }
}
