﻿using System.Collections.Immutable;
using System.Linq;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Tools;
using static ITMO.SymbolicComputations.Base.Predefined.ArithmeticFunctions;

namespace ITMO.SymbolicComputations.Base.Visitors.Implementations.PlusFunction {
    public sealed class PlusSymbolsReducer : ISymbolVisitor<Symbol> {
        public Symbol VisitFunction(Expression expression) {
            if (!Equals(expression.Head, Plus)) {
                return expression;
            }

            var symbols = expression.Arguments
                .Where(x => !IsConstant(x))
                .ToList();

            if (symbols.Count == 0) {
                return expression;
            }

            var constants = expression.Arguments
                .Where(IsConstant)
                .ToList();

            var powers = symbols
                .GroupWithCounting()
                .Select(group =>
                    group.Count == 1
                        ? group.Item
                        : Times[group.Item, group.Count]
                );

            return new Expression(expression.Head, powers.Concat(constants).ToImmutableList());

            bool IsConstant(Symbol symbol) => symbol is Constant;
        }

        public Symbol VisitSymbol(StringSymbol symbol) => symbol;

        public Symbol VisitConstant(Constant constant) => constant;
    }
}