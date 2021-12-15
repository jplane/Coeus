using Coeus.Results;
using Coeus.Functions;
using Newtonsoft.Json.Linq;
using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coeus
{
    public static partial class JQ
    {
        private static Parser<ParserResult> Function =>
                Funcs.Length
                    .Or(Funcs.Not)
                    .Or(Funcs.Keys)
                    .Or(Funcs.Has)
                    .Or(Funcs.Select);
    }
}
