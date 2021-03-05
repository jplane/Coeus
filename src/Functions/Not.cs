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
        public static Parser<ParserResult> Not =>
            Parse.String("not").Select(_ => new FunctionResult(token =>
            {
                bool result;

            if (token.Type == JTokenType.Null)
            {
                result = true;
            }
            else if (token.Type == JTokenType.Boolean)
            {
                result = !token.Value<bool>();
            }
            else
            {
                result = false;
            }

            return new[] { new JValue(result) };
        }));
    }
}
