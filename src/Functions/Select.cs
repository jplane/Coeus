using Coeus.Results;
using Newtonsoft.Json.Linq;
using Sprache;
using System.Collections.Generic;

namespace Coeus.Functions
{
    internal static partial class Funcs
    {
        public static Parser<ParserResult> Select =>
            from start in Parse.String("select(").Token()
            from booleanExpression in Parse.CharExcept(")").Many().Token().Text()
            from end in Parse.String(")").Token()
            select new FunctionResult(token =>
            {
                var result = new List<JToken>();
                if (token.Type == JTokenType.Array)
                {
                    var jArr = new JArray();
                    foreach(var child in (token as JArray))
                    {
                        if (Check(booleanExpression, child))
                        {
                            jArr.Add(child);
                        }
                    }

                    result.Add(jArr);
                }
                else
                {
                    if (Check(booleanExpression, token))
                    {
                        result.Add(token);
                    }
                }

                return result;
            });

        private static bool Check(string booleanExpression, JToken token)
        {
            var i = booleanExpression.EvalToToken(new JArray
            {
                token
            });
            if (i != null && bool.TryParse(i.ToString(), out bool b) && b)
            {
                return true;
            }

            return false;
        }
    }
}
