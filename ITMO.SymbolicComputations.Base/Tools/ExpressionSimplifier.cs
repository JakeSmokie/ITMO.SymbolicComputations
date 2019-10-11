using System.Collections.Immutable;
using System.Linq;
using ITMO.SymbolicComputations.Base.Models;

namespace ITMO.SymbolicComputations.Base.Tools {
    public static class ExpressionSimplifier {
        public static BaseSymbol Simplify(this BaseSymbol symbol) {
            return symbol switch {
                Function function => function.Symbol switch {
                    "Sum" => SimplifySum(function),
                    "Mul" => SimplifyProduct(function),
                    _ => symbol
                },
                BinaryOperation binaryOperation => binaryOperation.Name switch {
                    "Sub" => symbol,
                    _ => symbol
                },
                _ => symbol
            };

            BaseSymbol SimplifySum(Function function) {
                var arguments = function.Arguments
                    .Select(x => x.Simplify())
                    .ToImmutableList();

                arguments = MergeSums(arguments);
                arguments = ReduceConstants(arguments);
                arguments = SortEntries(arguments);

                return arguments.Count == 1
                    ? arguments[0]
                    : new Function(function.Symbol, arguments);

                ImmutableList<BaseSymbol> ReduceConstants(ImmutableList<BaseSymbol> arguments) {
                    var summed = new Constant(
                        arguments.OfType<Constant>()
                            .Select(x => x.Value)
                            .Sum()
                    );

                    var symbols = arguments
                        .Where(x => !(x is Constant))
                        .ToImmutableList();

                    return summed.Value == 0m
                        ? symbols
                        : symbols.Add(summed);
                }

                ImmutableList<BaseSymbol> MergeSums(ImmutableList<BaseSymbol> arguments) {
                    var sums = arguments
                        .OfType<Function>()
                        .Where(x => x.Symbol == "Sum")
                        .ToImmutableList();

                    return sums.SelectMany(x => x.Arguments)
                        .Concat(arguments.Except(sums))
                        .ToImmutableList();
                }

                ImmutableList<BaseSymbol> SortEntries(ImmutableList<BaseSymbol> arguments) {
                    var constants = arguments
                        .OfType<Constant>()
                        .ToImmutableList();

                    var symbols = arguments
                        .OfType<Symbol>()
                        .OrderBy(x => x.Name)
                        .ToImmutableList();

                    return arguments
                        .Except(constants)
                        .Except(symbols)
                        .Concat(symbols)
                        .Concat(constants)
                        .ToImmutableList();
                }
            }

            BaseSymbol SimplifyProduct(Function function) {
                var first = function.MapArguments(x => x.Simplify());

                var symbols = first.Arguments
                    .Where(x => !(x is Constant) && !(x is Symbol))
                    .ToImmutableList();

                var product = new Constant(
                    first.Arguments
                        .OfType<Constant>()
                        .Select(x => x.Value)
                        .Aggregate(1m, (acc, x) => acc * x)
                );

                return first;
            }
        }
    }
}