# Drunk Robot - Blog Post 3

In this sprint, I focused on making Drunk Robot feel more playable, not just technically functional. Since the core fantasy is controlling a malfunctioning robot, the challenge is to keep movement chaotic and funny while still giving the player enough control to react.

What I improved so far:

- UI readability across different resolutions
- Camera follow responsiveness
- Physics stability after collisions
- A code-driven "drinking whiskey" character animation without using Animator

On the UI side, I tested how text behaves on different screen sizes in Unity Canvas. At first, some labels were tiny on certain resolutions while others became too large and broke layout alignment. I learned this is easy to solve when Canvas Scaler is configured correctly. After adjusting it, the text became responsive and the interface looked much more consistent on different monitor sizes and aspect ratios.

For the camera, I switched to Cinemachine and tuned follow settings to make movement smoother and more responsive. In a game like this, unstable controls already create pressure. If the camera reacts too late or too aggressively, the game quickly feels unfair. With Cinemachine, player tracking became cleaner, and platform reading got easier during risky jumps.

The hardest technical issue was a collision bug. After hitting certain colliders, the character entered an endless left-right rotation loop and became uncontrollable. I fixed this by freezing rotation axes on the Rigidbody. Once Freeze Rotation was enabled, the unwanted spinning stopped and the character behavior became predictable again.

I also added a small stylized "drunk" moment where the character appears to drink whiskey, and I implemented it directly in code without Animator. The key part is blending hand rotation and bottle wobble over time:

```csharp
float p = Mathf.Clamp01(t / sipDuration);
float wave = Mathf.Sin(p * Mathf.PI);

handBone.localRotation = handStartRot * Quaternion.Euler(handTilt * wave);
bottle.localPosition += new Vector3(0f, Mathf.Sin(Time.time * 16f) * 0.0007f, 0f);
```

This gave me a lightweight animation style that fits the game tone and keeps full control inside gameplay code. Next, I want to increase obstacle difficulty, test level flow under pressure, and add more feedback to improve game feel.