using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Coeus.Results
{
    public class ObjectCtorResult : ParserResult
    {
        private readonly IEnumerable<Func<JToken, IEnumerable<JObject>, IEnumerable<JObject>>> _propDefs;

        public ObjectCtorResult(IEnumerable<Func<JToken, IEnumerable<JObject>, IEnumerable<JObject>>> propDefs)
        {
            _propDefs = propDefs;
        }

        public override IEnumerable<JToken> Collect(JToken token)
        {
            var results = new List<JObject>
            {
                new JObject()
            };

            foreach (var generator in _propDefs)
            {
                results = new List<JObject>(generator(token, results));
            }

            return results;
        }
    }
}
