using Newtonsoft.Json.Linq;
using Sprache;
using System.Collections.Generic;
using System.Linq;

namespace Coeus.Results
{
    public class IfThenResult : ParserResult
    {
        private readonly ParserResult _condition;
        private readonly ParserResult _branch;
        private readonly IOption<IEnumerable<(ParserResult Condition, ParserResult Branch)>> _elifBranches;
        private readonly IOption<ParserResult> _elseBranch;

        public IfThenResult(ParserResult condition,
                            ParserResult branch,
                            IOption<IEnumerable<(ParserResult, ParserResult)>> elifBranches,
                            IOption<ParserResult> elseBranch)
        {
            _condition = condition;
            _branch = branch;
            _elifBranches = elifBranches;
            _elseBranch = elseBranch;
        }

        public override IEnumerable<JToken> Collect(JToken token)
        {
            var results = new List<JToken>();

            IEnumerable<JToken> ProcessBranch(JToken target, ParserResult condition, ParserResult branch)
            {
                var conditionResults = condition.Collect(target);

                var falseResults = new List<JToken>();

                foreach (var conditionResult in conditionResults)
                {
                    if ((conditionResult.Type == JTokenType.Boolean && conditionResult.Value<bool>()) ||
                        (conditionResult.Type != JTokenType.Boolean && conditionResult.Type != JTokenType.Null))
                    {
                        results.AddRange(branch.Collect(token));
                    }
                    else
                    {
                        falseResults.Add(token);
                    }
                }

                return falseResults;
            }

            var fallthru = ProcessBranch(token, _condition, _branch);

            if (_elifBranches.IsDefined)
            {
                var branches = _elifBranches.Get();

                foreach (var branch in branches)
                {
                    var innerFallThru = new List<JToken>();

                    foreach (var target in fallthru)
                    {
                        innerFallThru.AddRange(ProcessBranch(target, branch.Condition, branch.Branch));
                    }

                    fallthru = innerFallThru;
                }
            }

            ParserResult elseBranch = new AtomicResult(token => token);

            if (_elseBranch.IsDefined)
            {
                elseBranch = _elseBranch.Get();
            }

            foreach (var target in fallthru)
            {
                results.AddRange(elseBranch.Collect(target));
            }

            return results;
        }
    }
}
