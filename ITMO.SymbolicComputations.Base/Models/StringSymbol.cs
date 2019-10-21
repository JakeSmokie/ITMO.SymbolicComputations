using System;
using System.Collections.Immutable;
using ITMO.SymbolicComputations.Base.Visitors;
using Newtonsoft.Json;

namespace ITMO.SymbolicComputations.Base.Models {
    public sealed class StringSymbol : Symbol, IEquatable<StringSymbol>, IComparable<StringSymbol>, IComparable {
        public StringSymbol(string name, params StringSymbol[] attributes) {
            Name = name;
            Attributes = attributes.ToImmutableSortedSet();
        }

        public string Name { get; }
        public override string ToString() => Name;

        [JsonIgnore]
        public ImmutableSortedSet<StringSymbol> Attributes { get; }

        public int CompareTo(object obj) {
            if (ReferenceEquals(null, obj)) {
                return 1;
            }

            if (ReferenceEquals(this, obj)) {
                return 0;
            }

            return obj is StringSymbol other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(StringSymbol)}");
        }

        public int CompareTo(StringSymbol other) {
            if (ReferenceEquals(this, other)) {
                return 0;
            }

            if (ReferenceEquals(null, other)) {
                return 1;
            }

            return string.Compare(Name, other.Name, StringComparison.InvariantCulture);
        }

        public bool Equals(StringSymbol other) {
            if (ReferenceEquals(null, other)) {
                return false;
            }

            if (ReferenceEquals(this, other)) {
                return true;
            }

            return Name == other.Name;
        }

        protected override T VisitImplementation<T>(ISymbolVisitor<T> visitor) =>
            visitor.VisitSymbol(this);

        public override bool Equals(object obj) => ReferenceEquals(this, obj) || obj is StringSymbol other && Equals(other);

        public override int GetHashCode() {
            return Name != null ? Name.GetHashCode() : 0;
        }

        public static bool operator ==(StringSymbol left, StringSymbol right) => Equals(left, right);

        public static bool operator !=(StringSymbol left, StringSymbol right) => !Equals(left, right);
    }
}