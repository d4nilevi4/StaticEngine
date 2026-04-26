# StaticEngine

An experimental game engine written in C#, inspired by [Bevy Engine][bevy-link]. All state lives in the ECS world and all logic runs in systems. No god objects, no inheritance hierarchies for game logic.

[Raylib-cs][raylib-link] handles windowing, input, and rendering. The ECS framework is [StaticECS][staticecs-link]. Math primitives live in the `Mathematics` project.

## Stack

- .NET 10
- [StaticECS][staticecs-link] for ECS
- [Raylib-cs][raylib-link] for windowing, input, and rendering

## Examples

### GravitySimulation

An N-body gravity simulation. Bodies attract each other under Newtonian gravity, computed as a brute-force O(N²) pairwise sum. The force kernel is parallelised across worker threads and vectorised with SIMD over a Structure-of-Arrays body buffer, so every inner-loop step accumulates contributions from a full SIMD vector of bodies at once. An optional spacetime grid bends under the masses; it is a visual metaphor, not actual general relativity.

Controls: `K` to pause, `Q` to quit, `Escape` to toggle cursor capture.

Without the grid:

![Without grid](GravitySimulation/PREVIEW/preview_without_grid.webp)

With the spacetime grid:

![With grid](GravitySimulation/PREVIEW/preview_with_grid.webp)

### Building and running

Requires the .NET 10 SDK.

```bash
git clone https://github.com/d4nilevi4/StaticEngine.git
cd StaticEngine
dotnet build
dotnet run --project GravitySimulation
```

[bevy-link]: https://github.com/bevyengine/bevy
[staticecs-link]: https://github.com/Felid-Force-Studios/StaticEcs
[raylib-link]: https://github.com/raylib-cs/raylib-cs
