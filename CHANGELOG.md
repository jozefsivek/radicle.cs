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
