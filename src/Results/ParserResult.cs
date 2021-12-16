using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Coeus.Results
{
    public abstract class ParserResult
    {
        protected ParserResult()
        {
        }

        public abstract IEnumerable<JToken> Collect(JToken token);
    }
}
