using System;
using System.Collections.Immutable;
using System.Linq;
using ITMO.SymbolicComputations.Base.Visitors;

namespace ITMO.SymbolicComputations.Base.Models {
    public sealed class Expression : Symbol, IEquatable<Expression> {
        public Expression(Symbol head, ImmutableList<Symbol> arguments) {
            EvaluationBudget.Current?.CheckCollectionSize(arguments.Count);
            Head = head;
            Arguments = arguments;
        }

        public Symbol Head { get; }
        public ImmutableList<Symbol> Arguments { get; }

        public bool Equals(Expression other) {
            if (ReferenceEquals(null, other)) {
                return false;
            }

            if (ReferenceEquals(this, other)) {
                return true;
            }

            return Head.Equals(other.Head) && Arguments.SequenceEqual(other.Arguments);
        }

        protected override T VisitImplementation<T>(ISymbolVisitor<T> visitor) =>
            visitor.VisitExpression(this);

        public override bool Equals(object obj) =>
            ReferenceEquals(this, obj) || obj is Expression other && Equals(other);

        public override int GetHashCode() {
            unchecked {
                var hash = Head != null ? Head.GetHashCode() : 0;
                foreach (var argument in Arguments) {
                    hash = (hash * 397) ^ (argument?.GetHashCode() ?? 0);
                }
                return hash;
            }
        }

        public static bool operator ==(Expression left, Expression right) => Equals(left, right);

        public static bool operator !=(Expression left, Expression right) => !Equals(left, right);

        public override string ToString() => Visit(new MathematicaPrinter());
    }
}
