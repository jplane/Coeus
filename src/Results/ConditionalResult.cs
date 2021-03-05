using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Coeus.Results
{
    public class ConditionalResult : ParserResult
    {
        private readonly string _operator;
        private readonly ParserResult _lhs;
        private readonly ParserResult _rhs;

        public ConditionalResult(string op, ParserResult lhs, ParserResult rhs)
        {
            _operator = op;
            _lhs = lhs;
            _rhs = rhs;
        }

        public override IEnumerable<JToken> Collect(JToken token)
        {
            Func<JToken, JToken, bool> op = GetOperatorFunc();

            var lhsTokens = _lhs.Collect(token);

            var rhsTokens = _rhs.Collect(token);

            var result = true;

            foreach (var lhs in lhsTokens)
            {
                foreach (var rhs in rhsTokens)
                {
                    result &= op(lhs, rhs);
                }
            }

            yield return new JValue(result);
        }

        private Func<JToken, JToken, bool> GetOperatorFunc()
        {
            switch (_operator)
            {
                case "==":
                    return Equal;
                case "!=":
                    return NotEqual;
                case ">":
                    return GreaterThan;
                case ">=":
                    return GreaterThanOrEqual;
                case "<":
                    return LessThan;
                case "<=":
                    return LessThanOrEqual;
                case "and":
                    return And;
                case "or":
                    return Or;

                default:
                    Debug.Fail("Unexpected math operator: " + _operator);
                    return null;
            }
        }

        private class JTokenComparer : IEqualityComparer<JToken>
        {
            public bool Equals(JToken x, JToken y)
            {
                return Equal(x, y);
            }

            public int GetHashCode(JToken obj)
            {
                return obj.GetHashCode();
            }
        }

        private static bool And(JToken lhs, JToken rhs)
        {
            if (lhs.Type == JTokenType.Null || rhs.Type == JTokenType.Null)
            {
                return false;
            }
            else if (lhs.Type == JTokenType.Boolean && rhs.Type == JTokenType.Boolean)
            {
                return lhs.Value<bool>() && rhs.Value<bool>();
            }
            else
            {
                return true;
            }
        }

        private static bool Or(JToken lhs, JToken rhs)
        {
            if (lhs.Type == JTokenType.Null)
            {
                return rhs.Type != JTokenType.Null;
            }
            else if (rhs.Type == JTokenType.Null)
            {
                return lhs.Type != JTokenType.Null;
            }
            else if (lhs.Type == JTokenType.Boolean && rhs.Type == JTokenType.Boolean)
            {
                return lhs.Value<bool>() || rhs.Value<bool>();
            }
            else
            {
                return true;
            }
        }

        private static bool Equal(JToken lhs, JToken rhs)
        {
            if (lhs.Type != rhs.Type)
            {
                return false;
            }

            if (lhs.Type == JTokenType.Null)
            {
                return true;
            }
            else if (lhs is JObject lobj)
            {
                var props = lobj.Properties().ToArray();

                var robj = (JObject)rhs;

                if (robj.Properties().Count() != props.Length)
                {
                    return false;
                }

                foreach (var lhsProp in props)
                {
                    var rhsProp = robj.Property(lhsProp.Name);

                    if (rhsProp == null)
                    {
                        return false;
                    }

                    if (!Equal(lhsProp.Value, rhsProp.Value))
                    {
                        return false;
                    }
                }

                return true;
            }
            else if (lhs is JArray larray)
            {
                var rarray = (JArray)rhs;

                return larray.Select(item => item).SequenceEqual(rarray.Select(item => item), new JTokenComparer());
            }
            else if (lhs is JValue lval && rhs is JValue rval)
            {
                if (lval.Type == JTokenType.String)
                {
                    return lval.Value<string>() == rval.Value<string>();
                }
                else if (lval.Type == JTokenType.Integer && rval.Type == JTokenType.Integer)
                {
                    return lval.Value<int>() == rval.Value<int>();
                }
                else if (lval.Type == JTokenType.Float || rval.Type == JTokenType.Float)
                {
                    return lval.Value<double>() == rval.Value<double>();
                }
            }

            throw new InvalidOperationException($"Unable to equals compare {lhs.Type} and {rhs.Type}.");
        }

        private static bool NotEqual(JToken lhs, JToken rhs)
        {
            return !Equal(lhs, rhs);
        }

        private static bool GreaterThan(JToken lhs, JToken rhs)
        {
            if (lhs is JValue lval && rhs is JValue rval)
            {
                if (lval.Type == JTokenType.Integer && rval.Type == JTokenType.Integer)
                {
                    return lval.Value<int>() > rval.Value<int>();
                }
                else if (lval.Type == JTokenType.Float || rval.Type == JTokenType.Float)
                {
                    return lval.Value<double>() > rval.Value<double>();
                }
            }

            throw new InvalidOperationException($"Unable to greater than compare {lhs.Type} and {rhs.Type}.");
        }

        private static bool GreaterThanOrEqual(JToken lhs, JToken rhs)
        {
            if (lhs is JValue lval && rhs is JValue rval)
            {
                if (lval.Type == JTokenType.Integer && rval.Type == JTokenType.Integer)
                {
                    return lval.Value<int>() >= rval.Value<int>();
                }
                else if (lval.Type == JTokenType.Float || rval.Type == JTokenType.Float)
                {
                    return lval.Value<double>() >= rval.Value<double>();
                }
            }

            throw new InvalidOperationException($"Unable to greater than or equal compare {lhs.Type} and {rhs.Type}.");
        }

        private static bool LessThan(JToken lhs, JToken rhs)
        {
            if (lhs is JValue lval && rhs is JValue rval)
            {
                if (lval.Type == JTokenType.Integer && rval.Type == JTokenType.Integer)
                {
                    return lval.Value<int>() < rval.Value<int>();
                }
                else if (lval.Type == JTokenType.Float || rval.Type == JTokenType.Float)
                {
                    return lval.Value<double>() < rval.Value<double>();
                }
            }

            throw new InvalidOperationException($"Unable to less than compare {lhs.Type} and {rhs.Type}.");
        }

        private static bool LessThanOrEqual(JToken lhs, JToken rhs)
        {
            if (lhs is JValue lval && rhs is JValue rval)
            {
                if (lval.Type == JTokenType.Integer && rval.Type == JTokenType.Integer)
                {
                    return lval.Value<int>() <= rval.Value<int>();
                }
                else if (lval.Type == JTokenType.Float || rval.Type == JTokenType.Float)
                {
                    return lval.Value<double>() <= rval.Value<double>();
                }
            }

            throw new InvalidOperationException($"Unable to less than or equal compare {lhs.Type} and {rhs.Type}.");
        }
    }
}
