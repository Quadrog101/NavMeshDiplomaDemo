# TEACHER_QA.md

## Where Is The NavMesh?

The NavMesh is built at runtime by `NavMeshSurface` from the colliders on the Desert Route Test map. It represents walkable areas between the start base and the finish camp.

## Where Is The Graph?

Unity does not show a simple explicit graph in the scene. Conceptually, NavMesh polygons/regions are graph nodes, adjacency between polygons is graph connectivity, and terrain costs are edge/area weights.

## What Are Vertices, Edges, And Weights Here?

- Nodes: NavMesh polygons or regions.
- Edges: possible transitions between neighboring regions.
- Weights: distance multiplied or biased by area costs such as Road, DeepSand, Hazard, Oasis, and Rock.

## How Does The NPC Receive A Goal?

Each NPC has a `NavMeshAgent`. The controller assigns a point inside the finish camp and calls `SetDestination`.

## Why Does The Route Change In Weighted Terrain Mode?

The central route is shorter, but it crosses DeepSand and Hazard areas. When those costs are high, a longer route through Road or Oasis can have a lower total weighted cost.

## Why Do Different Unit Profiles Matter?

Scout, Carrier, and Ranger use different speed/radius/acceleration values and per-agent area costs. The same map can therefore be evaluated differently by different unit types.

## Global Pathfinding Vs Local Avoidance

Global pathfinding chooses the overall route across the NavMesh. Local avoidance adjusts movement while agents follow their routes, especially in multi-agent mode.

## How Does The Dynamic Obstacle Work?

The canyon gate uses `NavMeshObstacle` with carving. It can be toggled during movement with `O`. The collider is a trigger, so it does not push NPCs physically; the path changes because the NavMesh changes.

## Current Demo Limitations

- It uses Unity NavMesh rather than a custom A* implementation.
- It is a controlled research test map, not a full game level.
- Metrics are runtime HUD values, not exported datasets.
- Unit profiles are simple examples for demonstrating terrain preferences.
