# OpenTK Tutorials

### Introduction

Basic and intermediate examples for getting started with [OpenTK](https://opentk.github.io/) from C# and .Net.

C# projects "translated" from C++ available at the excellent [**OpenGL tutorials site**](http://www.opengl-tutorial.org).

This is work in progress, the plan is to put all the examples from *opengl-tutorial* here, it might take a while though.

> **Although OpenTK supports multiple platforms, my code was only built and tested on Windows and Linux.**

### What you need

- Of course, OpenTK. You can get it as a [NuGet package](https://www.nuget.org/packages/OpenTK/) or build from [source](https://github.com/opentk/opentk). I use version 3.0.1 here.
- A building environment (I use Visual Studio Express 2017 on Windows and build with mono on Linux).
- A graphics card with drivers supporting OpenGL 3.3 or better. Almost everything newer than 3 or 4 years old should do. Sometimes a graphics driver update might be necessary. (If you aren´t sure about your card, check its capabilities with [OpenGL Extensions Viewer](http://realtech-vr.com/admin/glview) on Windows or using `gfxinfo` on Linux.)
- Some experience with C#. The examples are not meant to introduce you to a new language...

### Differences from OpenGL tutorials

I mentioned above that these tutorials are heavily based on the ones on [**OpenGL tutorials site**](http://www.opengl-tutorial.org).

If you peruse that site you´ll find excellent introductory texts to OpenGL features, apart from example code.

I kept the same basic organization they use (i. e. the tutorials have the same numbers). The recommended use is to start with their text and then read mine, which will only contain additional details relevant to OpenTK.

### Structure

Every tutorial is a standalone project in order to make it easier to understand and build each example. All tutorials are part of an overarching solution.

Each projects contains all files necessary to build (textures, shaders etc.).

### Index

- Tutorial 01 - Opening a Windows ([original tutorial](http://www.opengl-tutorial.org/beginners-tutorials/tutorial-1-opening-a-window/))
- Tutorial 02 - The first triangle ([original tutorial](http://www.opengl-tutorial.org/beginners-tutorials/tutorial-2-the-first-triangle/))
- Tutorial 03 : Matrices ([original tutorial](http://www.opengl-tutorial.org/beginners-tutorials/tutorial-3-matrices/))
