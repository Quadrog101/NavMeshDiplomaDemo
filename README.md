# NavMeshDiplomaDemo

Unity research prototype for the master's topic:

`Research of NPC navigation algorithms in game scenes with route costs and dynamic obstacles`.

The project is still compact. It does not implement custom A*, behavior trees, combat, procedural generation, or a full game. It uses Unity AI Navigation to make the core ideas visible.

## What The Demo Shows

- A larger `Desert Route Test` map with a start base on the left and a finish camp on the right.
- Several distinct routes between start and finish:
  - `North Road`: long, safe, cheap.
  - `Central Dunes`: short, but costly because of dunes and heat hazard.
  - `South Oasis Path`: medium length, mixed terrain, often attractive for some units.
  - `Canyon Gate`: a narrow route that can be blocked by a dynamic obstacle.
- Real `NavMeshModifierVolume` areas, not just decorative colors.
- Different NPC profiles that evaluate the same terrain differently.
- Runtime obstacle carving and delayed path replanning without a full scene reset.
- Global pathfinding and local avoidance in multi-agent mode.

## How To Run

1. Open the project in Unity.
2. Open `Assets/Scenes/NavMeshDemo.unity`.
3. Press Play.

The scene contains a bootstrap object. In Play Mode it creates the desert test map, builds a `NavMeshSurface`, creates terrain-cost zones, NPC profiles, the finish camp, and the dynamic canyon obstacle.

To recreate the bootstrap scene from Unity, use:

`Tools/NavMesh Diploma Demo/Rebuild Bootstrap Scene`

## Controls

- `1` - shortest path mode.
- `2` - weighted terrain mode.
- `3` - dynamic obstacle mode.
- `4` - multi-agent / multi-class mode.
- `Space` - start active agents.
- `R` - reset the current experiment.
- `C` - toggle high/low hazard terrain cost.
- `O` - toggle the canyon obstacle during the run; the scene is not reset.
- `N` - toggle one NPC / several NPCs.

## Terrain Areas

- `Road` cost 1: long safe route.
- `PackedSand` cost 2: moderate connector terrain.
- `DeepSand` cost 4: central dunes.
- `Rock` cost 3: rough route segment.
- `Hazard` cost 7 in weighted mode, cost 1 in shortest mode: heat/danger section.
- `Oasis` cost 1.2: safer southern route.
- `Not Walkable`: walls, rock islands, and map borders.

## Unit Profiles

- `Scout`: fast, small radius, handles sand/dunes better.
- `Carrier`: slower, larger, prefers roads and dislikes dunes/hazard.
- `Ranger`: balanced profile.

The profiles use `NavMeshAgent` speed/radius/acceleration and per-agent `SetAreaCost` values. In multi-agent mode they can choose different routes on the same map.

## Modes

### 1. Shortest Path

- One agent.
- Hazard cost is low.
- Expected route: central dunes / short route.
- Demonstrates geometric route choice when costs do not strongly penalize the shortcut.

### 2. Weighted Terrain

- One agent.
- Hazard and dunes are costly.
- Expected route: North Road or South Oasis instead of the central shortcut.
- Demonstrates route weights changing global pathfinding.

### 3. Dynamic Obstacle

- One agent.
- The canyon gate can be blocked/unblocked with `O` during movement.
- The obstacle uses `NavMeshObstacle` with carving and a trigger collider, so it does not physically push the NPC.
- Demonstrates real-time replanning after a one-frame carving delay.

### 4. Multi-Agent / Multi-Class

- Scout, Carrier, and Ranger are active.
- Agents have different terrain preferences and separate finish points.
- Demonstrates global pathfinding differences plus local avoidance between agents.

## HUD Metrics

The HUD shows:

- mode;
- active unit profiles;
- terrain costs;
- expected route;
- actual route;
- per-agent route summary;
- path status: `Complete`, `Partial`, or `Invalid`;
- repaths;
- path length;
- travel time;
- obstacle state;
- active/reached agents;
- FPS;
- short conclusion.

## Supporting Markdown Files

- `AGENTS.md` - working rules for future Codex changes.
- `DEMO_GOALS.md` - what the demo must prove.
- `DEMO_CHECKLIST.md` - checklist before showing the demo to the teacher.
- `TEACHER_QA.md` - short answers to likely teacher questions.
- `CODEX_TASK_TEMPLATE.md` - template for future short Codex tasks.

For future work, use `CODEX_TASK_TEMPLATE.md` and describe only the small change needed, what not to touch, and how to verify it in Unity.

## Main Project Files

- `Assets/Scenes/NavMeshDemo.unity` - demo entry scene.
- `Assets/Scripts/NavMeshDemoBootstrap.cs` - creates the runtime desert test map.
- `Assets/Scripts/NavMeshDemoController.cs` - modes, controls, HUD, metrics.
- `Assets/Scripts/NavMeshDemoAgent.cs` - NPC profiles, movement, route description, finish timing.
- `Assets/Scripts/NavMeshDemoFinishZone.cs` - wide finish zone and per-agent destination points.
- `Assets/Scripts/NavMeshDemoPathRenderer.cs` - path visualization through `LineRenderer`.
- `Assets/Editor/NavMeshDemoSceneBuilder.cs` - recreates the bootstrap scene from the Unity menu.
