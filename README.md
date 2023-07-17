Libraries of radicle.cs
=======================

Useful utilities for the C# and dotnet in general.


Release
-------

Check [nuget.org](https://www.nuget.org/profiles/jozef.sivek) for packages.

Check [CHANGELOG](CHANGELOG.md) for release notes and or changelog info.


License and authors
-------------------

license: [MIT License](LICENSE)
authors:

- owner: Jozef Sivek


Development tooling and utilities
---------------------------------

Use git plus C# editor or IDE (like Visual studio or similar)

- [publish checklist](checklist.md)
- [CI test run](ci-test)
- [flush of all build binaries](flushall)
- complete flush: `$ git clean -xdf # clean all non-source controlled files`
  (it may delete more then you want)


Documentation
-------------

Check `docs/` directory in the [repository](docs/README.md) root
and in any project root. Some highlights:

- [repository documentation](docs/README.md)
    - [files layout](docs/layout.md)
    - [testing](docs/testing.md)

- [Radicle.Common utilities](src/Radicle.Common/docs/README.md)
    - [profiling primitives](src/Radicle.Common/docs/profiling.md)
    - [concurrent processing with tasks](src/Radicle.Common/docs/concurrent_processing.md):
      a powerful toolbox for parallel tasks execution with
      low overhead and tight controll over how results are processed

