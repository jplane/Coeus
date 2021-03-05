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
        public static Parser<ParserResult> Keys =>
            Parse.String("keys").Select(_ => new FunctionResult(token =>
            {
                if (token.Type == JTokenType.Object)
            {
                return ((JObject)token).Properties().Select(prop => prop.Name)
                                                    .OrderBy(name => name)
                                                    .Select(name => new JValue(name));
            }
            else if (token.Type == JTokenType.Array)
            {
                return Enumerable.Range(0, token.Count()).Select(idx => new JValue(idx));
            }

            throw new InvalidOperationException("Unable to invoke Keys() on token: " + token.Type.ToString());
        }));
    }
}
