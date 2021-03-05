using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace Coeus.Results
{
    public class UpdateAssignmentResult : ParserResult
    {
        private readonly string _operator;
        private readonly ParserResult _lhs;
        private readonly ParserResult _rhs;

        public UpdateAssignmentResult(string op, ParserResult lhs, ParserResult rhs)
        {
            _operator = op;
            _lhs = lhs;
            _rhs = rhs;
        }

        public override IEnumerable<JToken> Collect(JToken token)
        {
            var func = GetOperatorAction();

            var targets = _lhs.Collect(token).ToArray();

            JToken output = null;

            foreach (var target in targets)
            {
                output = func(token, target);
            }

            yield return output;
        }

        private Func<JToken, JToken, JToken> GetOperatorAction()
        {
            switch (_operator)
            {
                case "|=":
                    return Update;
                case "+=":
                    return (original, prop) => Math('+', original, prop);
                case "-=":
                    return (original, prop) => Math('-', original, prop);
                case "*=":
                    return (original, prop) => Math('*', original, prop);
                case "/=":
                    return (original, prop) => Math('/', original, prop);

                default:
                    Debug.Fail("Unexpected update assignment operator: " + _operator);
                    return null;
            }
        }

        private JToken Math(char op, JToken original, JToken target)
        {
            var lhs = new AtomicResult(token => token);

            var mathResult = new MathResult(op, lhs, _rhs);

            if (target is JProperty prop)
            {
                var values = mathResult.Collect(prop.Value);

                prop.Value = values.FirstOrDefault() ?? JValue.CreateNull();

                return original;
            }
            else
            {
                return mathResult.Collect(target).FirstOrDefault() ?? JValue.CreateNull();
            }
        }

        private JToken Update(JToken original, JToken target)
        {
            if (target is JProperty prop)
            {
                var rhsValues = _rhs.Collect(prop.Value).ToArray();

                if (rhsValues.Length == 0)
                {
                    prop.Remove();
                }
                else
                {
                    prop.Value = rhsValues.First();
                }

                return original;
            }
            else
            {
                return _rhs.Collect(target).FirstOrDefault() ?? JValue.CreateNull();
            }
        }
    }
}
