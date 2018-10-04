# OpenTK Tutorials

### Introduction

Basic and intermediate examples for getting started with [OpenTK][otk] from C# and .Net.

C# projects "translated" from C++ available at the excellent **[OpenGL tutorials site]**[ogltutorial].

This is work in progress, the plan is to put all the examples from *opengl-tutorial* here, it might take a while though.

> **Although OpenTK supports multiple platforms, my code was only built and tested on Windows and Linux.**

### What you need

- Of course, OpenTK. You can get it as a [NuGet package][nug] or build from [source][src].
- A building environment (I use Visual Studio Express 2017 on Windows and build with mono on Linux).
- A graphics card with drivers supporting OpenGL 3.3 or better. Almost everything newer than 3 or 4 years old should do. Sometimes a graphics driver update might be necessary. (If you aren´t sure about your card, check its capabilities with [OpenGL Extensions Viewer][extview] on Windows or using `gfxinfo` on Linux.)
- Some experience with C#. The examples are not meant to introduce you to a new language...

### Differences from OpenGL tutorials

I mentioned above that these tutorials are heavily based on the ones on **[OpenGL tutorials site]**[ogltutorial].

If you peruse that site you´ll find excellent introductory texts to OpenGL features, apart from example code.

I kept the same basic organization they use (i. e. the tutorials have the same numbers). The recommended use is to start with their text and then read mine, which will only contain additional details relevant to OpenTK.

### Structure

Every tutorial is a standalone project in order to make it easier to understand and build each example.

It contains all files necessary to buld (textures, shaders etc.).

### Index

- Tutorial 01 - Opening a Windows ([original tutorial][tut01])
- Tutorial 02 - The first triangle ([original tutorial][tut02])
- Tutorial 03 : Matrices ([original tutorial][tut03])

[otk] : https://opentk.github.io/
[ogltutorial] : http://www.opengl-tutorial.org
[nug] : https://www.nuget.org/packages/OpenTK/
[src] : https://github.com/opentk/opentk
[extview] : http://realtech-vr.com/admin/glview
[tut01] : http://www.opengl-tutorial.org/beginners-tutorials/tutorial-1-opening-a-window/
[tut02] : http://www.opengl-tutorial.org/beginners-tutorials/tutorial-2-the-first-triangle/
[tut03] : http://www.opengl-tutorial.org/beginners-tutorials/tutorial-3-matrices/
