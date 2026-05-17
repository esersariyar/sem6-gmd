# Drunk Robot - Blog Post 5

This sprint focused on polish and small problems with long fix times. Most work went into UI responsiveness, a sobriety state bug, shader mismatches on a downloaded asset, and new hazards for the final level.

## Sobriety Slider

Adding the sobriety slider to the top of the screen took a lot of effort. Making the bar responsive across resolutions needed many tries with Canvas anchors and Rect Transforms. I also did not want the default round handle to appear, since the slider works as a visual indicator and not an interactive control. Removing the handle through the inspector was not enough. I disabled the handle GameObject and adjusted the fill rect so the bar still filled from left to right.

## Coffee State Bug

A harder issue came from coffee pickups. After I duplicated coffee objects and the player drank more than one, the drunk effect failed to return once the sober period ended. The second coffee overwrote the timer without saving the previous drunk state. I fixed this by storing the drunk state on its own instead of relying on the active effect. Now each coffee extends the sober window, and the drunk effect resumes once the saved state expires, no matter how many coffees the player stacks.

## Turret Shader Conversion

I downloaded a turret asset for use as a hazard. The asset was old and its materials did not match my render pipeline. Everything looked pink. I converted every material to Universal Render Pipeline Lit one by one, since the batch upgrade missed some of them. Slow work, but the turret then matched the scene.

## Arcade Machine Controls

The arcade machine prefab had scrambled button mappings. The labels and the real inputs did not line up. I sorted this out using the layout from the lecturer slides and tested each button in play mode until everything matched.

## Lasers and Moving Hazards

Lasers now sit alongside the moving obstacles from the last sprint. To make them feel real instead of flat red lines, I added a Volume with bloom and post processing tweaks. The glow reads as dangerous, and the scene lighting feels grounded.

Moving objects and lasers are finalized. The next blog post will be the final one, covering the last round of fixes and the finished build.
