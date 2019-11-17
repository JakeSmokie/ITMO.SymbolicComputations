﻿using System;
using System.Collections.Immutable;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Visitors.Casting;
using static ITMO.SymbolicComputations.Base.Predefined.Functions;

namespace ITMO.SymbolicComputations.Base.Visitors.Evaluation {
    public sealed class FunctionEvaluator : ISymbolVisitor<(ImmutableList<Symbol>, Symbol)> {
        public (ImmutableList<Symbol>, Symbol) VisitExpression(Expression expression) {
            var funcHead = expression.Head.Visit(AsExpressionVisitor.Instance);

            if (funcHead == null) {
                return (ImmutableList<Symbol>.Empty, expression);;
            }

            if (!Equals(funcHead.Head, Fun)) {
                return (ImmutableList<Symbol>.Empty, expression);
            }
            
            if (expression.Arguments.Count != 1) {
                throw new ArgumentException("You can't apply not 1 argument");
            }
            
            if (funcHead.Arguments.Count != 2) {
                throw new ArgumentException("Function declaration should contain only 2 arguments");
            }

            var variableSymbol = funcHead.Arguments[0];
            var variable = variableSymbol.Visit(AsStringSymbolVisitor.Instance);

            if (variable == null) {
                throw new ArgumentException("Fun parameter can be only StringSymbol. Something gone wrong");
            }

            var functionBody = funcHead.Arguments[1];
            var functionArgument = expression.Arguments[0];

            var substituted = functionBody.Visit(new VariableReplacer(variable, functionArgument));

            return substituted.Visit(new FullEvaluator());
        }

        public (ImmutableList<Symbol>, Symbol) VisitSymbol(StringSymbol symbol) =>
            (ImmutableList<Symbol>.Empty, symbol);

        public (ImmutableList<Symbol>, Symbol) VisitConstant(Constant constant) =>
            (ImmutableList<Symbol>.Empty, constant);
    }
}