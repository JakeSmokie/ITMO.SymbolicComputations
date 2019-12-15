using System;
using System.Collections.Immutable;
using System.Linq;
using System.Xml;
using ITMO.SymbolicComputations.Base;
using ITMO.SymbolicComputations.Base.Tools;
using ITMO.SymbolicComputations.Base.Visitors;
using ITMO.SymbolicComputations.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ITMO.SymbolicComputations.Web.Controllers {
    [ApiController]
    [Route("api")]
    public class SymbolicController : ControllerBase {
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

                var symbol = doc.AsExpressionInfo().Symbol;
                var mathematica = symbol.Visit(MathematicaPrinter.Default);

                var (steps, result) = new SymbolicContext().Run(symbol);

                return new ComputationResponse {
                    RawInput = mathematica,
                    Steps = steps.WithoutDuplicates().Select(x => x.Visit(MathematicaPrinter.Default)),
                    Result = result.Visit(MathematicaPrinter.Default),
                    X = ImmutableArray<decimal>.Empty,
                    Y = ImmutableArray<decimal>.Empty,
                    Sliders = ImmutableArray<ComputationResponse.Slider>.Empty
                };
            }
            catch (Exception e) {
                logger.LogError("{error}:\n{stack}", e.Message, e.StackTrace);
                return BadRequest(e.Message);
            }
        }
    }
}