# MoonWorks

MoonWorks is a free cross-platform game development framework. Its implementation is heavily inspired by [FNA](https://github.com/FNA-XNA/FNA), which is itself a free implementation of the Microsoft XNA API.

MoonWorks wraps native-code implementations in managed C# for clean high-level abstractions that still run fast. It's simple and it tries to stay out of your way as much as possible.

MoonWorks *does* provide the components you need to implement a functioning game: window management, input, graphics, 3D math utilities, and audio.

MoonWorks *does not* include things like a built-in physics engine, a GUI editor, or a standard rendering pipeline. My philosophy is that these decisions and tools are better made per-team or per-project.

If you don't mind learning what a vertex buffer is, this framework is for you.

## Dependencies

* [SDL2](https://github.com/flibitijibibo/SDL2-CS) - Window management, Input
* [Refresh](https://gitea.moonside.games/MoonsideGames/Refresh) - Graphics
* [FAudio](https://github.com/FNA-XNA/FAudio) - Audio

## License

MoonWorks is released under the zlib license. See LICENSE for details.

MoonWorks uses code from the FNA project, released under the Microsoft Public License. See fna.LICENSE for details. By extension it uses code from the Mono.Xna project, released under the MIT license. See monoxna.LICENSE for details.
