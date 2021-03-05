using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Coeus.Results
{
    public class MathResult : ParserResult
    {
        private readonly char _operator;
        private readonly ParserResult _lhs;
        private readonly ParserResult _rhs;

        public MathResult(char op, ParserResult lhs, ParserResult rhs)
        {
            _operator = op;
            _lhs = lhs;
            _rhs = rhs;
        }

        public override IEnumerable<JToken> Collect(JToken token)
        {
            Func<JToken, JToken, JToken> op = GetOperatorFunc();

            var lhsTokens = _lhs.Collect(token);

            var rhsTokens = _rhs.Collect(token);

            var results = new List<JToken>();

            foreach (var lhs in lhsTokens)
            {
                foreach (var rhs in rhsTokens)
                {
                    results.Add(op(lhs, rhs));
                }
            }

            return results;
        }

        private Func<JToken, JToken, JToken> GetOperatorFunc()
        {
            switch (_operator)
            {
                case '+':
                    return Add;
                case '-':
                    return Subtract;
                case '*':
                    return Multiply;
                case '/':
                    return Divide;
                case '%':
                    return Modulo;

                default:
                    Debug.Fail("Unexpected math operator: " + _operator);
                    return null;
            }
        }

        private static JToken Add(JToken lhs, JToken rhs)
        {
            if (lhs.Type == JTokenType.Null)
            {
                return rhs;
            }
            else if (rhs.Type == JTokenType.Null)
            {
                return lhs;
            }
            else if (lhs is JObject lobj && rhs is JObject robj)
            {
                robj.Merge(lobj);
                return robj;
            }
            else if (lhs is JArray larray && rhs is JArray rarray)
            {
                return new JArray(larray.Select(item => item).Concat(rarray.Select(item => item)));
            }
            else if (lhs is JValue lval && rhs is JValue rval)
            {
                if (lval.Type == JTokenType.String && rval.Type == JTokenType.String)
                {
                    return new JValue(lval.Value<string>() + rval.Value<string>());
                }
                else if (lval.Type == JTokenType.Integer && rval.Type == JTokenType.Integer)
                {
                    return new JValue(lval.Value<int>() + rval.Value<int>());
                }
                else if (lval.Type == JTokenType.Float && rval.Type == JTokenType.Float)
                {
                    return new JValue(lval.Value<double>() + rval.Value<double>());
                }
                else if (lval.Type == JTokenType.Integer && rval.Type == JTokenType.Float)
                {
                    return new JValue(lval.Value<double>() + rval.Value<double>());
                }
                else if (lval.Type == JTokenType.Float && rval.Type == JTokenType.Integer)
                {
                    return new JValue(lval.Value<double>() + rval.Value<double>());
                }
            }

            throw new InvalidOperationException($"Unable to add {lhs.Type} and {rhs.Type}.");
        }

        private static JToken Subtract(JToken lhs, JToken rhs)
        {
            if (lhs is JArray larray && rhs is JArray rarray)
            {
                foreach (var value in rarray.Select(item => item))
                {
                    larray.Remove(value);
                }

                return larray;
            }
            else if (lhs is JValue lval && rhs is JValue rval)
            {
                if (lval.Type == JTokenType.Integer && rval.Type == JTokenType.Integer)
                {
                    return new JValue(lval.Value<int>() - rval.Value<int>());
                }
                else if (lval.Type == JTokenType.Float && rval.Type == JTokenType.Float)
                {
                    return new JValue(lval.Value<double>() - rval.Value<double>());
                }
                else if (lval.Type == JTokenType.Integer && rval.Type == JTokenType.Float)
                {
                    return new JValue(lval.Value<double>() - rval.Value<double>());
                }
                else if (lval.Type == JTokenType.Float && rval.Type == JTokenType.Integer)
                {
                    return new JValue(lval.Value<double>() - rval.Value<double>());
                }
            }

            throw new InvalidOperationException($"Unable to subtract {lhs.Type} and {rhs.Type}.");
        }

        private static JToken Multiply(JToken lhs, JToken rhs)
        {
            if (lhs is JValue lval && rhs is JValue rval)
            {
                if (lval.Type == JTokenType.String && rval.Type == JTokenType.Integer)
                {
                    return new JValue(string.Concat(Enumerable.Repeat(lval.Value<string>(), rval.Value<int>())));
                }
                else if (lval.Type == JTokenType.Integer && rval.Type == JTokenType.String)
                {
                    return new JValue(string.Concat(Enumerable.Repeat(rval.Value<string>(), lval.Value<int>())));
                }
                else if (lval.Type == JTokenType.Integer && rval.Type == JTokenType.Integer)
                {
                    return new JValue(lval.Value<int>() * rval.Value<int>());
                }
                else if (lval.Type == JTokenType.Float && rval.Type == JTokenType.Float)
                {
                    return new JValue(lval.Value<double>() * rval.Value<double>());
                }
                else if (lval.Type == JTokenType.Integer && rval.Type == JTokenType.Float)
                {
                    return new JValue(lval.Value<double>() * rval.Value<double>());
                }
                else if (lval.Type == JTokenType.Float && rval.Type == JTokenType.Integer)
                {
                    return new JValue(lval.Value<double>() * rval.Value<double>());
                }
            }

            throw new InvalidOperationException($"Unable to multiply {lhs.Type} and {rhs.Type}.");
        }

        private static JToken Divide(JToken lhs, JToken rhs)
        {
            if (lhs is JValue lval && rhs is JValue rval)
            {
                if (lval.Type == JTokenType.String && rval.Type == JTokenType.String)
                {
                    return new JValue(lval.Value<string>().Split(rval.Value<string>()));
                }
                else if (lval.Type == JTokenType.Integer && rval.Type == JTokenType.Integer)
                {
                    return new JValue(lval.Value<int>() / rval.Value<int>());
                }
                else if (lval.Type == JTokenType.Float && rval.Type == JTokenType.Float)
                {
                    return new JValue(lval.Value<double>() / rval.Value<double>());
                }
                else if (lval.Type == JTokenType.Integer && rval.Type == JTokenType.Float)
                {
                    return new JValue(lval.Value<double>() / rval.Value<double>());
                }
                else if (lval.Type == JTokenType.Float && rval.Type == JTokenType.Integer)
                {
                    return new JValue(lval.Value<double>() / rval.Value<double>());
                }
            }

            throw new InvalidOperationException($"Unable to divide {lhs.Type} and {rhs.Type}.");
        }

        private static JToken Modulo(JToken lhs, JToken rhs)
        {
            if (lhs is JValue lval && rhs is JValue rval)
            {
                if (lval.Type == JTokenType.Integer && rval.Type == JTokenType.Integer)
                {
                    return new JValue(lval.Value<int>() % rval.Value<int>());
                }
                else if (lval.Type == JTokenType.Float && rval.Type == JTokenType.Float)
                {
                    return new JValue(lval.Value<double>() % rval.Value<double>());
                }
                else if (lval.Type == JTokenType.Integer && rval.Type == JTokenType.Float)
                {
                    return new JValue(lval.Value<double>() % rval.Value<double>());
                }
                else if (lval.Type == JTokenType.Float && rval.Type == JTokenType.Integer)
                {
                    return new JValue(lval.Value<double>() % rval.Value<double>());
                }
            }

            throw new InvalidOperationException($"Unable to mod {lhs.Type} and {rhs.Type}.");
        }
    }
}
