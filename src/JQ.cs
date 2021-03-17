using Coeus.Results;
using Newtonsoft.Json.Linq;
using Sprache;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Coeus
{
    public static partial class JQ
    {
        private static readonly ConcurrentDictionary<string, ParserResult> _evaluators =
            new ConcurrentDictionary<string, ParserResult>();

        public static JToken EvalToToken(this string jq, JToken token)
        {
            var results = Eval(jq, token).ToArray();

            if (results.Length == 1)
            {
                return results[0];
            }
            else
            {
                return new JArray(results);
            }
        }

        public static IEnumerable<JToken> Eval(this string jq, JToken token)
        {
            var evaluator = _evaluators.GetOrAdd(jq, _ => Pipe.End().Parse(jq));

            Debug.Assert(evaluator != null);

            return evaluator.Collect(token).ToArray();
        }

        private static Parser<ParserResult> ComplexExpression =>
                from first in UpdateAssignment
                                .Or(Assignment)
                                .Or(DottedPropertyOperator(false))
                                .Or(DottedOperator)
                                .Or(Expression)
                                .Once()
                from rest in Operator.Or(DottedPropertyOperator(false)).Many()
                select new PipeResult(first.Concat(rest));

        private static Parser<ParserResult> Expression =>
             ArrayCtor
                .Or(ObjCtor)
                .Or(IfThen)
                .Or(RecursiveDescent)
                .Or(Identity)
                .Or(Null)
                .Or(Function)
                .Or(Scalar)
                .Or(Parse.Ref(() => Pipe).Contained(Parse.String("(").Token(), Parse.String(")").Token()));

        private static Parser<ParserResult> Null =>
                Parse.String("null").Token().Select(_ => new AtomicResult(token => JValue.CreateNull()));

        private static Parser<ParserResult> Pipe =>
            Parse.ChainOperator(Parse.String("|").Token().Text(),
                                Comma,
                                (_, lhs, rhs) => new PipeResult(lhs, rhs));

        private static Parser<ParserResult> Comma =>
            Parse.ChainOperator(Parse.String(",").Token().Text(),
                                AddOrSubtract,
                                (_, lhs, rhs) => new CommaResult(lhs, rhs));

        private static Parser<ParserResult> IfThen =>
                from ifkeyword in Parse.String("if").Token()
                from condition in Parse.Ref(() => Pipe)
                from thenkeyword in Parse.String("then").Token()
                from branch in Parse.Ref(() => Pipe)
                from elifBranches in Elif.Many().Optional()
                from elseBranch in Else.Optional()
                from endkeyword in Parse.String("end").Token()
                select new IfThenResult(condition, branch, elifBranches, elseBranch);

        private static Parser<(ParserResult, ParserResult)> Elif =>
                from elifkeyword in Parse.String("elif").Token()
                from condition in Parse.Ref(() => Pipe)
                from thenkeyword in Parse.String("then").Token()
                from branch in Parse.Ref(() => Pipe)
                select (condition, branch);

        private static Parser<ParserResult> Else =>
                from elsekeyword in Parse.String("else").Token()
                from branch in Parse.Ref(() => Pipe)
                select branch;

        private static Parser<ParserResult> ObjCtor =>
                from props in PropDefs.Contained(Parse.String("{").Token(), Parse.String("}").Token()).Or(EmptyObjCtor)
                select new ObjectCtorResult(props);

        private static Parser<IEnumerable<Func<JToken, IEnumerable<JObject>, IEnumerable<JObject>>>> EmptyObjCtor =>
            Parse.String("{}").Token().Once().Return(Enumerable.Empty<Func<JToken, IEnumerable<JObject>, IEnumerable<JObject>>>());

        private static Parser<IEnumerable<Func<JToken, IEnumerable<JObject>, IEnumerable<JObject>>>> PropDefs =>
                QuotedPropDef.DelimitedBy(Parse.String(",").Token());

        private static Parser<Func<JToken, IEnumerable<JObject>, IEnumerable<JObject>>> QuotedPropDef =>
                from startQuote in Parse.Char('"').Token().Once()
                from identifier in Parse.CharExcept('"').Many().Text()
                from endQuote in Parse.Char('"').Token().Once()
                from colon in Parse.Char(':').Token().Once()
                from value in Parse.Ref(() => ComplexExpression)
                select (Func<JToken, IEnumerable<JObject>, IEnumerable<JObject>>)((token, incomingResults) =>
                {
                    var results = new List<JObject>();

                    foreach (var val in value.Collect(token))
                    {
                        foreach (var incoming in incomingResults)
                        {
                            var copy = (JObject) incoming.DeepClone();
                            copy[identifier] = val;
                            results.Add(copy);
                        } 
                    }

                    return results;
                });

        private static Parser<ParserResult> ArrayCtor =>
                from value in EmptyArrayCtor.Or(Parse.Ref(() => Pipe).Contained(Parse.String("[").Token(), Parse.String("]").Token()))
                select new ArrayCtorResult(value);

        private static Parser<ParserResult> EmptyArrayCtor =>
                from _ in Parse.String("[]").Token()
                select (ParserResult) null;
    }
}
