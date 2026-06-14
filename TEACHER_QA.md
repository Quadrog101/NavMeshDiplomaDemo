# TEACHER_QA.md

## Where Is The NavMesh?

The NavMesh is built at runtime by `NavMeshSurface` from the scene colliders. It represents the walkable parts of the test map.

## Where Is The Graph?

Unity does not expose the NavMesh as a simple hand-written graph in the scene. Conceptually, the NavMesh polygons/regions are graph nodes, their adjacency is graph connectivity, and area costs are weights used during path search.

## What Are Vertices, Edges, And Weights Here?

- Vertices/nodes: NavMesh polygons or regions used internally by Unity.
- Edges: neighboring walkable connections between those regions.
- Weights: traversal distance combined with area cost, such as `Expensive` or `Mud`.

## How Does The NPC Receive A Goal?

Each NPC has a `NavMeshAgent`. The demo assigns a destination point inside the finish zone and calls `SetDestination`.

## Why Does The Route Change When Area Cost Changes?

The central shortcut is geometrically short. When its `Expensive` cost is low, it is preferred. When its cost is high, a longer bypass can have lower total weighted cost.

## Global Pathfinding Vs Local Avoidance

Global pathfinding chooses the overall route on the NavMesh. Local avoidance adjusts agent movement while following the route, especially when several agents move together.

## How Does The Dynamic Obstacle Work?

The obstacle uses `NavMeshObstacle` with carving. It changes the navigable area so the shortcut becomes blocked. The collider is a trigger, so it does not physically push NPCs.

## Current Demo Limitations

- It uses Unity NavMesh rather than a custom A* implementation.
- It is a controlled test map, not a full game level.
- Metrics are simple runtime observations, not exported experiment datasets.
- Area names and costs are configured for demonstration clarity.
