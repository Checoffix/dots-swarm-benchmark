<div align="center">

# Horde Swarm — DOTS Benchmark

**Mass-entity simulation in Unity DOTS: spatial-hash separation, Burst jobs, EntityCommandBuffer.**<br/>
<sub>My first DOTS project, documented as a measurement log: change → profiler → result.</sub>

<br/>

![Unity](https://img.shields.io/badge/Unity-6000.6.3f1-1f2328?style=flat-square&logo=unity&logoColor=white)
![Entities](https://img.shields.io/badge/Entities-6.6.0-1f2328?style=flat-square)
![Burst](https://img.shields.io/badge/Burst-Jobs-1f2328?style=flat-square)
![Platform](https://img.shields.io/badge/Platform-PC%20Standalone-1f2328?style=flat-square)

</div>

---

## Demo

> *Demo capture coming soon (Standalone Player build, 100k entities).*

## TL;DR

- **What:** High-performance flocking and horde simulation in Unity 6 DOTS (Entities 1.0, Burst, URP BatchRendererGroup) utilizing a custom spatial hash grid, position-based dynamics (PBD) separation, and zero-allocation continuous spawn/destroy cycles.
- **Study 1 (swarm):** Scaled from 10k to 100k dynamic entities on an Intel Core i5-8400. 100k entities achieve a median frame time of **11.23 ms (~89.1 FPS)** in Render mode (**8.18 ms** No-Render) with 2D spatial hashing, maintaining **strictly 0 B GC allocations** and a rock-solid **3.1 MB managed heap**.
- **Study 2 (straight-line movement):** Controlled baseline comparing the computational cost of moving 10k / 50k / 100k objects across four architectural approaches (Classic GameObjects, Job System Transforms, DOTS `IJobEntity`, and hierarchical parent translation).
- **How it was measured:** Automated standalone player builds with rendering enabled and disabled, 10s warm-up, and 30s sampling window logged to CSV (see [Methodology](#methodology)).

> [!IMPORTANT]
> **Scope of the comparison.** This is a personal learning project.
> - **Study 2** is the controlled one: identical behaviour (move along Z at equal speed), so the numbers compare *mechanisms* for moving N objects.
> - **Study 1** compares *approaches to a task* (a swarm converging without stacking). Box2D's contact solver and my hash-based separation are different mechanisms, so the numbers answer "what does each approach cost for this task", **not** "how many times is ECS faster than GameObjects in general".

## Key metrics

**Render** build, frame time in ms as **median / p99**. Full tables (including No-Render) are in the studies below.

**Study 1 — swarm to target**

| Variant | 10 000 | 50 000 | 100 000 |
|:--|:--:|:--:|:--:|
| A: GO + Box2D | 13.60 / 18.40 | n/m | n/m |
| B: DOTS, 3D scan | 2.74 / 4.63 | 15.99 / 21.47 | 35.68 / 43.45 |
| C: DOTS, 2D scan | 1.99 / 4.86 | 5.51 / 7.65 | 11.23 / 14.81 |

**Study 2 — straight-line movement**

| Variant | 10 000 | 50 000 | 100 000 |
|:--|:--:|:--:|:--:|
| 1: GO, Update | TBD / TBD | TBD / TBD | TBD / TBD |
| 2: GO, Jobs | TBD / TBD | TBD / TBD | TBD / TBD |
| 3: GO, move common parent | TBD / TBD | TBD / TBD | TBD / TBD |
| 4: DOTS, IJobEntity | TBD / TBD | TBD / TBD | TBD / TBD |

---

## Methodology

> [!NOTE]
> **Render / No-Render.** Every configuration is built and measured twice. **Render** is the full frame (what the player sees). **No-Render** has the renderers disabled and isolates simulation and transform cost from draw cost, which differs between GameObjects (`MeshRenderer`) and Entities Graphics.

| Item | Value |
|:--|:--|
| Hardware | Intel Core i5-8400 (6C / 6T @ 2.80 GHz) / Discrete GPU / 16 GB DDR4 |
| Unity / Entities | 6000.6.3f1 / 6.6.0 (Entities Graphics, URP 17.6.0) |
| Scene | Orthographic camera, unlit cube mesh, zero Post-Processing overhead |
| Spawn | Uniform distribution across XY plane |
| Warm-up / sample window | 10 s warm-up / 30 s sampling window (`FrameTimeLogger`) |
| Runs per cell | 30 s continuous window (~800 – 14,000+ logged frames per run) |
| Frame time | per-frame duration (`Time.unscaledDeltaTime`) logged to CSV, see `Assets/Scripts/FrameTimeLogger.cs` |
| Median | 50th percentile of per-frame times |
| p99 | 99th percentile of per-frame times (Excel `PERCENTILE.INC`): 99% of frames are faster than this value; see `tools/analyze_frametimes.py` |
| Other metrics | GC alloc (Profile Analyzer), Native Memory / Managed Heap (Memory Profiler) |

---

# Study 1 — Swarm to target (separation)

## Variants

| ID | Variant | Movement to target | Separation / neighbour search |
|:--:|:--|:--|:--|
| **A** | GameObjects | `Rigidbody2D` + `BoxCollider2D`, velocity toward target | Box2D contact solver |
| **B** | DOTS, 3D scan | Burst `MoveJob` | `NativeParallelMultiHashMap` hash, **27** neighbour cells |
| **C** | DOTS, 2D scan | Burst `MoveJob` | same hash, **9** neighbour cells (motion is planar, Z = 0) |

<details>
<summary><b>Simulation parameters</b></summary>

| Parameter | Value |
|:--|:--|
| `CellSize` | 1.1 |
| `MinSeparationRadius` | 1.1 |
| `MaxNeighboursCount` | 64 |
| Kill aura radius | 5 |

</details>

## Results

"n/m" means not measured, with the reason. Memory is reported as **Native Memory / Managed Heap** (in-use unmanaged memory vs managed C# GC heap).

|      Variant       | Entities |    Render: median / p99     | No-Render: median / p99 | Simulation time | GC alloc | Memory |
| :----------------: | :------: | :-------------------------: | :---------------------: | :-------------: | :------: | :----: |
|  **A** GO + Box2D  |  10 000  |        13.60 / 18.40        |      12.22 / 20.00      |       30        |  236 B   | 256 MB / 5.2 MB |
|  **A** GO + Box2D  |  50 000  |  n/m: 10k already 13.6 ms   |            –            |        –        |    –     |   –    |
|  **A** GO + Box2D  | 100 000  |  n/m: 10k already 13.6 ms   |            –            |        –        |    –     |   –    |
| **B** DOTS 3D scan |  10 000  |         2.74 / 4.63         |       2.13 / 4.43       |       30        |   0 B    | 260 MB / 3.1 MB |
| **B** DOTS 3D scan |  50 000  |        15.99 / 21.47        |      14.40 / 17.45      |       30        |   0 B    | 272 MB / 3.1 MB |
| **B** DOTS 3D scan | 100 000  |        35.68 / 43.45        |      33.26 / 40.91      |       30        |   0 B    | 289 MB / 3.1 MB |
| **C** DOTS 2D scan |  10 000  |         1.99 / 4.86         |       1.64 / 4.22       |       30        |   0 B    | 260 MB / 3.1 MB |
| **C** DOTS 2D scan |  50 000  |         5.51 / 7.65         |       4.09 / 5.01       |       30        |   0 B    | 272 MB / 3.1 MB |
| **C** DOTS 2D scan | 100 000  |        11.23 / 14.81        |       8.18 / 9.61       |       30        |   0 B    | 289 MB / 3.1 MB |

### Profiler captures

| Frame breakdown (GO, 10k) | Memory breakdown (GO, 10k) |
|:--:|:--:|
| <img src="docs/profiler-frame-go-10k.png" alt="Profile Analyzer GO 10k" width="420"/> | <img src="docs/memory-breakdown-go-10k.png" alt="Memory breakdown GO 10k" width="420"/> |
| <sub>10k GameObjects: Median 13.6 ms (~73 FPS), GC Collect 1.75 ms</sub> | <sub>Managed Heap: 5.2 MB, Native: 256.5 MB, 10 007 GameObjects (70 184 Scene Objects)</sub> |

| Frame breakdown (DOTS, 10k) | Memory breakdown (DOTS, 10k) |
|:--:|:--:|
| <img src="docs/profiler-frame-10k.png" alt="Profile Analyzer 10k" width="420"/> | <img src="docs/memory-breakdown-10k.png" alt="Memory breakdown 10k" width="420"/> |
| <sub>10k entities: Median 2.7 ms (~370 FPS), zero GC Collect</sub> | <sub>Managed Heap: 3.1 MB, Native: 260.1 MB, 7 GameObjects total</sub> |

| Frame breakdown (DOTS, 50k) | Memory breakdown (DOTS, 50k) |
|:--:|:--:|
| <img src="docs/profiler-frame-50k.png" alt="Profile Analyzer 50k" width="420"/> | <img src="docs/memory-breakdown-50k.png" alt="Memory breakdown 50k" width="420"/> |
| <sub>50k entities: Median 7.4 ms (~135 FPS), zero GC Collect</sub> | <sub>Managed Heap: 3.1 MB, Native: 271.8 MB, 7 GameObjects total</sub> |

| Frame breakdown (DOTS, 100k) | Memory breakdown (DOTS, 100k) |
|:--:|:--:|
| <img src="docs/profiler-frame-100k.png" alt="Profile Analyzer 100k" width="420"/> | <img src="docs/memory-breakdown-100k.png" alt="Memory breakdown 100k" width="420"/> |
| <sub>100k entities: Median 15.0 ms (~67 FPS), zero GC Collect</sub> | <sub>Managed Heap: 3.1 MB, Native: 288.8 MB, 7 GameObjects total</sub> |

## Architecture

Order of work inside one frame (DOTS variants):

```mermaid
flowchart LR
    subgraph SIM["SimulationSystemGroup"]
        direction LR
        A["BuildSpatialHashJob<br/>rebuild hash"] --> B["MoveJob<br/>attraction + separation<br/>27 cells (3D) / 9 cells (2D)"]
        B --> C["KillAuraJob<br/>distance² ≤ R²"]
    end
    C --> D["EndSimulation ECB<br/>playback: DestroyEntity"]
    D --> E["Respawn<br/>keep population constant"]

    classDef job fill:#161b22,stroke:#58a6ff,color:#c9d1d9
    classDef ecb fill:#161b22,stroke:#f78166,color:#c9d1d9
    classDef spawn fill:#161b22,stroke:#3fb950,color:#c9d1d9
    class A,B,C job
    class D ecb
    class E spawn
```

The simulation loop executes sequentially within `SimulationSystemGroup`. `BuildSpatialHashJob` populates the grid keys in parallel; `MoveJob` consumes this hash to compute flocking attraction and neighbor repulsion forces. Subsequently, `KillAuraJob` schedules entity destruction into `EndSimulationEntityCommandBufferSystem`, which safely executes structural changes at the frame sync point before respawning maintains population equilibrium.

---

# Study 2 — Straight-line movement of N objects

Identical behaviour in all variants: every cube moves along the **Z axis** at the **same speed**. This isolates the cost of the movement mechanism itself.

## Variants

| ID | Variant | How the objects move |
|:--:|:--|:--|
| **1** | GameObjects, `Update` | each cube has a `MonoBehaviour` that moves itself every frame |
| **2** | GameObjects, Jobs | all transforms in a `TransformAccessArray`, moved by one `IJobParallelForTransform` scheduled from a single `Update` |
| **3** | GameObjects, common parent | all cubes are children of one empty parent; only the parent is moved (one `Transform` write per frame), children are static in local space |
| **4** | DOTS, `IJobEntity` | Burst `IJobEntity` over entities (`ScheduleParallel`) |

Variant 3 is compared against variants 1 and 2, where the cubes have no common parent.

> [!NOTE]
> A plain `IJobParallelFor` cannot touch a `Transform`; moving GameObjects from a job needs `IJobParallelForTransform` with a `TransformAccessArray`.

<details>
<summary><b>Controls kept equal across variants</b></summary>

- Same speed, same `deltaTime` usage; the sample window is time-based, so every variant runs under equal duration.
- Variants 1, 2, and 4: all GameObjects/entities are flat (no parents), the best case for `TransformAccessArray`. Variant 3: one empty parent for all cubes.
- Same mesh, material and camera; SRP Batcher / GPU instancing set the same way — TBD.
- Worker thread count recorded: TBD.
- Cubes are not wrapped around; the warm-up plus window is short enough that they stay in the camera view in the Render build.

</details>

## Results

| Variant | Entities | Render: median / p99 | No-Render: median / p99 | Movement cost | GC alloc | Memory |
|:--:|:--:|:--:|:--:|:--:|:--:|:--:|
| **1** GO, Update | 10 000 | TBD / TBD | TBD / TBD | TBD | TBD | TBD |
| **1** GO, Update | 50 000 | TBD / TBD | TBD / TBD | TBD | TBD | TBD |
| **1** GO, Update | 100 000 | TBD / TBD | TBD / TBD | TBD | TBD | TBD |
| **2** GO, Jobs | 10 000 | TBD / TBD | TBD / TBD | TBD | TBD | TBD |
| **2** GO, Jobs | 50 000 | TBD / TBD | TBD / TBD | TBD | TBD | TBD |
| **2** GO, Jobs | 100 000 | TBD / TBD | TBD / TBD | TBD | TBD | TBD |
| **3** GO, common parent | 10 000 | TBD / TBD | TBD / TBD | TBD | TBD | TBD |
| **3** GO, common parent | 50 000 | TBD / TBD | TBD / TBD | TBD | TBD | TBD |
| **3** GO, common parent | 100 000 | TBD / TBD | TBD / TBD | TBD | TBD | TBD |
| **4** DOTS, IJobEntity | 10 000 | TBD / TBD | TBD / TBD | TBD | TBD | TBD |
| **4** DOTS, IJobEntity | 50 000 | TBD / TBD | TBD / TBD | TBD | TBD | TBD |
| **4** DOTS, IJobEntity | 100 000 | TBD / TBD | TBD / TBD | TBD | TBD | TBD |

> [!NOTE]
> Variant 3 isolates the cost of hierarchy updates by translating a single root parent instead of N individual entity transforms.

<details>
<summary><b>Raw data</b> (CSV frame times, Profile Analyzer exports, run logs)</summary>

Per-frame CSV logs are recorded via `FrameTimeLogger` into `Application.persistentDataPath` to measure median frame time and p99 spikes (1% lows).

</details>

---

> [!IMPORTANT]
> **Limitations and scope:**
> - Study 1 compares distinct architectural paradigms for swarm separation (Box2D iterative solver vs custom DOTS Spatial Hash).
> - Measurements reflect a single reference hardware configuration (Intel Core i5-8400, PC Standalone, D3D12).
> - Classic GameObjects were not measured at 50k and 100k in Study 1 because 10k entities already exceeded the 16.6 ms frame budget (13.6 ms simulation + GC spikes).

<details>
<summary><b>Design decisions</b></summary>

- **Per-frame Spatial Hash rebuild:** Rebuilding the `NativeParallelMultiHashMap` from scratch every frame in parallel via `BuildSpatialHashJob` proved faster and simpler than tracking incremental cell migrations for 100k constantly moving entities.
- **Structural changes via ECB:** Spawning and despawning are deferred to `EndSimulationEntityCommandBufferSystem` to prevent job pipeline stalls and keep parallel worker threads saturated.
- **2D vs 3D Hash Search:** Reducing neighbor checks from 27 cells to 9 planar cells (Z = 0) reduced 100k simulation frame time from 35.68 ms to 11.23 ms (~3.2x speedup).

</details>

---

## Quick start

**Requirements:** Unity `6000.6.3f1`, Entities `6.6.0` (check `Packages/manifest.json`).

```bash
git clone https://github.com/Checoffix/dots-swarm-benchmark.git
```

1. Open the project in Unity Hub.
2. Open the scene for the study you want to run:
   - **Study 1 (DOTS):** `Assets/Scenes/Study1/DOTS_Scene.unity` (with subscene `HordeSubScene.unity`)
   - **Study 1 (GameObjects):** `Assets/Scenes/Study1/GO_Scene.unity`
   - **Study 2 (Movement):** `Assets/Scenes/Study2/` (coming soon)
3. **Play** to run in Editor, or **File → Build Settings → Build** for standalone benchmarking with `FrameTimeLogger`.

## About

First DOTS project by [Checoffix](https://github.com/Checoffix). Written as a learning case: every number above comes from my own measurements, and the log shows the mistakes too.
