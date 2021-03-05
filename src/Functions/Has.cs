using Coeus.Results;
using Newtonsoft.Json.Linq;
using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coeus.Functions
{
    internal static partial class Funcs
    {
        public static Parser<ParserResult> Has =>
            from start in Parse.String("has(").Token()
            from key in Parse.CharExcept(")").Many().Token().Text()
            from end in Parse.String(")").Token()
            select new FunctionResult(token =>
            {
                bool result;

                if (token.Type == JTokenType.Object)
                {
                    result = ((JObject)token).ContainsKey(key.Trim('"'));
                }
                else if (token.Type == JTokenType.Array && int.TryParse(key, out int idx))
                {
                    result = idx >= 0 && idx < token.Count();
                }
                else
                {
                    throw new InvalidOperationException($"Unable to invoke Has() on token {token.Type} and key {key}.");
                }

                return new[] { new JValue(result) };
            });
    }
}
