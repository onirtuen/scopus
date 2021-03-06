﻿using System.Collections.Generic;
using System.Text;

namespace Scopus.LexicalAnalysis.RegularExpressions
{
    internal class RangeRegExp : RegExp
    {
        private readonly char mLeft;
        private readonly char mRight;

        internal RangeRegExp(char left, char right, Encoding encoding)
        {
            mLeft = left;
            mRight = right;
            Encoding = encoding;
        }

        internal override FiniteAutomata AsNFA()
        {
            var nfa = new FiniteAutomata("range");

            for (int c = mLeft; c <= mRight; c++)
            {
                var literal = (char) c;
                var bytes = Encoding.GetBytes(new[] { literal });

                State state = nfa.StartState;
                InputChar inputChar;

                for (int i = 0; i < bytes.Length - 1; i++)
                {
                    List<State> transitions;
                    inputChar = InputChar.For(bytes[i]);
                    if (state.Transitions.TryGetValue(inputChar, out transitions) && transitions.Count > 0)
                    {
                        state = transitions[0];
                    }
                    else
                    {
                        var newState = new State("range:" + bytes[i]);
                        state.Transitions.Add(inputChar, new List<State> { newState });
                        state = newState;
                    }
                }
                // Assign last transition to Terminator for everyone
                inputChar = InputChar.For(bytes[bytes.Length - 1]);
                state.AddTransitionTo(nfa.Terminator, inputChar);
            }

            return nfa;
        }

        public bool Equals(RangeRegExp rangeRegExp)
        {
            if (!base.Equals(rangeRegExp)) return false;

            if (rangeRegExp.mRight != mRight || rangeRegExp.mLeft != mLeft) return false;

            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return base.Equals(obj);

            var rangeRegExp = obj as RangeRegExp;
            if (rangeRegExp != null)
                return Equals(rangeRegExp);

            return false;
        }

        public override string ToString()
        {
            return "[" + mLeft + "-" + mRight + "]";
        }
    }
}
