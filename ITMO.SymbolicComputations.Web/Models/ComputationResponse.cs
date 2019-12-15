using System.Collections.Generic;
using System.Collections.Immutable;

namespace ITMO.SymbolicComputations.Web.Models {
    public class ComputationResponse {
        public class Slider {
            public string Variable;

            public decimal From;
            public decimal To;
        }

        public string RawInput;
        public string RawOutput;
        
        public ImmutableArray<decimal> X;
        public ImmutableArray<decimal> Y;
        public ImmutableArray<Slider> Sliders;
        public IEnumerable<string> Steps;
        public string Result;
    }
}