using Coeus.Results;
using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coeus
{
    public static partial class JQ
    {
        private static Parser<ParserResult> LhsAssignmentPipe =>
            Parse.ChainOperator(Parse.String("|").Token().Text(),
                                LhsAssignmentComma,
                                (_, lhs, rhs) => new LhsAssignmentPipeResult(lhs, rhs));

        private static Parser<ParserResult> LhsAssignmentComma =>
            Parse.ChainOperator(Parse.String(",").Token().Text(),
                                LhsAssignmentExpression.Or(Identity),
                                (_, lhs, rhs) => new LhsAssignmentCommaResult(lhs, rhs));

        private static Parser<ParserResult> LhsAssignmentExpression =>
            from first in DottedPropertyOperator(true)
                                    .Or(DottedOperator)
                                    .Once()
            from rest in Operator.Or(DottedPropertyOperator(true)).Many()
                                 .Or(Parse.Ref(() => LhsAssignmentPipe).Once())
            select new LhsAssignmentResult(first.Concat(rest));

        private static Parser<ParserResult> UpdateAssignment =>
            from lhs in LhsAssignmentPipe
                            .Or(LhsAssignmentPipe.Contained(Parse.String("(").Token(), Parse.String(")").Token()))
            from op in Parse.String("|=")
                                     .Or(Parse.String("+="))
                                     .Or(Parse.String("-="))
                                     .Or(Parse.String("*="))
                                     .Or(Parse.String("/=")).Token().Text()
            from rhs in Parse.Ref(() => Pipe)
            select new UpdateAssignmentResult(op, lhs, rhs);

        private static Parser<ParserResult> Assignment =>
            from lhs in LhsAssignmentPipe
                            .Or(LhsAssignmentPipe.Contained(Parse.String("(").Token(), Parse.String(")").Token()))
            from _ in Parse.String("=").Token().Text()
            from rhs in Parse.Ref(() => Pipe)
            select new AssignmentResult(lhs, rhs);

    }
}
