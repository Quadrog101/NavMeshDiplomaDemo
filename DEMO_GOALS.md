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
  - River / Ford;
  - Road;
  - Not Walkable.
- Route choice can also be explained through approximate resource metrics:
  - distance;
  - time;
  - water;
  - food;
  - stamina;
  - risk;
  - oasis bonus.
- Different unit profiles can evaluate the same terrain differently through per-agent area costs.
- A `NavMeshObstacle` with carving can block a canyon gate and trigger real-time replanning.
- Local avoidance is separate from global pathfinding and is visible when several agents move together.

## Current Demonstration Modes

- `1 Shortest Path`: effective cost is 1, central desert shortcut is preferred.
- `2 Resource-weighted`: Hazard cost is 18, terrain/resource penalties make the route move to Oasis/Road bypass.
- `3 Dynamic Obstacle`: Canyon Gate is carved into the NavMesh and toggles during movement, causing replanning.
- `4 Multi-Profile`: Scout, Carrier, and Nomad move together with different terrain preferences.

## Limits

- No custom A*.
- No behavior tree.
- No procedural generation.
- No large asset system.
- No combat/RTS logic.
