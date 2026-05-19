# Drunk Robot - Blog Post 6

This is the final post for Drunk Robot. The game is done, the build runs in the browser, and this sprint went into wrapping up the last fixes, polishing the feel, and looking back at the full project.

## The Final Game

Drunk Robot is a 3D arcade platformer where you guide a malfunctioning robot through two levels of moving walls, lasers, turrets, fans, and fleeing female robot. Coffee pickups restore your sobriety for a short window, so the player has to balance risk and recovery. The intro scene sets the tone, Level 1 teaches the rules with the white, red, and yellow walls, and the final level layers on harder hazards and tighter pacing.

## Last Round of Fixes

The remaining work was small but visible. I cleaned up respawn behavior so coffee pickups reset along with the player. I fine tuned the drunk camera curve so the wobble feels playful instead of nauseating. I also adjusted the pause menu layout so the buttons line up on both ultrawide and standard screens.

A few collider sizes on the final level hazards were off by enough to feel unfair, so I shrank them to match the visible mesh. Small change, big difference in how the level reads.

## What Worked

- Keeping movement code simple and tweaking values instead of rewriting systems.
- Building Level 1 as a teaching level before adding harder mechanics.
- Writing the drunk and drink animations in code instead of fighting the Animator.
- Using a Volume with bloom to sell the lasers as real threats.

## What I Would Do Differently

- Commit more often. The Unity cache rollback in an earlier sprint cost me hours.
- Start with the Universal Render Pipeline in mind before downloading third party assets.
- Plan the UI scaling rules once at the start instead of fixing each element later.

## How to Play

Use WASD to move and Space to jump. The robot starts sober, but alcohol pickups make the camera wobble and push the robot forward without your input. Coffee pickups reset the sobriety meter and give you back full control for a short window. The game also supports the arcade machine setup the lecturer asked us to target, so the required buttons are shown on the wall and inside the on screen warning texts during play. Survive the corridor, dodge the hazards, reach the end zone.

## Final Features

- Intro scene, Level 1, and a harder final level.
- Sobriety system with alcohol and coffee pickups.
- Hazards: moving walls, lasers, turrets, fans, fleeing female robot.
- Responsive sobriety slider, pause menu, and HUD.

Play: gmd.eser.dk
Source: github.com/esersariyar/sem6-gmd

## Closing

Drunk Robot started as a small idea about a robot who drinks too much and ends up barely able to walk straight. It grew into a full arcade run with two levels, custom hazards, a sobriety system, and a tone I am happy with. Thanks for following along.
