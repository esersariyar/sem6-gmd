# Roll a Ball: A Brief Technical Description of How I Implemented My First Game in the Course

Welcome to my very first game development project for the GMD1 course! In this post, I will provide a brief technical description of how I implemented the classic "Roll-a-ball" game using the Unity engine. This project served as an excellent introduction to the world of game development, teaching me the fundamental concepts of 3D space, physics, scripting, and the Unity Editor interface.

## Building the Foundation

To build this game, I closely followed the official Unity "Roll-a-ball" tutorial. The tutorial was incredibly helpful in guiding me through the initial setup, such as creating the game environment, setting up the player object, and writing my first C# game scripts. By following the step-by-step instructions, I learned how to manipulate GameObjects and attach components like colliders and rigidbodies. 

The core mechanic of the game revolves around controlling a sphere on a flat plane to collect floating cubes (pickups). To achieve this, I utilized Unity's physics engine. The player's movement is driven by adding forces to the Rigidbody component attached to the player sphere.

## Technical Experiences and Debugging

While implementing the player movement, I encountered my first learning experience. The script reads the input from the keyboard and applies a physics force to move the ball. The code snippet for applying the force looks like this:

```csharp
rb.AddForce(movement * speed);
```

During my initial testing, I noticed that the ball was moving incredibly slowly, barely accelerating at all. After some reviewing, I realized my mistake: I had originally forgotten to multiply the movement vector by the `speed` variable! Without the speed multiplier, the applied force was simply too small. Once I added the `speed` multiplier to the `AddForce` method and adjusted the value in the Unity Inspector, the ball moved smoothly. It was a simple mistake, but a great reminder of how important multipliers are in physics simulations.

## Adding Enemies and Unexpected Hilarity

As I expanded upon the basic game, I experimented with adding moving enemies to make the game more challenging. During this implementation, I had another funny experience. 

I wanted to add some dynamic boxes as obstacles, and I mistakenly added these dynamic box GameObjects directly as child objects *inside* the enemy GameObject in the Unity hierarchy. Because of how parent-child relationships work in Unity, child objects inherit the transform of their parent. As a result, these dynamic boxes started moving perfectly in sync with the enemy! Whenever the enemy moved or turned, the boxes stubbornly followed suit in a perfectly rigid formation. It looked absolutely hilarious, like the enemy was carrying around its own personal entourage of floating boxes. :D

While it wasn't what I originally intended, seeing those boxes confidently glide across the screen attached to the enemy gave me a great laugh. It taught me a valuable lesson about the Unity hierarchy. I eventually separated them out of the enemy's hierarchy to make them independent physics objects.

## Conclusion

Developing "Roll-a-ball" has been a rewarding first step into game development. Following the tutorial provided a solid technical foundation, but it was the small mistakes and funny visual bugs that truly reinforced my understanding of Unity's engine. I'm excited to apply these technical lessons to future projects!
