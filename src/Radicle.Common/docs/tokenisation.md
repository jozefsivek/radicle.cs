Tokenisation and parsing
========================

The main aim of the tokenisation and parsing is to provide
ability to either make sense of user input string or to provide
reverse path for parsing string encoded values.

One way or another the tooling is aimed for the cases when
[regular expression matching groups](https://www.regular-expressions.info/quickstart.html)
will not cut it.

The [namespace](../src/Tokenization/) described further is
`Radicle.Common.Tokenization`.


Low level string parsing
------------------------

The most low level tooling is `StopWordStringParser`. Normally
this should be the last resort if there is no higher tooling available for
parsing the value (like CLI–like input, or C style string literals).

The parser operates by reading individual characters of the string input.

The configuration of the parser is non–complex. One defines specific
stopwords and action to perform when the stopword is encountered.
In the end the resulting instance can be used to parse input string to
collection of `ParsedToken` instances with no built-in error handling
as the parsed tokens themselves should be later used to interpret errors
in the input values. The only aid of the models for error detection
is in the form of incomplete read flag mentioned later.

First check [the models](../src/Tokenization/Models/). Any input text
can be divided in to the series of parsed tokens, e.g.:

    "very long input text ..."
     \__/V\__/V\___/V ...
                 \   \_ token
                  \_ token

There are 3 forms of parsed token (all have string value):

- `ParsedTokenStopWord`: parsed token stop word, where stop word is defined
  within parser. _A common example is markdown symbols for bold or emphasised word
  in form of asterisk or underscore character respectively. Or a parenthesis (`{`, `(`)
  in programming language_
- `ParsedTokenControl`: token which was preceeded by special stop word,
  this token has a flag determining if the token is complete or incomplete (read).
  _Think of e.g. unicode escape sequence like `\uHHHH`, where `\` is a stopword
  and `uHHHH` is control token, which can be potentially trimmed
  at the end of string value like `uH`._
- `ParsedTokenFreeText`: free text token which is not stop word or control token


### How we get a collection of tokens from the string input?

The first step is to provide a list of stop words in the form of the aforementioned
`ParsedTokenStopWord`. The stop words are matched completely, so incomplete
stopword will be treated only as `ParsedTokenFreeText`. When parsing, the stop words
are part of the returned enumeration of tokens. The stop words can be omitted,
however this will give you instance of parser which just returns the whole input as free text token.

The second step is to provide a callback defining what to do when stop word is encountered.
Or specifically what to do with the characters following a stop word.
You can decide to omit this callback in which case the parser will yield
only stopwords and free text tokens.

The callback receives the `ParsedReadState` which contains buffer of characters
after the last yielded stopword as well as this stopword. The callback then
returns desired action, which can be one of:

`ParseAction.Continue`: basically continue normally, so the parser will
continue yielding free text and search for next stopword

`ParseAction.YieldControl`: tell parser to yield what ever
is in the aforementioned buffer as control. Even if the buffer is empty,
in which case the yielded token is empty. E.g.:

    Lets assume a parser with one stopword "STOP" and value to parse:

    "input with a STOPcode and text"
                         ^-the current position of the parser

    parsed read state: stop word="STOP"; buffer="code"
    so far yielded 2 tokens:
        - free text "input with a "
        - stopword "STOP"

    with YieldControl the following yielded tokens will be:
        - control token "code"
        - free text " and text"

`ParseAction.ReadMoreControl`: tell parser to advance in the position,
add character to the buffer and call the callback again. If the string input
is exhausted the parser will yield control token but mark it as incomplete.

`ParseAction.ContinueWithControllUntil({positive_amount_of_caracters})`:
tell parser to read exactly given number of characters, yield those as
control token and then continue as normally. If the string input
is exhausted the parser will yield control token but mark it as incomplete.
If the callback request to read less characters than in the buffer,
the parsing will fail as that is a bug in the callback.

`ParseAction.YieldFreeText`: tell parser to yield what ever
is in the aforementioned buffer as free text. Even if the buffer is empty,
in which case the yielded token is empty.

For more examples of various possible returned collections of tokens,
see the unit tests. Or play with the parser. The key is to keep
callback readable and simple.


### Possible use cases

The callback logic very much defines the behaviour. It allow one to
have a special stop words which can shield the consecutive characters.
So for example you can have an "escape sequence" stopword which can be
followed even by another stopword, without that being caught by parser.
Like is commonly done for string literals when backslash can escape
conflicting characters.

Always think of this form of parsing as pre–processing. On its own is limited,
however if it is a first stage of a parsing it can be a powerfull tool
to quickly write a stream parser with domain specific API without
the necessity to work on raw input text.


High level string parsing
-------------------------

TODO: TBD
