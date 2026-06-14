# DEMO_CHECKLIST.md

Use this checklist before showing the demo to the teacher.

## Scene Startup

- [ ] Open `Assets/Scenes/NavMeshDemo.unity`.
- [ ] Press Play.
- [ ] The whole map is visible in Game view with the top-down camera.
- [ ] HUD is readable and does not hide the route.

## Mode 1: Shortest Path

- [ ] Press `1`, then `Space`.
- [ ] HUD shows mode `Shortest path`.
- [ ] Expected route says `cost=1 -> central shortcut`.
- [ ] The path line goes through the central shortcut.
- [ ] Travel time finishes when the agent reaches the finish zone.

## Mode 2: Weighted Cost

- [ ] Press `2`, then `Space`.
- [ ] HUD shows `Expensive` cost as high.
- [ ] Expected route says `cost=18 -> long bypass`.
- [ ] The path line chooses a longer bypass instead of the orange expensive shortcut.

## Mode 3: Dynamic Obstacle

- [ ] Press `3`, then `Space`.
- [ ] Obstacle is visible and blocks the shortcut.
- [ ] `NavMeshObstacle` carving causes a bypass path.
- [ ] The obstacle does not physically push the NPC.
- [ ] Press `O` during movement.
- [ ] The obstacle toggles and the path replans after a short delay.

## Mode 4: Multi-Agent

- [ ] Press `4`, then `Space`.
- [ ] Several NPCs are active.
- [ ] Agents use separate destination points inside the finish zone.
- [ ] Reached count eventually matches the number of active agents.

## Metrics And Readability

- [ ] HUD shows mode, expected route, actual route, path status, repaths, path length, travel time, FPS, area costs, obstacle state, and active/reached agents.
- [ ] Finish zone is large and visually clear.
- [ ] Path visualization is high contrast.
- [ ] Start, finish, expensive area, bypasses, obstacle, and surface types are visually distinguishable.
