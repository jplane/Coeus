using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Coeus.Results
{
    public class HideErrorResult : ParserResult
    {
        private readonly ParserResult _inner;

        public HideErrorResult(ParserResult inner)
        {
            _inner = inner;
        }

        public override IEnumerable<JToken> Collect(JToken token)
        {
            try
            {
                return _inner.Collect(token).ToArray(); // need to materialize here to ensure error handler is invoked
            }
            catch
            {
                return Enumerable.Empty<JToken>();
            }
        }
    }
}
