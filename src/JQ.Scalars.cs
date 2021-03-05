using Coeus.Results;
using Newtonsoft.Json.Linq;
using Sprache;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coeus
{
    public static partial class JQ
    {
        private static Parser<ParserResult> Scalar => String.Or(Float).Or(Int).Or(Bool).Token();

        private static Parser<ParserResult> String =>
                from startQuote in Parse.Char('"').Token().Once()
                from identifier in Parse.CharExcept('"').Many().Text()
                from endQuote in Parse.Char('"').Token().Once()
                select new AtomicResult(_ => new JValue(identifier));

        private static Parser<ParserResult> Float =>
                FloatLeadingDecimal
                    .Or(NegativeFloatLeadingDecimal)
                    .Or(FloatNonLeadingDecimal);

        private static Parser<ParserResult> FloatNonLeadingDecimal =>
                from first in Parse.Digit.Or(Parse.Char('-')).Once().Text()
                from beforeDecimal in Parse.Digit.Many().Text()
                from dec in Parse.Char('.').Once().Text()
                from afterDecimal in Parse.Digit.Many().Text()
                select new AtomicResult(_ => new JValue(double.Parse(first + beforeDecimal + dec + afterDecimal)));

        private static Parser<ParserResult> FloatLeadingDecimal =>
                from first in Parse.String(".").Text()
                from rest in Parse.Digit.Many().Text()
                select new AtomicResult(_ => new JValue(double.Parse(first + rest)));

        private static Parser<ParserResult> NegativeFloatLeadingDecimal =>
                from first in Parse.String("-.").Text()
                from rest in Parse.Digit.Many().Text()
                select new AtomicResult(_ => new JValue(double.Parse(first + rest)));

        private static Parser<ParserResult> Int =>
                from value in ParseInteger
                select new AtomicResult(_ => new JValue(int.Parse(value)));

        private static Parser<ParserResult> Bool =>
                from value in Parse.String("true").Or(Parse.String("false")).Text()
                select new AtomicResult(_ => new JValue(bool.Parse(value)));
    }
}
