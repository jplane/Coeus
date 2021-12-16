using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Coeus.Results
{
    public class PropertyResult : ParserResult
    {
        private readonly string _propertyName;

        public PropertyResult(string propertyName)
        {
            _propertyName = propertyName;
        }

        internal string PropertyName { get => _propertyName; }

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
