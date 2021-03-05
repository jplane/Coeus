using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Coeus.Results
{
    public class LhsAssignmentResult : ParserResult
    {
        private readonly ParserResult[] _results;

        public LhsAssignmentResult(IEnumerable<ParserResult> results)
        {
            _results = results.ToArray();
        }

        public override IEnumerable<JToken> Collect(JToken token)
        {
            var targets = new List<JToken>
            {
                token
            };

            foreach (var result in _results)
            {
                var innerTargets = new List<JToken>();

                foreach (var target in targets)
                {
                    innerTargets.AddRange(result.Collect(target));
                }

                targets = innerTargets;
            }

            return targets;
        }
    }

    public class LhsPropertyAssignmentResult : ParserResult
    {
        private readonly string _propertyName;

        public LhsPropertyAssignmentResult(string propertyName)
        {
            _propertyName = propertyName;
        }

        public override IEnumerable<JToken> Collect(JToken token)
        {
            JProperty processObject(JObject jobj)
            {
                var prop = jobj.Property(_propertyName);

                if (prop == null)
                {
                    prop = new JProperty(_propertyName, JValue.CreateNull());
                    jobj.Add(prop);
                }

                return prop;
            }

            JProperty result = null;

            if (token is JObject jobj)
            {
                result = processObject(jobj);
            }
            else if (token is JProperty prop)
            {
                var value = prop.Value;

                Debug.Assert(value != null);

                if (value.Type == JTokenType.Null)
                {
                    prop.Value = value = new JObject();
                }

                if (value.Type == JTokenType.Object)
                {
                    result = processObject((JObject)value);
                }
            }

            if (result == null)
            {
                throw new InvalidOperationException("Can only update assign to JObject.");
            }

            yield return result;
        }
    }

    public class LhsAssignmentPipeResult : ParserResult
    {
        private readonly ParserResult _lhs;
        private readonly ParserResult _rhs;

        public LhsAssignmentPipeResult(ParserResult lhs, ParserResult rhs)
        {
            _lhs = lhs;
            _rhs = rhs;
        }

        public override IEnumerable<JToken> Collect(JToken token)
        {
            var props = _lhs.Collect(token).ToArray();

            Debug.Assert(props.Length == 1);
            Debug.Assert(props[0] is JProperty);

            var prop = (JProperty)props[0];

            var lhsValue = prop.Value;

            Debug.Assert(lhsValue != null);

            if (lhsValue.Type == JTokenType.Null)
            {
                prop.Value = lhsValue = new JObject();
            }

            props = _rhs.Collect(lhsValue).ToArray();

            Debug.Assert(props.Length == 1);
            Debug.Assert(props[0] is JProperty);

            yield return props[0];
        }
    }

    public class LhsAssignmentCommaResult : ParserResult
    {
        private readonly ParserResult _lhs;
        private readonly ParserResult _rhs;

        public LhsAssignmentCommaResult(ParserResult lhs, ParserResult rhs)
        {
            _lhs = lhs;
            _rhs = rhs;
        }

        public override IEnumerable<JToken> Collect(JToken token)
        {
            foreach (var lhs in _lhs.Collect(token))
            {
                yield return lhs;
            }

            foreach (var rhs in _rhs.Collect(token))
            {
                yield return rhs;
            }
        }
    }
}
