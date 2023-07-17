Concurrent processing
=====================

The modern approach to concurrent processing in the dotnet
is driven by asynchronous code. In a simple example one can
do the following:

```
var task1 = GetTask();
var task2 = GetTask();

await task1;
await task2;
```

This works well for a simple workflow but it does not scale well
with e.g. dozen of tasks which we may produce by means of enumeration.

Because of this the library contains 3 generic as well as plain
primitives for processing unbound collection of tasks:

- `TaskInterweaver`: most lightweight utility with up to 2 degrees of paralelism
- `TaskBuffer`: beefier utility for buffer–like task processing
- `TaskRiver`: utility for most optimal packing of the task

The generic versions work with generic enqueued `Task<TValue>`
and meta–data object associated with each task.
The plain versions support only non generic tasks.


The goals
---------

The main aims of these utilities are:

- simple use when tasks are produced by enumeration
- low memory and orchestration processing overhead
- controll over amount of pending tasks, i.e.
  tasks can be created when resources ara available
  in place of spinning all tasks at ones
- clear exception propagation (there are no hidden
  background threads capturing exceptions from the tasks)
- possibility of serial result processing which greatly
  increases amount of available model options for user
- simple form of scalability at creation time or runtime

One thing which was never an aim is to enqueue tasks
from multiple threads. All the utilities are aimed to be
operated by serial code, as they are not thread safe.

This should be still a resonable because parallel queuing,
if needed at all, can be implement with concurrent queue.

The error handling is simple as well, any failing enqueued task
is let its exception to propagate via `Flush*` method
(and or `EnqueueAsync` in case of `TaskInterweaver`)
to calling user code and the utility is cleaned to prevent
its internal state corruption.

This failure handling is in a line good rule of thumb to
fail fast on exceptions.

Description
-----------

The `TaskInterweaver` has low paralelism between 1 (level 0) to 2 (level 1)
concurrent tasks. It is aimed for cases when you can process a result
while awaiting a next one. Like interweaving two different
but complementary tasks. Because of this the instance has just
`EnqueueAsync` and `FlushAsync` method, for enqueuing and awaiting
tasks and method for final flush of any stored tasks.

`TaskBuffer` and `TaskRiver` have arbitrary paralelism. It is specified
as "maximum capacity" which can be capped in the runtime by setting
capacity parameter (it defaults to maximum capacity).
Because of this both instances work by enqueueing tasks and then
awaiting them if the instance become full, the capacity is reached.
This is thus a generalisation of the aforementioned example
to arbitrary amount of tasks.

In order to simplify certain cases the instances can be configured
to indicate reaching capacity after certain amount of calls.
In this way user does not need to flush instance after
enumeration if the amount of tasks was known at the beginning.

The best way to see the difference of the individual tools is to
draw a task lifetime plot:

    ===== - task as thread in time
    ***** - tasks result processing (as callback)
    ..... - tasks result processing (as calling user code)
    >N    - enqueuing of the new task by user
    X     - flushing remaining task by user

`TaskInterweaver<TValue, TMetaData>` with level 1:

               A
    paralelism | >1=====**** >3===  ** >5==========X****
    (threads)  |  >2======    **** >4=====****
               +-------------------------------------->
                                time

Note the callbacks or later user processing never overlaps.
this simplifies the concurrency processing greatly as we can mix
thread safe (tasks) and non–thread (result processing) safe code safely.

In this way the utility works best when callback is
as fast or faster than task completion.
For the plain version there is always at least one task awaited

`TaskInterweaver` with level 1:

               A
    paralelism | >1===== >3=== >5==========X
    (threads)  |  >2====== >4=====
               +-------------------------------------->
                                time

The working with level 0 is equal to serial processing,
thus any level value between 0 and 1

`TaskBuffer` with capacity of 4:

               A
               | >1==..      >5========..     >9=X.
    paralelism |  >2====..    >6=====    .     >10===.
    (threads)  |   >3==   .    >7=        .
               |    >4=====..   >8=        ...
               +-------------------------------------->
                                time

Task buffer approach is beneficial for tasks of similar completion
time and or when the order of tasks creation and their results
processing is important. Task buffer is basically FIFO stack
for tasks.


`TaskRiver` with capacity of 4:

               A
               | >1==..>5========   ... >10==
    paralelism |  >2====....>6=====      X..
    (threads)  |   >3==      >7= . >9=      ..
               |    >4=====   ... >8=         ...
               +-------------------------------------->
                                time

In task river the task results are available for processing by user
as soon as possible (first completed tasks are served first).
This tight packing (like logs in the river) allows for better utilisation
of the available CPU time. However the order of results is mixed.

It helps in the situations when some tasks take disproportionally
more time and they will make e.g. aforementioned buffer waste
the resources by awaiting only one or few tasks while more could
be processed.

Examples
--------

`TaskInterweaver`:

    TaskInterweaver<TValue, TMeta> ti = new(
            item => /* callback body */);

    for (...)
    {
        await ti.EnqueueAsync(task, meta).ConfigureAwait(false);
    }

    // this can be skipped if autoFlushCallCount is used
    await ti.FlushAsync().ConfigureAwait(false);


`TaskBuffer`:

    TaskBuffer<TValue, TMeta> buffer = new();

    for (...)
    {
        if (buffer.Enqueue(task, meta))
        {
            await foreach ((int result, int meta) in buffer.FlushAsync().ConfigureAwait(false))
            {
                ...
            }
        }
    }

    // this final enumeration is not needed
    // if fullCallCount is used
    await foreach ((int result, int meta) in buffer.FlushAsync().ConfigureAwait(false))
    {
        ...
    }


`TaskRiver`:

    TaskRiver<TValue, TMeta> river = new();

    for (...)
    {
        if (river.Enqueue(task, meta))
        {
            await foreach ((int result, int meta) in river.FlushAsync().ConfigureAwait(false))
            {
                ...
            }
        }
    }

    // this final enumeration is not needed
    // if fullCallCount is used
    await foreach ((int result, int meta) in river.FlushAllAsync().ConfigureAwait(false))
    {
        ...
    }
