using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

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
