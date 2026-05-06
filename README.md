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

Inspired by [this gravity simulation video][grav-video] and its [reference implementation on GitHub][grav-repo].

Controls: `K` to pause, `Q` to quit, `Escape` to toggle cursor capture.

Without the grid:

![Without grid](GravitySimulation/PREVIEW/preview_without_grid.webp)

With the spacetime grid:

![With grid](GravitySimulation/PREVIEW/preview_with_grid.webp)

### AtomSimulation

A visualisation of hydrogen atom orbitals. The electron is drawn as a cloud of points: where the cloud is dense, the electron is more likely to be found. The shape of the cloud is controlled by three quantum numbers `(n, l, m)`, which can be changed at runtime. Each point is coloured by an inferno heatmap based on local density, and the whole cloud slowly rotates around the polar axis to give a sense of the quantum probability flow.

Inspired by [this atomic orbital simulation video][atom-video] and its [reference implementation on GitHub][atom-repo].

Controls: arrow `Up`/`Down` change `n`, `Left`/`Right` change `l`, `[`/`]` change `m`, `-`/`=` change particle count, `K` to pause, `Q` to quit, `Escape` to toggle cursor capture.

![Orbital 1](AtomSimulation/PREVIEW/preview1.webp)

![Orbital 2](AtomSimulation/PREVIEW/preview2.webp)

![Orbital 3](AtomSimulation/PREVIEW/preview3.webp)

### Building and running

Requires the .NET 10 SDK.

```bash
git clone https://github.com/d4nilevi4/StaticEngine.git
cd StaticEngine
dotnet build
dotnet run --project GravitySimulation
dotnet run --project AtomSimulation
```

[bevy-link]: https://github.com/bevyengine/bevy
[staticecs-link]: https://github.com/Felid-Force-Studios/StaticEcs
[raylib-link]: https://github.com/raylib-cs/raylib-cs
[grav-video]: https://www.youtube.com/watch?v=_YbGWoUaZg0
[grav-repo]: https://github.com/kavan010/gravity_sim
[atom-video]: https://www.youtube.com/watch?v=OSAOh4L41Wg
[atom-repo]: https://github.com/kavan010/Atoms
