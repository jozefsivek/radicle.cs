Testing practices
=================

Check [files layout](layout.md) as well.

C#
--

### Test method names

Unit test methods are descriptive about what is being tested, under what conditions,
and what are the expectations. Check [best practices](https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices#naming-your-tests)
which give us handy schema like (compact yet readable):

    {Name}_{Condition}_{ExpectedOutcome}

or, if no condition is not applicable:

    {Name}_{ExpectedOutcome}

Examples:

    Length_NoIssues_ReturnsOne
    Constructor_Throws

Where:

- *Name* stands for name of method under the test, class
  or *Constructor* (reserved word)
- *Condition* stands for short, human readable, `PascalCase`
  description of the test case like: *NullName*, *MissingName*.
- *ExpectedOutcome* stands for expected outcome, as short,
  human readable `PascalCase` description. Common reserved
  words are: *DoesNotThrow*, *Throws*, *Returns*,
  etc., if there is nothing better, *Works* will do


### Use of theory (xunit) inline data

Use inline data, e.g. numbers, strings. Too complex
(collections, custom types, etc.) objects in `MemberData`
can cause issues with tests listing, only solvable by:

    [MemberData(nameof(name), DisableDiscoveryEnumeration = true)]

Which is far from ideal. Thus break the unit tests methods
to more simple ones which will not require too complex data.


Unit testing
------------

Or white box testing, follow:

- run all unit tests before commits / pull request
- unit tests are witten for everything except trivial code
- unit tests should be very fast
- unit tests run in parallel
- for rest there are functional test

Run tests in the following ways:

- via the IDE, projects with `Tests` suffix
- automatically by CI/CD
- manually with `$ dotnet test` whoch should always
  work in root of the rpeository


Functional testing
------------------

Or black box testing, is preferred over the "integration testing"
because this is a simple repository.
Note "integration testing" is used for large (scale or resource intensive)
application testing.
