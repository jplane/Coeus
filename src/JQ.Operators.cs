using Coeus.Results;
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
        private static Parser<ParserResult> DottedPropertyOperator(bool assignment) =>
            Parse.String(".").Then(_ => PropertyOperator(assignment));

        private static Parser<ParserResult> PropertyOperator(bool assignment) =>
                from prop in Property(assignment).Or(SpecialNameProperty(assignment)).Or(IndexProperty(assignment))
                from optional in Parse.String("?").Optional()
                select optional.IsEmpty ? prop : new HideErrorResult(prop);

        private static Parser<ParserResult> DottedOperator => Parse.String(".").Then(_ => Operator);

        private static Parser<ParserResult> Operator =>
                ArrayIndex
                    .Or(Slice)
                    .Or(SliceImplicitStart)
                    .Or(SliceImplicitEnd)
                    .Or(Iterator);

        private static Parser<ParserResult> AddOrSubtract =>
            Parse.ChainOperator(Parse.Chars('+', '-').Token(),
                                MultiplyOrDivideOrModulo,
                                (op, lhs, rhs) => new MathResult(op, lhs, rhs));

        private static Parser<ParserResult> MultiplyOrDivideOrModulo =>
            Parse.ChainOperator(Parse.Chars('*', '/', '%').Token(),
                                Conditional,
                                (op, lhs, rhs) => new MathResult(op, lhs, rhs));

        private static Parser<ParserResult> Conditional =>
            Parse.ChainOperator(Parse.String("==")
                                     .Or(Parse.String("!="))
                                     .Or(Parse.String(">"))
                                     .Or(Parse.String(">="))
                                     .Or(Parse.String("<"))
                                     .Or(Parse.String("<="))
                                     .Or(Parse.String("and"))
                                     .Or(Parse.String("or")).Token().Text(),
                                Expression,
                                (op, lhs, rhs) => new ConditionalResult(op, lhs, rhs));

        private static Parser<ParserResult> RecursiveDescent =>
                from _ in Parse.String("..").Token()
                select new RecursiveDescentResult();

        private static Parser<ParserResult> Identity =>
                from _ in Parse.String(".").Token()
                select new AtomicResult(token => token);

        private static Parser<ParserResult> Property(bool assignment) =>
                from first in Parse.Letter.Once().Or(Parse.Char('_').Once()).Text()
                from rest in Parse.LetterOrDigit.Or(Parse.Char('_')).Many().Text()
                select (ParserResult)(assignment ? new LhsPropertyAssignmentResult(first + rest) : new PropertyResult(first + rest));

        private static Parser<ParserResult> IndexProperty(bool assignment) =>
                from startBracket in Parse.Char('[').Token().Once()
                from startQuote in Parse.Char('"').Token().Once()
                from identifier in Parse.CharExcept('"').Many().Text()
                from endQuote in Parse.Char('"').Token().Once()
                from endBracket in Parse.Char(']').Token().Once()
                select (ParserResult)(assignment ? new LhsPropertyAssignmentResult(identifier) : new PropertyResult(identifier));

        private static Parser<ParserResult> SpecialNameProperty(bool assigment) =>
                from startQuote in Parse.Char('"').Token().Once()
                from identifier in Parse.CharExcept('"').Many().Text()
                from endQuote in Parse.Char('"').Token().Once()
                select (ParserResult)(assigment ? new LhsPropertyAssignmentResult(identifier) : new PropertyResult(identifier));

        private static Parser<ParserResult> Iterator =>
                from brackets in Parse.String("[]")
                select new IteratorResult();

        private static Parser<ParserResult> ArrayIndex =>
                from startBracket in Parse.Char('[').Token().Once()
                from index in ParseInteger
                from endBracket in Parse.Char(']').Token().Once()
                select new AtomicResult(obj =>
                {
                    if (obj is JArray array)
                    {
                        var idx = int.Parse(index);

                        if (idx >= 0)
                        {
                            return array[idx];
                        }
                        else
                        {
                            return array.Reverse().ElementAt((idx * -1) - 1);
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("Expecting JArray.");
                    }
                });

        private static Parser<string> ParseInteger =>
            from first in Parse.Digit.Or(Parse.Char('-')).Once().Text()
            from rest in Parse.Digit.Many().Text()
            select first + rest;

        private static Parser<ParserResult> Slice =>
                from startBracket in Parse.Char('[').Token().Once()
                from inclusiveStartIndex in ParseInteger
                from colon in Parse.Char(':').Token().Once()
                from exclusiveEndIndex in ParseInteger
                from endBracket in Parse.Char(']').Token().Once()
                select new AtomicResult(obj => _Slice(obj, inclusiveStartIndex, exclusiveEndIndex));

        private static Parser<ParserResult> SliceImplicitStart =>
                from startBracket in Parse.Char('[').Token().Once()
                from colon in Parse.Char(':').Token().Once()
                from exclusiveEndIndex in ParseInteger
                from endBracket in Parse.Char(']').Token().Once()
                select new AtomicResult(obj => _Slice(obj, null, exclusiveEndIndex));

        private static Parser<ParserResult> SliceImplicitEnd =>
                from startBracket in Parse.Char('[').Token().Once()
                from inclusiveStartIndex in ParseInteger
                from colon in Parse.Char(':').Token().Once()
                from endBracket in Parse.Char(']').Token().Once()
                select new AtomicResult(obj => _Slice(obj, inclusiveStartIndex, null));

        private static JToken _Slice(JToken obj, string inclusiveStartIndex, string exclusiveEndIndex)
        {
            void getBounds(int length, out int start, out int end)
            {
                start = int.Parse(string.IsNullOrWhiteSpace(inclusiveStartIndex) ? "0" : inclusiveStartIndex);
                end = int.Parse(string.IsNullOrWhiteSpace(exclusiveEndIndex) ? (length - 1).ToString() : exclusiveEndIndex);

                if (start < 0)
                {
                    start = length + start;
                }

                if (end < 0)
                {
                    end = length + end - 1;
                }
            }

            if (obj is JArray array)
            {
                getBounds(array.Count, out int startIdx, out int endIdx);

                return new JArray(array.Skip(startIdx).Take(endIdx - startIdx + 1));
            }
            else if (obj is JValue v && v.Type == JTokenType.String)
            {
                var s = v.Value<string>();

                getBounds(s.Length, out int startIdx, out int endIdx);

                return s[startIdx..endIdx];
            }
            else
            {
                throw new InvalidOperationException("Expecting JArray.");
            }
        }
    }
}
