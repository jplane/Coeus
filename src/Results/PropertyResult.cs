using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Coeus.Results
{
    public class PropertyResult : ParserResult
    {
        private readonly string _propertyName;

        public PropertyResult(string propertyName)
        {
            _propertyName = propertyName;
        }

        public override IEnumerable<JToken> Collect(JToken token)
        {
            JToken value = null;

            if (token is JObject jobj)
            {
                if (!jobj.TryGetValue(_propertyName, out value))
                {
                    value = JValue.CreateNull();
                }
            }
            else if (token.Type == JTokenType.Null)
            {
                value = JValue.CreateNull();
            }
            else
            {
                throw new InvalidOperationException("Expecting JObject.");
            }

            yield return value;
        }
    }
}
