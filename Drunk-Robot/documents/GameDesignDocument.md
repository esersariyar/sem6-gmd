# Game Design Document – Drunk Robot (Version 1.0)

## Working Title
Drunk Robot

## Concept Statement
Drunk Robot is a comedic 3D arcade platformer where the player controls a malfunctioning robot trying to escape a small factory obstacle course. Due to a broken control system, the robot (sometimes) moves with delayed and unstable inputs, creating chaotic and funny gameplay.

## Genre
3D Arcade Platformer

## Target Audience
Casual players and arcade audiences looking for short, humorous, and replayable challenges.

## Unique Selling Points
* Unstable “drunk-like” movement caused by a broken control system
* Very short levels designed for quick arcade sessions (30–60 seconds)
* Funny physics-based movement that creates unpredictable situations
* High replayability by encouraging players to improve completion time

---

# Gameplay

## Player Experience
The player controls a malfunctioning robot using a joystick. The goal is to reach the exit of a small factory obstacle course without falling off the platforms or getting destroyed by hazards.

Because the robot’s control system is broken, movement is slightly delayed, unstable, and sometimes randomly deviates from the intended direction. The player must adapt and maintain balance while navigating the environment.

## Core Gameplay Loop
1. Player starts at the beginning of the level
2. Navigate unstable robot movement
3. Avoid hazards and obstacles
4. Reach the exit as fast as possible
5. Replay to improve time or avoid mistakes

---

# Mechanics

## Movement System
* Player moves the robot using joystick controls
* Movement has **input delay and wobbling**
* The robot may slightly drift left or right randomly
* Player must constantly adjust balance

## Hazards
* Moving platforms
* Rotating fans that push the robot away
* Laser beams that destroy the robot
* Narrow platforms that increase the risk of falling

## Failure Conditions
* Falling off the level
* Being hit by lasers or hazards

---

# Game World

## Setting
A small industrial factory environment designed like an obstacle course for testing robots.

## Level Design
Levels are short and focused on fast gameplay:
* Each level lasts **30–60 seconds**
* Simple platforming challenges
* Increasing difficulty with more hazards

### Example Level Structure:
* **Level 1** – Basic movement tutorial
* **Level 2** – Moving platforms and narrow paths
* **Level 3** – Fans and lasers added

---

# Visual & Audio Style

## Visual Style
* Simple **low-poly 3D graphics**
* Bright colors and clear obstacles
* Cartoon-style robot animations

## Audio
* Funny robot malfunction sounds
* Light arcade-style background music
* Sound effects for lasers, fans, and falling

---

# Platform
Arcade machine / PC

---

# Project Milestones

### Milestone 1 – Player Movement Prototype
Implement basic robot movement with delayed and unstable controls.

### Milestone 2 – Obstacles and Level Prototype
Add moving platforms, fans, and laser hazards. Create the first playable level.

### Milestone 3 – Final Level and Polish
Add visual polish, sound effects, and difficulty balancing for the final arcade experience.
