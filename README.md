# 1. Yada Engine
Yada Engine was a 3D game engine and editor made for the courses GAM300 and GAM350 at university. Using it, we made the co-op platformer Fruit Punch!

Utilized: C++, C#, Mono, Assimp, Vulkan, ImGUI, Xinput, Winsock

# 2. My Responsibilities 
Continuing from LossEngine, I oversaw the loading of assets into the engine. Model and audio file types such as FBX and OGG take up a lot of space so I binarized them before streaming them back into the editor. This reduced loading times by roughly 60% for the editor.

I also ensured that newly imported or created assets in the windows platform would display in the editor through hot-reloading.

I also created the prefab system similar to Unity where game designers could create templates of frequently used game objects and save them. I touched upon my teammate's component system and another teammate's I/O file writing system to achieve this.

Gameplay wise, I created helper scripts that would allow the game designers to have functionality such as hooks and callbacks. For example, they could write a key and door script and quickly link the two of these entities together. 

# 3. How to Use
Run Executable/YadaEditor.exe

You can load any of the scenes in Resources/Scenes to try out the game.

You can also play the game made with this engine at: https://arcade.digipen.edu/games/fruit-punch
