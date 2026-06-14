# DEMO_GOALS.md

The demo should be a compact research prototype for NPC navigation, not a full diploma project.

## What The Demo Must Prove

- Unity `NavMeshSurface` creates a walkable navigation representation from scene geometry.
- The NavMesh can be explained as a graph-like structure:
  - polygons/regions are nodes;
  - neighbor connections are edges;
  - area costs are weights.
- NPCs receive goals through `NavMeshAgent.SetDestination`.
- Global pathfinding selects a route based on distance and area costs.
- Different terrain surfaces can influence route selection:
  - Road;
  - PackedSand;
  - DeepSand / Dunes;
  - Rock / Rough;
  - Hazard / Heat;
  - Oasis / SafePath;
  - Not Walkable.
- Different unit profiles can evaluate the same terrain differently through per-agent area costs.
- A `NavMeshObstacle` with carving can block a canyon gate and trigger real-time replanning.
- Local avoidance is separate from global pathfinding and is visible when several agents move together.

## Current Demonstration Modes

- `1 Shortest Path`: low penalties, central shortcut is preferred.
- `2 Weighted Terrain`: high terrain penalties, a safer longer route is preferred.
- `3 Dynamic Obstacle`: canyon gate toggles during movement and causes replanning.
- `4 Multi-Agent / Multi-Class`: Scout, Carrier, and Ranger move together with different preferences.

## Limits

- No custom A*.
- No behavior tree.
- No procedural generation.
- No large asset system.
- No combat/RTS logic.
