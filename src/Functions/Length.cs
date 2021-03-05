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
        public static Parser<ParserResult> Length =>
            Parse.String("length").Select(_ => new FunctionResult(token =>
        {
            int length;

            if (token.Type == JTokenType.String)
            {
                length = token.Value<string>().Length;
            }
            else if (token.Type == JTokenType.Array)
            {
                length = token.Count();
            }
            else if (token.Type == JTokenType.Object)
            {
                length = ((JObject)token).Properties().Count();
            }
            else if (token.Type == JTokenType.Null)
            {
                length = 0;
            }
            else
            {
                throw new InvalidOperationException("Unable to compute length for " + token.Type.ToString());
            }

            return new[] { new JValue(length) };
        }));
    }
}
