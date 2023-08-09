Radicle change log (and release notes)
======================================

This file contains notes about the releases
and changes, i.e. change log
(see also [Keep a Changelog](https://keepachangelog.com/en/1.0.0/) ).

Mind the versions bellow follow [Semantic Versioning](https://semver.org/)

*Upgrade urgency levels* inspired by [Redis](https://github.com/redis/redis):

- LOW: No need to upgrade unless there are new features you want to use.
- MODERATE: Program an upgrade, but it's not urgent.
- HIGH: There is a critical bug that may affect a subset of functionality
  but can be mitigated by the user. Upgrade!
- CRITICAL: There is a critical bug affecting broad functionality
  and can not be easily mitigated by the user due to the extend or complexity.
  Upgrade ASAP.
- SECURITY: There are security fixes in the release.

*Types of changes*

- Add or Extend: new features.
- Change: changes in existing functionality.
- Deprecate: for soon to be removed features.
- Remove: removed features.
- Fix: any bug fixes.
- Secure: vulnerabilities.

*Breaking changes*

Small note on what DOES NOT count as a breaking change
(and thus not require major change update):

- any changes to explicitly marked internal
  and or experimental API
- addition of new models, methods, properties and fields

Current list of maintained releases:

- features and bug fixes: 0.x or later
- only severe bug fixes: -
- not supported: -


0.10.0 (2023-08-09)
-------------------

Upgrade urgency: LOW; new features, some potential breaking changes

- Add counters for concurrent counting for profiling or work,
  see `CountersDictionary`. The `REPLEventLoop` also prints
  default ones if used.
- Change [concurrent processing utilities](src/Radicle.Common/docs/concurrent_processing.md),
  some changes may be breaking, the API is now more unified
  and feature complete.
- Change namespace of interval modes (out of Visual namespace)
- Add DiscreteValueDistribution, inverse transform sampling
  for discrete values. See documentation of `DiscreteValueDistribution`
  for more information
- Change Shuffle extension signature to return shuffled list
- Fix transparent progress to be more usable: add event,
  interface, add methods to allow easy work with nullables,
  add option to create child stages. Some of the changes
  can be breaking.
- Add Clamp TimeSpan extension
- Add Metronome tooling for keeping up with a periodic
  events by pulling. For scheduled events naturally
  use `System.Threading.Timer`.
- Add ETA calculator with 2 possible strategies,
  with easy use together with transparent progress.


0.9.1 (2023-05-20)
------------------

Upgrade urgency: MODERATE; bugfixes and small improvements

- Fix potential memory leak in TypedName when used with default settings.
  The values in TypedName were Intern by default, this can
  cause memory leak if one creates names with high variability of
  values. And it is easy to overlook the default setting.
- Fix wacky ETA REPL calculation for non uniform progress
- Add low (1 to 2 paralle tasks) parallelization task utility
- Fix wrong use of ValueTask
- Add easier readable performance counters


0.9.0 (2023-05-11)
------------------

Upgrade urgency: LOW; new tooling and improvements

- Add support for ordering disposables so they
  are disposed in predictable order
- Add experimental utility for smart detection
  of compressed or STDIN/OUT files
- Add ASCII dots plot style and correct
  the enum values to allow future extensions
- Add optional human readable status text to
  TransparentProgress
- Add optional [cancellation token](http://blog.monstuff.com/archives/2019/03/async-enumerables-with-cancellation.html)
  into the async enumerator in TaskBuffer


0.8.0 (2023-05-04)
------------------

Upgrade urgency: LOW; new tooling

- Fix inverse colour flag behaviour for console output
- Add experimental OneFrom iterface for sum types
- Add experimental interfaces on Token models
- Add implicit string operators for string RESP types
  to ease instance creation
- Use single base class for all string like RESP values
- Add more convenience methods for RESP array
  to and from map conversion
- Add an extension to split string in to
  the lines by any known new line characters
- Add tooling for ASCII-like visualisation
  of tree structures
- Add horizontal bar plot formatter for
  progress and value ASCII-like in-line plots


0.7.0 (2023-03-31)
------------------

Upgrade urgency: LOW; new RESP3 models and RESP2 serialization

- Change auxiliary packages and fix new warnings. Also
  fix potential issue in RandomPick for changed enumerable
  and check for null keys in AllNotNull just in case the enumerable
  was no dictionary in the first place
  Check [SponsorLink blog post](https://www.cazzulino.com/sponsorlink.html)
  for the new info on build of this library when building locally.
- Add RESP2 and RESP3 models with support for RESP2 serialization
  [RESP](https://github.com/redis/redis-specifications/blob/master/protocol/RESP3.md)


0.6.2 (2023-01-28)
------------------

Upgrade urgency: LOW; new profiling API

- Add extension to fire and forget tasks to avoid warnings
- Fix warnings and adjust string literal in the typed name ToString
  (now with double quotes instead of single quotes)
- Add universal profiling API. This should be compatible with profiling,
  structured logging and or logging in general.
- Fix wrong language use


0.5.0 (2022-10-13)
------------------

Upgrade urgency: LOW; bug fixes and new API

- Fix bug preventing correct validation of long strings for no new lines
- Clarify use of base disposable class in documentation
- Add new Snippet end SnippetLiteral extension,
  more suited for trimmed string value dumps than Ellipsis


0.4.0 (2022-09-30)
------------------

Upgrade urgency: LOW; bug fixes and new API

- Fix missing "Async" in some methods (technically a breaking
  change, but minor change required)
- Add tasks handling tools for easy parallelization
  and batching/buffering
- Add hassle free gzip tooling for small data compression
- Add shell expanders for expanding home paths etc.
- Add ready to use disposable base classes
- Add variable-length quantity (VLQ) implementation
- Add utility for generating compact UUID/GUIDs
- Add partial implementation of ISO 8601:2004 dates
  parsing and serialization


0.3.0 (2022-08-22)
------------------

Upgrade urgency: LOW; new CLI and common functionality

- Add sample project to show capabilities of REPL CLI tooling
- Add tooling for building iteractive REPL (Read-eval-print loop)
  CLI (command line interface) program. Supported features includ
  progress indicators as well as scaffold for commands.
  See sample projects for examples.
- Add new extensions and parsing functionality including:
  string parameter line check, new string and binary dumps,
  new time related extensions, looping collection,
  tokenizers and parsers, string literal functionality,
  support for simple markdown for CLI, better progress class.


0.2.0 (2022-07-13)
------------------

Upgrade urgency: LOW; new common functionality

- Add better definition of breaking changes here
- Add common utility functionality
- Add flush utility and local test for CI
- Add scaffold for sample project


0.1.0 (2022-06-27)
------------------

Upgrade urgency: N/A

- initial scaffold release
