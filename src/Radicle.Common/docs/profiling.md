Profiling primitives
====================

Profiling is an important part of any code for 2 main use cases:

- capture internal workings like logs
- capture (profile) temporary flow of the internal code path

The tooling here is made to resemble StackExchange.Redis profiling primitives
in order to be familiar and more widely reusable.

- https://github.com/StackExchange/StackExchange.Redis/blob/main/src/StackExchange.Redis/Profiling/IProfiledCommand.cs
- https://github.com/StackExchange/StackExchange.Redis/blob/69e0f3235f0f69d7b4fe659e9b5f3d2991c4bee0/docs/Profiling_v2.md
