﻿using System;
using NUnit.Framework;
using Scopus.LexicalAnalysis;
using Scopus.LexicalAnalysis.Algorithms;
using Scopus.LexicalAnalysis.RegularExpressions;
using Scopus.SyntaxAnalysis;

namespace ScopusUnitTests
{
    [TestFixture]
    public class ParserInterfaceTests
    {
        private ITokenizer tokenizer;

        [SetUp]
        public void InitTests()
        {
            tokenizer = new RegExpTokenizer();
            tokenizer.SetTransitionFunction(new TableDrivenTransitionFunction());
        }

        [Test]
        public void GrammarEntitySequenceTest()
        {
            NonTerminal E = new NonTerminal("E"), T = new NonTerminal("T"), F = new NonTerminal("F");

            var prod = E --> T & F;
            
            Assert.That(prod.ToString(), Is.EqualTo("E --> T F"));
            Console.WriteLine(prod);
        }

        [Test]
        public void GrammarEntitySequenceWithTokensTest()
        {
            NonTerminal E = new NonTerminal("E"), T = new NonTerminal("T"), F = new NonTerminal("F");

            var plus = tokenizer.UseTerminal(RegExp.Literal('+'));
            var mult = tokenizer.UseTerminal(RegExp.Literal('*'));
            var leftBrace = tokenizer.UseTerminal(RegExp.Literal('('));
            var rightBrace = tokenizer.UseTerminal(RegExp.Literal(')'));

            var prod = E --> T & mult & leftBrace &  plus & rightBrace;

			Assert.That(prod.ToString(), Is.EqualTo("E --> T * ( " + "+" + " )"));
            Console.WriteLine(prod);
        }

        [Test]
        public void GrammarRecoursiveProductionTest()
        {
            NonTerminal E = new NonTerminal("E"), T = new NonTerminal("T"), F = new NonTerminal("F");

            var plus = tokenizer.UseTerminal(RegExp.Literal('+'));
            var mult = tokenizer.UseTerminal(RegExp.Literal('*'));

            Assert.That((E --> E & F).ToString(), Is.EqualTo("E --> E F"));
            Assert.That((E --> F & E).ToString(), Is.EqualTo("E --> F E"));
            Assert.That((E --> E).ToString(), Is.EqualTo("E --> E"));
            Assert.That((E --> F & E & T).ToString(), Is.EqualTo("E --> F E T"));
            Assert.That((E --> plus & E).ToString(), Is.EqualTo("E --> + E"));
            Assert.That((E --> E & plus).ToString(), Is.EqualTo("E --> E +"));
            Assert.That((E --> plus & E & mult).ToString(), Is.EqualTo("E --> + E *"));
        }

        [Test]
        public void ProductionWithSemanticActionTest()
        {
            NonTerminal E = new NonTerminal("E"), T = new NonTerminal("T"), F = new NonTerminal("F");

            var plus = tokenizer.UseTerminal(RegExp.Literal('+'));
            var mult = tokenizer.UseTerminal(RegExp.Literal('*'));

            int testValue = 0;

			var g = new AugmentedGrammar()
                        {
                            E --> T & F ^ (v => testValue = 3), // Semantic action of production 1
                            T --> plus,
                            E --> T & plus ^ (v => testValue = 5), // production 3
                            E --> plus & mult & T ^ (v => testValue = 8), //production 4
                        };

            Assert.That(testValue, Is.EqualTo(0));
            g.Productions[1].SemanticAction(null);
            Assert.That(testValue, Is.EqualTo(3));
            g.Productions[3].SemanticAction(null);
            Assert.That(testValue, Is.EqualTo(5));
            g.Productions[4].SemanticAction(null);
            Assert.That(testValue, Is.EqualTo(8));
        }

        [Test]
        public void ProductionWithSemanticActionComplicatedTest()
        {
            var E = new NonTerminal("E");
            var T = new NonTerminal("T");
            var F = new NonTerminal("F");

            var plus = tokenizer.UseTerminal(RegExp.Literal('+'));
            var mult = tokenizer.UseTerminal(RegExp.Literal('*'));
            var leftBrace = tokenizer.UseTerminal(RegExp.Literal('('));
            var rightBrace = tokenizer.UseTerminal(RegExp.Literal(')'));

            var kuj = 0; // куй :D

            var prod1 = E --> E & plus & T ^ (v => kuj = 2);
            var prod2 = E --> T;
            var prod3 = T --> T & mult & F ^ (v => kuj = 3);
            var prod4 = T --> F;
            var prod5 = F --> leftBrace & E & rightBrace;

            Assert.That(kuj, Is.EqualTo(0));
            prod1.PerformSemanticAction(null);
            Assert.That(kuj, Is.EqualTo(2));
            prod2.PerformSemanticAction(null);
            Assert.That(kuj, Is.EqualTo(2));
            prod3.PerformSemanticAction(null);
            Assert.That(kuj, Is.EqualTo(3));
            prod4.PerformSemanticAction(null);
            Assert.That(kuj, Is.EqualTo(3));
            prod5.PerformSemanticAction(null);
            Assert.That(kuj, Is.EqualTo(3));
        }

        [Test]
        public void AugmentedGrammarInitialProductionTest()
        {
            var P = new NonTerminal("P");
			var g = new AugmentedGrammar()
                        {
                            P --> P
                        };

            Assert.That(g.ToString(), Is.EqualTo("(0) " + g.InitialProduction.Symbol + " --> P\n(1) P --> P\n"));
        }
    }
}
