using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Coeus.Results
{
    public class AssignmentResult : ParserResult
    {
        private readonly ParserResult _lhs;
        private readonly ParserResult _rhs;

        public AssignmentResult(ParserResult lhs, ParserResult rhs)
        {
            _lhs = lhs;
            _rhs = rhs;
        }

        public override IEnumerable<JToken> Collect(JToken token)
        {
            var rhsValues = _rhs.Collect(token).ToArray();

            foreach (var rhs in rhsValues)
            {
                var target = token.DeepClone();

                var lhsValues = _lhs.Collect(target).ToArray();

                foreach (var lhs in lhsValues)
                {
                    if (lhs.Type == JTokenType.Property)
                    {
                        ((JProperty) lhs).Value = rhs;
                    }
                    else
                    {
                        target = rhs;
                    }
                }

                yield return target;
            }
        }
    }
}
