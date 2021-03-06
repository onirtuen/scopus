﻿using ParserEngine.LexicalAnalysis;

namespace ParserEngine.Exceptions
{
    /// <summary>
    /// Thrown when unexpected lexeme was read from lexeme stream.
    /// </summary>
    public class UnexpectedLexemeException : ParserException
    {
        public Token Token { get; private set; }

        public UnexpectedLexemeException(Token token)
            : base(string.Format("The lexeme was unexpected at this time ({0}:'{1}')", 
                token.GetType().Name, token) )
        {
            Token = (Token)token.Clone();
        }
    }
}
