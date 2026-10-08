using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.StandardLibrary;

namespace ITMO.SymbolicComputations.Base.Parsing {
    /// <summary>Resolves the canonical symbol instances, including their evaluation attributes.</summary>
    public static class BuiltInSymbols {
        private static readonly IReadOnlyDictionary<string, StringSymbol> Symbols = ReadSymbols();

        public static StringSymbol Resolve(string name) {
            if (string.IsNullOrWhiteSpace(name)) {
                throw new ArgumentException("Имя символа не должно быть пустым.", nameof(name));
            }

            return Symbols.TryGetValue(name, out var symbol) ? symbol : new StringSymbol(name);
        }

        private static IReadOnlyDictionary<string, StringSymbol> ReadSymbols() {
            var symbols = new Dictionary<string, StringSymbol>(StringComparer.Ordinal);
            var standardLibrary = typeof(ArithmeticFunctions);

            foreach (var type in standardLibrary.Assembly.GetTypes()
                         .Where(type => type.Namespace == standardLibrary.Namespace)
                         .OrderBy(type => type.FullName, StringComparer.Ordinal)) {
                foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Static)
                             .Where(field => field.FieldType == typeof(StringSymbol))) {
                    if (field.GetValue(null) is StringSymbol symbol && !string.IsNullOrEmpty(symbol.Name)) {
                        symbols[symbol.Name] = symbol;
                    }
                }
            }

            return symbols;
        }
    }
}
