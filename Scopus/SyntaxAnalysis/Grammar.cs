using System.Collections;
using System.Collections.Generic;

namespace Scopus.SyntaxAnalysis
{
    public class Grammar : ICollection<Production>
    {
        protected Grammar()
        {
            Productions = new List<Production>();
            NonTerminals = new List<NonTerminal>();
            UsedTerminals = new List<Terminal>();
            GrammarSymbols = new List<GrammarEntity>();

            var endMark = new Terminal(Terminal.END_MARKER_TOKEN_NAME, Terminal.END_MARKER_TOKEN_ID);
            UsedTerminals.Add(endMark);
            GrammarSymbols.Add(endMark);
        }

        // returns end marker terminal (stored at index 0)
        internal Terminal EndMarker { get { return UsedTerminals[0]; } }

        // contains productions, which configurate this grammar
        internal List<Production> Productions { get; private set; }
        // contains all non-terminal symbols, met in productions
        internal List<NonTerminal> NonTerminals { get; private set; }
        // may not be equal to amount of tokens added to tokenizer
        internal List<Terminal> UsedTerminals { get; private set; } // contains terminals, only found in productions;
        // contains union of UsedTerminals and NonTerminals
        internal List<GrammarEntity> GrammarSymbols { get; private set; }

        public Production this[int index]
        {
            get { return Productions[index]; }
        }


        #region ICollection<Production> Members

        public IEnumerator<Production> GetEnumerator()
        {
            return Productions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public virtual void Add(Production production)
        {
            Productions.Add(production);
            production.ID = Productions.IndexOf(production);

            var expression = new List<GrammarEntity>(production.Expression) {production.Symbol};

            foreach (GrammarEntity entity in expression)
            {
                if (!GrammarSymbols.Contains(entity)) GrammarSymbols.Add(entity);
                // TODO: check: else continue;

                var terminal = (entity as Terminal);
                if (terminal != null && !UsedTerminals.Contains(terminal))
                {
                    UsedTerminals.Add(terminal);
                    continue;
                }

                var nonTerminal = (entity as NonTerminal);
                if (nonTerminal != null && !NonTerminals.Contains(nonTerminal))
                {
                    NonTerminals.Add(nonTerminal);
                    nonTerminal.ID = NonTerminals.IndexOf(nonTerminal);
                }
            }
        }

        public void Clear()
        {
            Productions.Clear();
        }

        public bool Contains(Production item)
        {
            return Productions.Contains(item);
        }

        public void CopyTo(Production[] array, int arrayIndex)
        {
            Productions.CopyTo(array, arrayIndex);
        }

        public bool Remove(Production item)
        {
            return Productions.Remove(item);
        }

        public int Count
        {
            get { return Productions.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion

        public override string ToString()
        {
            string value = string.Empty;
            foreach (Production production in Productions)
            {
                value += string.Format("({0}) {1}\n", production.ID, production);
            }

            return value;
        }
    }
}