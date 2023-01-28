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

- addition of optional parameters in function/method call
  with default values replicating the previous behaviour
- change of the order of names optional parameters
  (just use names when doing call to prevent breaks)
- any changes to explicitly marked internal
  and or experimental API

Current list of maintained releases:

- features and bug fixes: 0.x or later
- only severe bug fixes: -
- not supported: -


0.6.0 (2023-01-28)
------------------

Upgrade urgency: LOW; new profiling API

- Add extension to fire and forget tasks to avoid warnings
- Fix warnings and adjust string literal in the typed name ToString
  (now with double quotes instead of single quotes)
- Add universal profiling API. This should be compatible with profiling,
  structured logging and or logging in general.


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
