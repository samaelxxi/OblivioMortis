# Oblivio Mortis
Code for the game 'Oblivio Mortis,' developed during a 3-month  [GameDev Camp](https://gamedev.camp) in collaboration with a 2D artist, 3D artist, and game designer.   

## Game description
Oblivio Mortis is an isometric hack-and-slash game.   
Players can engage in melee combat, but they can also use ranged attacks.    
A unique game mechanic allows players to steal ammo by attacking enemies in melee. The type of ammo obtained depends on the enemy that was damaged.   
Another interesting mechanic is the ricochet feature. Players can aim at targets and then aim at a second target. The bullet will ricochet from the first target, inheriting its properties (such as increased damage or causing an explosion on impact), and then fly to the next target.

## Implementation details
The game extensively uses pooling. Factories contain pools for objects that tend to appear and disappear, such as bullets, enemies, and various visual effects (VFX). 

I followed a data-driven approach during development, allowing most values affecting game balance, like player or enemy speed, HP, etc., to be changed and fine-tuned in play mode.

To support our hack-and-slash gameplay, we required two main components: enemies and devices. The enemy AI is implemented using Behavior Trees, with the use of the NodeCanvas asset to expedite development without reinventing the wheel.

To add the possibility of creating complex interactions between devices within the game levels, I implemented a small ScriptableObject Events system. This system enables game designers to create their own events without writing additional code, allowing interaction with existing devices.

The game's visual effects (VFX) are created using a combination of Unity's built-in tools, including VFX Graph, Particle Systems, Decals, and custom shaders developed using Shader Graph.  
