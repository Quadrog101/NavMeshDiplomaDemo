# DEMO_GOALS.md

The demo must prove a small set of navigation ideas without becoming a full diploma project.

## What The Demo Should Show

- How Unity `NavMeshSurface` creates a walkable navigation representation from scene geometry.
- How NavMesh can be explained as a graph-like structure: polygons/regions act like nodes, neighbor connections act like edges, and area costs act like weights.
- How an NPC receives a goal through `NavMeshAgent.SetDestination`.
- How real-time pathfinding updates the selected route when costs or obstacles change.
- How a high-cost area changes route choice even when that route is geometrically shorter.
- How `NavMeshObstacle` with carving changes the navigable space for a dynamic obstacle.
- How global pathfinding differs from local avoidance:
  - global pathfinding chooses the route across the NavMesh;
  - local avoidance helps agents not collide while following a route.
- How multiple NPC agents can move to one finish zone using separate destination points.

## What The Demo Should Not Become

- no custom A* implementation;
- no behavior tree;
- no combat/gameplay system;
- no procedural level generator;
- no large art pass or third-party assets.
