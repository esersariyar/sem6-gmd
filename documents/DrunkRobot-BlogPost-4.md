# Drunk Robot - Blog Post 4

This sprint was a mix of frustrating setbacks and real progress. I completed Level 1 with full obstacle logic, fixed a critical level transition bug, and dealt with an unexpected Unity cache issue that nearly cost me hours of work.

## Unity Cache Rollback

The biggest problem this week had nothing to do with code. Unity's cache system prompted me with a dialog, and I accidentally clicked "Yes" - which rolled back a significant portion of my progress. Some asset references and GitHub links were lost. I tried reverting through version history, but certain connections between scenes and prefabs were already broken. I had to manually re-link several references and rebuild parts of the project state. A painful reminder to read Unity dialogs carefully and commit more frequently.

## End Zone Level Transition Fix

Another issue was the end zone not triggering the transition to Level 2. The original approach used `OnTriggerEnter`, but the collision was not registering consistently. I rewrote it using a distance-based check instead. The `SpawnEndZone` script now calculates the Z-axis distance every frame, and when the player gets within 2 tiles, it loads the next scene:

```csharp
float dist = Mathf.Abs(player.position.z - transform.position.z);
if (dist <= triggerDistance)
{
    triggered = true;
    SceneManager.LoadScene(2);
}
```

Setting `triggerDistance` to roughly 2 tiles made the transition reliable — simpler and more predictable than relying on physics triggers.

## Level 1 Design - Walls, Colors, and Chaos

With technical issues resolved, I focused on completing Level 1. The level is a corridor where walls move toward the player. Three obstacle colors define distinct behaviors:

- **White walls** slide toward you from the front. They block the path and force the player to time their movement or find gaps to pass through.
- **Red obstacles** temporarily distort the player's vision when touched. They call `BoostDrunk()` on the camera's `MouseLook` script, making the screen wobble unpredictably for a short duration. These do not kill or stop you, they just make everything harder to play.
- **Yellow obstacles** push the player on contact. They apply a force that knocks you sideways or backward. The goal is to jump over them or avoid their trajectory.

The combination of these three types creates a layered challenge: white walls demand timing, red walls punish careless movement with confusion, and yellow walls physically disrupt positioning. The player needs to reach the end of the corridor by dodging, jumping, and adapting. Each wall group is randomized using the `ObstacleRandomizer` script, which activates one of three block variants per spawn, keeping each run different.

Level 1 now feels like a complete experience. Next sprint, I plan to build out Level 2 with new hazard types and start tuning difficulty progression across both levels.