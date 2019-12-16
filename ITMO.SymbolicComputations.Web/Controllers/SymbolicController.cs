using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Xml;
using ITMO.SymbolicComputations.Base;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Tools;
using ITMO.SymbolicComputations.Base.Visitors;
using ITMO.SymbolicComputations.Base.Visitors.Casting;
using ITMO.SymbolicComputations.Base.Visitors.Evaluation;
using ITMO.SymbolicComputations.Web.Models;
using ITMO.SymbolicComputations.Web.Visitors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ITMO.SymbolicComputations.Web.Controllers {
    [ApiController]
    [Route("api")]
    public class SymbolicController : ControllerBase {
        private static readonly List<StringSymbol> Symbols =
            typeof(SymbolicContext).Assembly.GetTypes()
                .SelectMany(x =>
                    x.GetFields().Where(x =>
                        x.FieldType == typeof(StringSymbol)
                        && x.IsStatic
                        && x.IsPublic
                    )
                )
                .Select(x => (StringSymbol) x.GetValue(null))
                .ToList();

        private readonly ILogger<SymbolicController> logger;

        public SymbolicController(
            ILogger<SymbolicController> logger
        ) {
            this.logger = logger;
        }

        [HttpPost("compute")]
        public ActionResult<ComputationResponse> Compute([FromBody] ComputationRequest request) {
            try {
                var doc = new XmlDocument();
                doc.LoadXml(request.XmlExpression);

                var symbol = ReplaceBuiltInStringSymbols(doc.AsExpressionInfo().Symbol);
                var (context, inputs) = symbol.Visit(new FormInputReader());

                var (steps, result) = new SymbolicContext(context).Run(symbol);
                
                var points = ListOfListToTuples((result as Expression)?.Arguments?.Last() as Expression)
                    .ToImmutableArray();

                return new ComputationResponse {
                    RawInput = symbol.Visit(MathematicaPrinter.Default),
                    Steps = steps.WithoutDuplicates().Select(x => x.Visit(MathematicaPrinter.Default)),
                    Result = result.Visit(MathematicaPrinter.Default),
                    //
                    Points = points,
                    FormInputs = inputs.Select(x => new ComputationResponse.FormInput {
                        Variable = x.Item1,
                        Default = x.Item2
                    }).ToImmutableArray()
                };
            }
            catch (Exception e) {
                logger.LogError("{error}:\n{stack}", e.Message, e.StackTrace);
                return BadRequest(e.Message);
            }
        }

        private static Symbol ReplaceBuiltInStringSymbols(Symbol symbol) =>
            Symbols.Aggregate(symbol, (acc, x) =>
                acc.Visit(new VariableReplacer(new StringSymbol(x.Name), x, true))
            );
        
        private static IEnumerable<ComputationResponse.Point> ListOfListToTuples(Expression expression) {
            if (expression?.Arguments == null) {
                yield break;
            }
            
            foreach (var argument in expression.Arguments) {
                var list = argument.Visit(AsExpressionVisitor.Instance);
                var first = list?.Arguments?[0]?.Visit(AsConstantVisitor.Instance);
                var second = list?.Arguments?[1]?.Visit(AsConstantVisitor.Instance);

                if (first == null || second == null) {
                    continue;
                }

                yield return new ComputationResponse.Point {
                    X = first.Value,
                    Y = second.Value
                };
            }
        }
    }
}