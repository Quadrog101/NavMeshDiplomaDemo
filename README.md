# NavMeshDiplomaDemo

Unity demo for the practical part of the master's topic:

`Research of NPC navigation algorithms in game scenes with route costs and dynamic obstacles`.

The project is intentionally small. It is a teaching and discussion demo, not a full diploma implementation.

## What The Demo Shows

- Unity `NavMeshSurface` builds a walkable navigation representation from scene geometry.
- `NavMeshAgent` receives a target inside a finish zone and builds a route.
- Area costs change route choice.
- `NavMeshObstacle` with carving changes the route at runtime.
- Several NPC agents can move to one wide finish zone using separate destination points.
- Global pathfinding and local avoidance are visible as separate ideas.

## How To Run

1. Open the project in Unity.
2. Open `Assets/Scenes/NavMeshDemo.unity`.
3. Press Play.

The scene contains a bootstrap object. In Play Mode it creates the test map, builds a `NavMeshSurface`, creates NPC agents, surface-cost zones, the finish zone, and the dynamic obstacle.

To recreate the bootstrap scene from Unity, use:

`Tools/NavMesh Diploma Demo/Rebuild Bootstrap Scene`

## Controls

- `1` - shortest path mode.
- `2` - weighted cost mode.
- `3` - dynamic obstacle mode.
- `4` - multi-agent mode.
- `Space` - start active agents.
- `R` - reset the current experiment.
- `C` - toggle the central `Expensive` cost.
- `O` - toggle the dynamic obstacle during the run; the scene is not fully reset, and the path is recalculated after a one-frame carving delay.
- `N` - toggle one NPC / several NPCs.

## Modes

### 1. Shortest Path

- `Expensive` cost is low.
- Obstacle is off.
- Expected route: `cost=1 -> central shortcut`.
- Conclusion: with equal costs, the agent prefers the geometrically shorter route.

### 2. Weighted Cost

- The central shortcut uses `Expensive` cost 18.
- Obstacle is off.
- Expected route: `cost=18 -> long bypass`.
- Conclusion: a longer route can become preferable when the short route crosses a high-cost area.

### 3. Dynamic Obstacle

- Obstacle is on and blocks the shortcut.
- The obstacle collider is a trigger, so it does not physically push the NPC.
- `NavMeshObstacle` uses carving.
- Pressing `O` during movement toggles the obstacle and demonstrates real-time replanning.
- Conclusion: dynamic carving changes the navigable space and the agent replans around it.

### 4. Multi-Agent

- Several NPC agents are active.
- Each agent receives a separate destination point inside the wide finish zone.
- Conclusion: a finish zone is more stable than forcing all NPCs into one tiny target point.

## Scene Legend

- Blue area: start zone.
- Green area: finish zone.
- Orange area: `Expensive` / dangerous central shortcut.
- Brown area: `Mud` / slow surface.
- Gray area: `Road` / preferred surface.
- Light-blue strips: bypass routes.
- Red cube: dynamic obstacle.
- Yellow line: current NavMesh path.
- Dark walls/islands: blocked geometry / not walkable space.

## HUD Metrics

The HUD shows:

- current mode;
- expected route;
- actual route;
- path status: `Complete`, `Partial`, or `Invalid`;
- repaths for the first active agent;
- active/reached agents;
- path length;
- travel time;
- FPS;
- area costs: Normal, Mud, Road, Expensive;
- obstacle on/off;
- short conclusion for the current mode.

## Supporting Markdown Files

- `AGENTS.md` - working rules for future Codex changes.
- `DEMO_GOALS.md` - what the demo must prove.
- `DEMO_CHECKLIST.md` - checklist before showing the demo to the teacher.
- `TEACHER_QA.md` - short answers to likely teacher questions.
- `CODEX_TASK_TEMPLATE.md` - template for future short Codex tasks.

For future work, use `CODEX_TASK_TEMPLATE.md` and describe only the small change needed, what not to touch, and how to verify it in Unity.

## Main Project Files

- `Assets/Scenes/NavMeshDemo.unity` - demo entry scene.
- `Assets/Scripts/NavMeshDemoBootstrap.cs` - creates the runtime test map.
- `Assets/Scripts/NavMeshDemoController.cs` - modes, controls, HUD, metrics.
- `Assets/Scripts/NavMeshDemoAgent.cs` - NPC movement and finish timing.
- `Assets/Scripts/NavMeshDemoFinishZone.cs` - wide finish zone and per-agent destination points.
- `Assets/Scripts/NavMeshDemoPathRenderer.cs` - path visualization through `LineRenderer`.
- `Assets/Editor/NavMeshDemoSceneBuilder.cs` - recreates the bootstrap scene from the Unity menu.
