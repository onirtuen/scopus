using System;
using System.IO;
using System.Text;
using Scopus.LexicalAnalysis.RegularExpressions;
using Scopus.SyntaxAnalysis;

namespace Scopus.LexicalAnalysis
{
    /// <summary>
    /// Represents lexer = LEXical analyzER. Lexical analysis is phase that is 
    /// done prior to syntax analyzing (by Parser).
    /// </summary>
    public interface ILexer
    {
        /// <summary>
        /// Gets index of last recognized token start position in the buffer.
        /// </summary>
        int LastTokenStartIndex { get; }

        /// <summary>
        /// Gets buffer which holds input data from stream.
        /// </summary>
        byte[] Buffer { get; }

        /// <summary>
        /// Gets array where tokens indices will be stored.
        /// </summary>
        int[] TokensIndices { get; }

        /// <summary>
        /// Gets array where token classes (types) will be stored.
        /// </summary>
        int[] TokensClasses { get; }

        /// <summary>
        /// 
        /// </summary>
        int[] TokensLengths { get; }

        /// <summary>
        /// Gets or sets <see cref="ITokenizer"/> for lexer.
        /// </summary>
        ITokenizer Tokenizer { get; set; }

        /// <summary>
        /// Gets tokens as enumerable collection. This is more convinient way to scan input.
        /// </summary>
        TokensCollection TokensStream { get; }

        /// <summary>
        /// Sets stream as source of data which should be analyzed (tokenized).
        /// </summary>
        /// <param name="stream">Data source.</param>
        void SetDataSource(Stream stream);

        /// <summary>
        /// Reads data from stream into buffer and tokenizes it. Each call to this
        /// method will cause processing of chunk (&lt BufferSize in size) of data,
        /// hence most probably this method is called more than once.
        /// </summary>
        /// <returns>true if end of stream is NOT achieved yet, otherwise - false.</returns>
        bool ReadTokens();

        /// <summary>
        /// Sets encoding to use in lexical analysis
        /// </summary>
        /// <param name="encoding">Encoding being used in lexical analysis. Default is Encoding.ASCII.</param>
        void SetEncoding(Encoding encoding);

        /// <summary>
        /// Gets or sets notation for regular expressions used in UseTerminal() and IgnoreTerminal() methods
        /// </summary>
        RegExpNotation RegExpNotation { get; set; }

        /// <summary>
        /// Performs initializations before use by parser. Parser should call this method.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Gets number of defined tokens.
        /// </summary>
        int TokensNumber { get; }

        /// <summary>
        /// Returns terminal that represents epsilon (ε, empty word). 
        /// Used only for production rules in Grammar definition, i.e. S --> ε
        /// </summary>
        /// <returns>Terminal object representing epsilon (ε, empty word)</returns>
        Terminal UseEpsilon();

        /// <summary>
        /// Tells tokenizer to recognize given pattern as terminal and pass it to Parser
        /// </summary>
        /// <param name="regexp">Regular expression representing terminal</param>
        /// <param name="lexicalAction">Lexical action that will be performed if given token is recognized. Lexical action 
        ///   may return boolean value that indicates whether recognized token should be used or not (ignored)</param>
        /// <param name="notation">Regular expression notation used in given regexp</param>
        /// <returns>Terminal variable for using in a production rules</returns>
        Terminal UseTerminal(string regexp, Func<Token, bool> lexicalAction = null, RegExpNotation notation = RegExpNotation.POSIXNotation);

        /// <summary>
        /// Tells tokenizer to recognize given pattern and DO NOT pass it to Parser. Using this method
        /// it is possible to implement a filtering preprocessor.
        /// </summary>
        /// <param name="ignoree">Regular expression represening pattern to ignore.</param>
        /// <param name="lexicalAction">Lexical action that will be performed if given token is recognized. Lexical action 
        ///   may return boolean value that indicates whether recognized token should be used or not (ignored).
        ///   When this method is used to specify token, if the lexical action will return true the token will still be ignored</param>
        /// <param name="notation">Regular expression notation used in given regexp</param>
        void IgnoreTerminal(string ignoree, Func<Token, bool> lexicalAction = null, RegExpNotation notation = RegExpNotation.POSIXNotation);
    }
}