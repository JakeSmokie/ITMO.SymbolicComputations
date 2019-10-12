using System;
using System.Collections.Immutable;
using ITMO.SymbolicComputations.Base.Visitors;
using Newtonsoft.Json;

namespace ITMO.SymbolicComputations.Base.Models {
    public sealed class StringSymbol : Symbol, IEquatable<StringSymbol> {
        public StringSymbol(string name, ImmutableHashSet<StringSymbol>? attributes = null) {
            Name = name;
            Attributes = attributes ?? ImmutableHashSet<StringSymbol>.Empty;
        }

        public string Name { get; }

        [JsonIgnore]
        public ImmutableHashSet<StringSymbol> Attributes { get; }

        protected override T VisitImplementation<T>(ISymbolVisitor<T> visitor) =>
            visitor.VisitSymbol(this);

        public bool Equals(StringSymbol other) {
            if (ReferenceEquals(null, other)) {
                return false;
            }

            if (ReferenceEquals(this, other)) {
                return true;
            }

            return Name == other.Name && Equals(Attributes, other.Attributes);
        }

        public override bool Equals(object obj) => ReferenceEquals(this, obj) || obj is StringSymbol other && Equals(other);

        public override int GetHashCode() {
            unchecked {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (Attributes != null ? Attributes.GetHashCode() : 0);
            }
        }

        public static bool operator ==(StringSymbol left, StringSymbol right) => Equals(left, right);

        public static bool operator !=(StringSymbol left, StringSymbol right) => !Equals(left, right);
    }
}