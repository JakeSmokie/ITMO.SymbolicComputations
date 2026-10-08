using System;
using System.Globalization;
using System.Linq;
using ITMO.SymbolicComputations.Base.Models;
using ITMO.SymbolicComputations.Base.Parsing;
using ITMO.SymbolicComputations.Base.StandardLibrary;
using Xunit;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ArithmeticFunctions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.Functions;
using static ITMO.SymbolicComputations.Base.StandardLibrary.ListFunctions;

namespace ITMO.SymbolicComputations.Base.Tests {
    public sealed class ParserTests {
        [Theory]
        [InlineData("2 + 3 * 4", "14")]
        [InlineData("(2 + 3) * 4", "20")]
        [InlineData("2 ^ 3 ^ 2", "512")]
        [InlineData("-2 ^ 2", "-4")]
        [InlineData("(-2) ^ 2", "4")]
        [InlineData("2 ^ -2", "0.25")]
        [InlineData("2 ^ -2 ^ 2", "0.0625")]
        [InlineData("8 / 2 / 2", "2")]
        [InlineData("5 - 3 - 1", "1")]
        [InlineData("2--3", "5")]
        [InlineData("++3", "3")]
        [InlineData(".5 + 1.25e2", "125.5")]
        public void ArithmeticHasStandardMeaning(string formula, string expected) {
            var result = new SymbolicContext().Run(SymbolicParser.Parse(formula)).Item2;
            Assert.Equal(decimal.Parse(expected, CultureInfo.InvariantCulture), Assert.IsType<Constant>(result).Value);
        }

        [Fact]
        public void PowerAndUnaryMinusHaveDistinctTrees() {
            Assert.Equal(Times[-1, Power[2, 2]], SymbolicParser.Parse("-2^2"));
            Assert.Equal(Power[Times[-1, 2], 2], SymbolicParser.Parse("(-2)^2"));
            Assert.Equal(Power[2, Power[3, 2]], SymbolicParser.Parse("2^3^2"));
        }

        [Fact]
        public void DecimalSyntaxDoesNotDependOnCurrentCulture() {
            var previousCulture = CultureInfo.CurrentCulture;
            try {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ru-RU");
                Assert.Equal(12.5m, Assert.IsType<Constant>(SymbolicParser.Parse("1.25E+1")).Value);
                Assert.Equal(0.001m, Assert.IsType<Constant>(SymbolicParser.Parse("1e-3")).Value);
                Assert.Equal(1m, Assert.IsType<Constant>(SymbolicParser.Parse("1.")).Value);
                Assert.Throws<FormulaParseException>(() => SymbolicParser.Parse("1,25"));
            }
            finally {
                CultureInfo.CurrentCulture = previousCulture;
            }
        }

        [Fact]
        public void BothCallStylesAndListSyntaxBuildEquivalentTrees() {
            Assert.Equal(Plus[1, Times[2, 3]], SymbolicParser.Parse("Plus(1, Times[2, 3])"));
            Assert.Equal(List[1, List[2, 3]], SymbolicParser.Parse("{1, {2, 3}}"));
            Assert.Equal(EmptyList, SymbolicParser.Parse("{}"));
            Assert.Equal(BuiltInSymbols.Resolve("custom")[new Symbol[0]], SymbolicParser.Parse("custom()"));
        }

        [Fact]
        public void ChainedCallsExecuteNestedFunctions() {
            var expression = SymbolicParser.Parse("Fun[x, Fun[y, x + y]][2](3)");
            var result = new SymbolicContext().Run(expression).Item2;
            Assert.Equal(5m, Assert.IsType<Constant>(result).Value);
        }

        [Fact]
        public void ParsedMapRunsAgainstTheExistingEngine() {
            var expression = SymbolicParser.Parse("Map[{1, 2, 3}][Fun[x, x ^ 2]]");
            var result = new SymbolicContext().Run(expression).Item2;
            Assert.Equal(List[1, 4, 9], result);
        }

        [Fact]
        public void BuiltInResolutionPreservesEvaluationAttributes() {
            var hold = Assert.IsType<Expression>(SymbolicParser.Parse("Hold[1 + 2]"));
            Assert.Same(Hold, hold.Head);
            Assert.Contains(Attributes.HoldAll, Assert.IsType<StringSymbol>(hold.Head).Attributes);
            Assert.Equal(Hold[Plus[1, 2]], new SymbolicContext().Run(hold).Item2);

            var assignment = Assert.IsType<Expression>(SymbolicParser.Parse("Set[x, 1]"));
            Assert.Same(Set, assignment.Head);
            Assert.Contains(Attributes.HoldFirst, Assert.IsType<StringSymbol>(assignment.Head).Attributes);
            Assert.Same(Plus, BuiltInSymbols.Resolve("Plus"));
            Assert.Contains(Attributes.Orderless, BuiltInSymbols.Resolve("Plus").Attributes);
        }

        [Fact]
        public void UnknownNamesAndUnicodeVariablesRemainSymbols() {
            Assert.Equal("скорость_2'", Assert.IsType<StringSymbol>(SymbolicParser.Parse("скорость_2'")).Name);
            Assert.Equal("sin", BuiltInSymbols.Resolve("sin").Name);
            Assert.NotSame(Sin, BuiltInSymbols.Resolve("sin"));
            Assert.Equal(BuiltInSymbols.Resolve("custom")["α"], SymbolicParser.Parse("custom(α)"));
        }

        [Theory]
        [InlineData("", 0)]
        [InlineData("   ", 3)]
        [InlineData("1 +", 3)]
        [InlineData("1 + )", 4)]
        [InlineData("(1+2", 4)]
        [InlineData("f(1,)", 4)]
        [InlineData("f[1)", 3)]
        [InlineData("{1,,2}", 3)]
        [InlineData("1e+", 3)]
        [InlineData("1.2.3", 3)]
        [InlineData("2x", 1)]
        [InlineData("2(3)", 1)]
        [InlineData("1,2", 1)]
        [InlineData("x y", 2)]
        [InlineData("1;2", 1)]
        public void ErrorsPointToTheUnexpectedCharacterOrEnd(string formula, int position) {
            var error = Assert.Throws<FormulaParseException>(() => SymbolicParser.Parse(formula));
            Assert.Equal(position, error.Position);
            Assert.Contains($"позиция {position + 1}", error.Message);
        }

        [Theory]
        [InlineData("1e100")]
        [InlineData("1e-100")]
        public void DecimalRangeErrorsDoNotSilentlyChangeValues(string formula) {
            var error = Assert.Throws<FormulaParseException>(() => SymbolicParser.Parse(formula));
            Assert.Equal(0, error.Position);
            Assert.Contains("decimal", error.Message);
        }

        [Fact]
        public void InputLengthLimitIsCheckedBeforeParsing() {
            var limits = new FormulaParserLimits { MaxInputLength = 8 };
            Assert.IsType<Constant>(SymbolicParser.Parse("12345678", limits));
            var error = Assert.Throws<FormulaParseException>(() => SymbolicParser.Parse("123456789", limits));
            Assert.Equal(8, error.Position);
        }

        [Fact]
        public void NodeLimitIncludesListHeadAndExpression() {
            Assert.Equal(List[1, 2], SymbolicParser.Parse("{1,2}", new FormulaParserLimits { MaxNodes = 4 }));
            var error = Assert.Throws<FormulaParseException>(() =>
                SymbolicParser.Parse("{1,2}", new FormulaParserLimits { MaxNodes = 3 }));
            Assert.Contains("узлов", error.Message);
        }

        [Theory]
        [InlineData("((((((((1))))))))")]
        [InlineData("--------1")]
        [InlineData("1^1^1^1^1^1^1^1")]
        public void RecursiveSyntaxIsBounded(string formula) {
            var error = Assert.Throws<FormulaParseException>(() =>
                SymbolicParser.Parse(formula, new FormulaParserLimits { MaxDepth = 4 }));
            Assert.Contains("глубин", error.Message);
        }

        [Fact]
        public void LeftAssociativeAndChainedCallTreesAreAlsoBounded() {
            var limits = new FormulaParserLimits { MaxDepth = 4 };
            Assert.Throws<FormulaParseException>(() => SymbolicParser.Parse("1+2+3+4+5", limits));
            Assert.Throws<FormulaParseException>(() => SymbolicParser.Parse("f()()()()", limits));
            Assert.Throws<FormulaParseException>(() =>
                SymbolicParser.Parse(string.Join("+", Enumerable.Repeat("1", 100))));
        }

        [Fact]
        public void InvalidLimitConfigurationCannotDisableTheDepthGuard() {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                SymbolicParser.Parse("1", new FormulaParserLimits { MaxDepth = int.MaxValue }));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                SymbolicParser.Parse("1", new FormulaParserLimits { MaxNodes = 0 }));
        }
    }
}
