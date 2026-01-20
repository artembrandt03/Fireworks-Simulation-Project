# Prep Lab - Analyzing performance
Artem Brandt

## Timing Results

Benchmark configuration:

- Resolution: `800 x 600`
- Runs per batch: always 100
- Pattern: Circle pattern (same for all tests)
- Environment update loop runs until all fireworks are finished and cleared.

### Our batches and test results

| Number of Fireworks | Debug Time (ms) | Release Time (ms) |
|---------------------|-----------------|-------------------|
| 100                 | 12.43           | 3.91              |
| 1 000               | 139.77          | 18.75             |
| 10 000              | 1682.94         | 456.91            |

---

### 2. How are the times different between Debug and Release?

- Release is consistently faster than Debug for all batch sizes.
- This happens because:
  - Debug builds include extra checks and no optimizations
  - Release builds enable compiler optimizations and generally reduce overhead per update.

So the same algorithm behaves a bit differently depending on build configuration, even tho logically it’s doing the same work.

---

### 3. Trend between time and number of fireworks

Looking at the Debug numbers:

- Going from 100 to 1 000 fireworks increases time by about ×11.
- Going from 1 000 to 10 000 fireworks increases time by about ×12.

This is close to linear scaling that you taught us during the lecture: when we multiply the number of fireworks by 10, the total time is also multiplied by roughly 10–12!  
That matches what we expect, because each firework has its own particles and updates every frame; and also our benchmark loop updates the environment until 
all fireworks are done, so more fireworks logically takes more total work.

In Release, the trend is similar but less linear:

- 100 to 1 000 fireworks: about ×4.8
- 1 000 to 10 000 fireworks: about ×24

Conclusion: More fireworks equals more total time, roughly proportional to the amount of work.

---

### 4. How to benchmark other types of fireworks

To benchmark other fireworks I would:

1. Keep the benchmarking config the same
   - Same `WIDTH`, `HEIGHT`
   - Same `RUNS_PER_BATCH`
   - Same logic: create `N` fireworks, then `Update()` in a loop until the environment is empty.

2. Change exactly one variable at a time
   For example:
   - Right now I run tests where all fireworks use a circle pattern.
   - Then run another set where all fireworks use a rectangle pattern.
   - Then again with star pattern etc.
   This lets me compare how each pattern’s complexity affects timing.

3. Use the same batch sizes
   - For consistency I'd benchmark 100, 1 000, and 10 000 fireworks for each pattern.
   - Record Debug and Release times in a similar table for each pattern.

## Before optimization
This section reports the performance characteristics of the Fireworks library before any optimization. 
Benchmarking and profiling were performed using the console benchmark application and the Visual Studio CPU Usage performance tool.

### Benchmark Timing Results
All tests were run with:
- Screen size: 800 × 600
- Batch runs: 100
- Firework types: circle pattern and default pattern

**Debug Mode Results:**
100 fireworks: 12.43 ms

1,000 fireworks: 139.77 ms

10,000 fireworks (circle): 1682.94 ms

10,000 fireworks (default): ~29,000 ms

**Release Mode Results:**

100 fireworks: 3.91 ms

1,000 fireworks: 18.75 ms

10,000 fireworks (circle): 456.91 ms

10,000 fireworks (default): ~29,000 ms

The default pattern takes dramatically longer than the circle pattern—approximately 65× slower in Release mode—so it requires profiling to understand the cause.

### CPU Profiler Results

Visual Studio’s CPU profiler identifies the following hot functions:

- ShapesFactory.CreateCircle(...): 32.93% of total CPU time. Circle pattern generation is expensive because it constructs many vertices.
- Vector.op_Addition(...): 21.59% of CPU time. Called millions of times during particle movement.
- Particle.Update(): 24.74% of CPU time. This is the core per-frame update for each explosion particle.
- Firework.Update(): 86.82% inclusive CPU time, since it calls particle updates and explosion logic.
- Vector.set_x(float): 6.12% of CPU time, due to excessive property setter usage inside tight loops.

**Hot Path Summary:**
Program.Main → Benchmark → FireworkEnvironment.Update → Firework.Update → Particle.Update → (Circle creation + Vector math)

**Summary of Findings**
- The majority of CPU time is spent updating particles every frame.
- The default explosion pattern is extremely slow because it generates many particles and performs heavy vector math.
- Shape creation (especially circles) is a major cost because of unnecessary repeated work.

## After optimization
This section reports the performance characteristics of the Fireworks library after applying optimizations to the benchmark and core library code. 
**Optimizations** included:
- Removing the extra AllFireworksDone scan and relying on FireworkEnvironment.Update to prune finished fireworks (while (env.Items.Count > 0)).
- Parallelizing per-firework updates in FireworkEnvironment.Update using Parallel.For, then doing a sequential removal pass.
- Refactoring Firework.Update so that particle updates and removals are separated (update loop + RemoveAll(p => p.Done)), with optional parallel updates for “large” explosions.

### Benchmark Timing Results (Debug mode)
All tests use:
- Screen size: 800 × 600
- Batch runs: 100
- Firework types: circle pattern and default pattern

Approximate average times from the debug runs:
- 100 circle fireworks: ≈ 15 ms (similar to before – very small workload, dominated by overhead)
- 100 default fireworks: ≈ 49 ms (similar to before)
- 1,000 circle fireworks: ≈ 52 ms (similar to before)
- 1,000 default fireworks: ≈ 500 ms (similar to before)
- 10,000 circle fireworks: ≈ 530 ms (improved from 1,682.94 ms → roughly 3× faster)
- 10,000 default fireworks: ≈ 13,200 ms (improved from ~29,000 ms → roughly 2.2× faster)

The optimizations don’t change much for small batches (100 / 1,000 fireworks), where the work per frame is relatively small. 
They however provide significant gains for the heavy workloads (10,000 fireworks), which are closer to worst-case stress tests.

### CPU Profiler Results
- FireworkEnvironment.Update.AnonymousMethod__5 (the body of the Parallel.For) now appears in the hot path, confirming that per-firework updates are running concurrently on multiple cores.
- Fireworks.Particle.Update() remains the main CPU consumer, as expected, because it is the core per-frame particle simulation.
- ShapesFactory.CreateCircle(...) and Vector.op_Addition(...) are still expensive, but their relative impact is lower once the work is spread across cores.
- Overall CPU usage shows multiple cores busy during the heaviest parts of the benchmark, rather than a mostly single-threaded profile.