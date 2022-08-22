namespace Radicle.Common.Tokenization;

using System;
using System.Collections.Generic;
using Radicle.Common.Tokenization.Models;

/// <summary>
/// Interface of string tokenizer. Tokenizer can
/// split given string to individual sub string tokens which can be
/// used for further analysis. Note the tokens can have different
/// length than the original input and be missing ignored parts
/// by the specific tokenizer.
/// </summary>
public interface IStringTokenizer
{
    /*
     How this work:

     original input:
                                      ignored parts from original input
                                   ,_/_,
                 |012.............................................................n| <-- string characters
                   |               |   |                 |     |                  |
    tokenizer 1    <-----t[1] 1---->   <-----t[1] 2------> ... <-t[1] n1 (binary)->  <- binary tokens are passed
                   |               |   |                 |     |                  |
    tokenizer 2    <t[2] 1> <t[2] 2>   <-----t[2] 3------>     |                  |
                   |      | |      |   |                 |     |                  |
    ...            |      | |      |   |                 |     |                  |
                   |      | |      |   |                 |     |                  |
    tokenizer x    <t[x] 1> <t[x] 2>   <t[x] 3-> <-t[x] 4>     <-t[x] nx (binary)-> <-- final set of tokens
                                                                                        with their positions
                                                                                        in original input
     */

    /// <summary>
    /// Gets parent tokenizer which is applied before this tokenizer.
    /// Note that binary decoded strings are not parsed further.
    /// If undefined then this is the first tokenizer in the tokenization chain.
    /// </summary>
    IStringTokenizer? Parent { get; }

    /// <summary>
    /// Convert given <paramref name="input"/> string to tokens.
    /// </summary>
    /// <param name="input">Input to convert.</param>
    /// <param name="startAt">Optional start position of the parsing.</param>
    /// <returns>Collection of tokens, may be empty
    ///     if <paramref name="input"/> yields no tokens.</returns>
    /// <exception cref="ArgumentNullException">Thrown
    ///     if required parameter is <see langword="null"/>.</exception>
    /// <exception cref="FormatException">Thrown
    ///     if <paramref name="input"/> has format errors.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown
    ///     if <paramref name="startAt"/> is out of allowed
    ///     range [0, input_length].</exception>
    IEnumerable<TokenWithValue> Parse(
            string input,
            int startAt = 0);
}
