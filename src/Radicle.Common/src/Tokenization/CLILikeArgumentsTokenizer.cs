namespace Radicle.Common.Tokenization;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Radicle.Common.Check;
using Radicle.Common.Tokenization.Base;
using Radicle.Common.Tokenization.Models;

/// <summary>
/// White-space and escaped string tokenizer like the one used
/// to parse command line arguments.
/// </summary>
public sealed class CLILikeArgumentsTokenizer : StringTokenizer
{
    private static readonly HashSet<char> Spaces = new(new[] { ' ', '\t', '\r', '\n', '\f', '\v' });

    private readonly IStringLiteralDefinition stringLiteralDefinition;

    /// <summary>
    /// Initializes a new instance of the <see cref="CLILikeArgumentsTokenizer"/> class.
    /// </summary>
    /// <param name="parent">Optional parent tokenizer.</param>
    /// <param name="stringLiteralDefinition">Optional string
    ///     literal definition to use to parse string literals.
    ///     Defaults to <see cref="CLikeStringLiteralDefinition.Normal"/>
    ///     with double quotes quoting mark.</param>
    public CLILikeArgumentsTokenizer(
            IStringTokenizer? parent = null,
            IStringLiteralDefinition? stringLiteralDefinition = null)
            : base(parent)
    {
        this.stringLiteralDefinition = stringLiteralDefinition
                ?? CLikeStringLiteralDefinition.Normal;
    }

    /// <inheritdoc/>
    protected override TokenMatch TryReadToken(
            TokenString input,
            int startAt = 0)
    {
        Ensure.Param(input).Done();
        Ensure.Param(startAt).InRange(0, input.StringValue.Length).Done();

        string inputString = input.StringValue;
        bool inToken = false;
        List<TokenWithValue> buffer = new();

        for (int pos = startAt; pos < inputString.Length; pos++)
        {
            char ch = inputString[pos];

            if (Spaces.Contains(ch))
            {
                // yield the token so far read till this space character
                if (inToken)
                {
                    break;
                }

                /* else just skip */
                continue;
            }

            inToken = true;

            // try to read string literal
            TokenDecoding literalDecoding = this.stringLiteralDefinition
                    .TryReadStringLiteral(input, startAt: pos);

            if (literalDecoding is TokenNoMatch)
            {
                buffer.Add(new TokenString(
                        inputString,
                        pos,
                        endAt: pos,
                        continueAt: pos + 1,
                        stringValue: new string(ch, 1))
                {
                    Parent = input,
                });
            }
            else if (literalDecoding is TokenFailure literalDecodingFailure)
            {
                return literalDecodingFailure;
            }
            else if (literalDecoding is TokenWithNoValue literalDecodingNoValue)
            {
                // advance in position and continue
                pos = literalDecodingNoValue.EndAt;
            }
            else if (literalDecoding is TokenWithValue literalDecodingValue)
            {
                // advance in position and continue
                pos = literalDecodingValue.EndAt;
                buffer.Add(literalDecodingValue);
            }
        }

        if (buffer.Count > 0)
        {
            return Merge(buffer);
        }

        // the input string was exhausted to end
        return new TokenWithNoValue(
                inputString,
                startAt,
                endAt: Math.Max(startAt, inputString.Length - 1),
                continueAt: inputString.Length)
        {
            Parent = input,
        };
    }

    private static TokenWithValue Merge(IReadOnlyList<TokenWithValue> buffer)
    {
        Ensure.Collection(buffer).NotEmpty().Done();

        TokenWithValue first = buffer[0];
        TokenWithValue last = buffer[buffer.Count - 1];

        // binary
        if (buffer.Any(d => d is TokenBinary))
        {
            List<byte> bytes = new();

            foreach (TokenWithValue tokenWithValue in buffer)
            {
                if (tokenWithValue is TokenBinary tb)
                {
                    bytes.AddRange(tb.Bytes);
                }
                else if (tokenWithValue is TokenString ts)
                {
                    bytes.AddRange(Encoding.UTF8.GetBytes(ts.StringValue));
                }
            }

            return new TokenBinary(
                    first.SourceString,
                    first.StartAt,
                    endAt: last.EndAt,
                    continueAt: last.ContinueAt,
                    bytes: bytes)
            {
                Parent = first.Parent,
            };
        }

        StringBuilder sb = new();

        foreach (TokenWithValue tokenWithValue in buffer)
        {
            if (tokenWithValue is TokenString ts)
            {
                sb = sb.Append(ts.StringValue);
            }
        }

        return new TokenString(
                first.SourceString,
                first.StartAt,
                endAt: last.EndAt,
                continueAt: last.ContinueAt,
                stringValue: sb.ToString())
        {
            Parent = first.Parent,
        };
    }
}
