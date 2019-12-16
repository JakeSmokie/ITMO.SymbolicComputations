using System.Collections.Generic;

namespace ITMO.SymbolicComputations.Web.Models {
    public class ComputationResponse {
        public class FormInput {
            public string Variable;
            public decimal Default;
        }
        
        public class Point {
            public decimal X;
            public decimal Y;
        }

        public string RawInput;
        public string RawOutput;

        public string Result;
        public IEnumerable<string> Steps;

        public IEnumerable<FormInput> FormInputs;
        public IEnumerable<Point> Points;
    }
}