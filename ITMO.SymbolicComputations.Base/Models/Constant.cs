using System;
using ITMO.SymbolicComputations.Base.Visitors;

namespace ITMO.SymbolicComputations.Base.Models {
    public sealed class Constant : Symbol, IEquatable<Constant> {
        public Constant(decimal value) =>
            Value = value;

        public decimal Value { get; }

        protected override T VisitImplementation<T>(ISymbolVisitor<T> visitor) =>
            visitor.VisitConstant(this);

        public bool Equals(Constant other) {
            if (ReferenceEquals(null, other)) {
                return false;
            }

            if (ReferenceEquals(this, other)) {
                return true;
            }

            return Value == other.Value;
        }

        public override bool Equals(object obj) => ReferenceEquals(this, obj) || obj is Constant other && Equals(other);

        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(Constant left, Constant right) => Equals(left, right);

        public static bool operator !=(Constant left, Constant right) => !Equals(left, right);
    }
}