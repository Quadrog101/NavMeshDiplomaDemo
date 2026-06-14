# DEMO_CHECKLIST.md

Use this checklist before showing the demo to the teacher.

## Scene Startup

- [ ] Open `Assets/Scenes/NavMeshDemo.unity`.
- [ ] Press Play.
- [ ] The whole Desert Route Test map is visible in Game view.
- [ ] HUD is readable and does not hide the main route decisions.

## Map Readability

- [ ] Start Base is on the left.
- [ ] Finish Camp is on the right.
- [ ] North Road, Central Dunes, South Oasis Path, and Canyon Gate are visually distinct.
- [ ] River/Ford and Mountain/Rock areas are visually distinct from Road, Oasis, and Desert.
- [ ] Terrain surfaces have different colors and labels.
- [ ] Rock islands and asymmetric passages make the map look like a test route, not a flat picture.

## Mode 1: Shortest Path

- [ ] Press `1`, then `Space`.
- [ ] HUD shows `Shortest path`.
- [ ] HUD shows effective `Hazard cost=1`.
- [ ] Expected route says central desert shortcut.
- [ ] The visible path line prefers the short Central Desert route.
- [ ] Travel time finishes at the finish camp.

## Mode 2: Weighted Terrain

- [ ] Press `2`, then `Space`.
- [ ] HUD shows `Resource-weighted`.
- [ ] HUD shows `Hazard cost=18`.
- [ ] Expected route says Oasis/Road bypass.
- [ ] Resource metrics are visible: water, food, stamina, risk, oasis bonus.
- [ ] The route moves away from the central shortcut toward North Road or South Oasis.

## Mode 3: Dynamic Obstacle

- [ ] Press `3`, then `Space`.
- [ ] The canyon gate obstacle is active.
- [ ] `NavMeshObstacle` carving affects the path.
- [ ] Press `O` during movement.
- [ ] The obstacle toggles without resetting the scene.
- [ ] The path replans after a short delay.
- [ ] Repaths counter increases after the toggle.
- [ ] The obstacle does not physically push the NPC.

## Mode 4: Multi-Agent / Multi-Class

- [ ] Press `4`, then `Space`.
- [ ] Scout, Carrier, and Nomad are active.
- [ ] Agents have different colors/profiles.
- [ ] HUD lists active profiles and selected routes.
- [ ] Agents use separate finish points.
- [ ] Local avoidance is visible when agents get close.

## HUD And Metrics

- [ ] HUD shows mode.
- [ ] HUD shows selected profiles.
- [ ] HUD shows terrain costs.
- [ ] HUD shows resource metrics.
- [ ] HUD shows expected route and actual route.
- [ ] HUD shows path status.
- [ ] HUD shows repaths.
- [ ] HUD shows path length and travel time.
- [ ] HUD shows obstacle state.
- [ ] HUD shows active/reached agents.
- [ ] HUD shows a short conclusion.
