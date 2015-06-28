# Soapvox
Old voxel-based game engine project

##Disclaimer

This project will not open in Visual Studio, as it depends on the XNA Framework, which is now abandoned. Additionally, even if
all the dependencies were somehow straightened out, and the project opens, there's no guarantee that it will run, as I have no 
recollection of leaving the project in a runnable state.

##What is this?

Way back when, I attempted to immitate Minecraft's voxel-based game engine, and sought to learn a few things, the hard way, about game
programming. To call this project structured, would be to admit blindness, however, there are oasises of organization here and there.
Take a look, if you're bored. Some of the things the project includes: 

- Somewhat optimized octrees(3D Binary trees, essentially)
- Geometry shaders.
- Vertex batching(based on cube faces), to minimize draw() calls.
- In-game quake-style console, with undo/redoable commands.
- Implementation of the Command Pattern(See Gang of Four).
- 3D brushes(i.e. The user is able to create instances of a mailbox, represented by voxels, in the world.)

##Why is it here?

Mostly for posterity's sake, and it's something I can point people to when I bring it up.

