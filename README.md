# Campus Crawl
*Repository for SCC.230 group project Campus Crawl.*

Welcome to the Campus Crawl repository. To clone the project, follow the below instructions.
1. Make sure you have the .NET Framework 4.7.2 SDK installed in the Visual Studio installer.
2. `git clone https://github.com/c272/campus-crawl.git`
3. `git submodule init`
4. `git submodule update --recursive`
5. Restore NuGet packages for all projects.

## Gameplay Programming
To get started programming with tileEngine, you can view the documentation within the tileEngine subfolder in this repository (`tileEngine\Docs`). There is HTML documentation and articles describing basic use of the API, and how to integrate this with the editor. For basic steps without reading the documentation, to start building the main project you will first need to build tileEngine.

## Map Editing & Events
To get started editing the map, you must build tileEngine first. Open `tileEngine\tileEngine.sln` and restore NuGet packages by navigating to `Properties -> Manage NuGet Packages -> Restore`. Then, build the solution. Once this is done, you can then launch the tileEngine editor through Visual Studio's debugger, or through the output folder. **Please be aware that .teproj files are not compatible with git merge.** Do not edit the project with the editor at the same time as another person, otherwise changes will not be possible to merge.

## Contribution Guidelines
These guidelines determine how we should develop with this repository, for ease of development and to prevent code conflict.
1. **Do not contribute directly to `master`.** Create a new branch when developing a feature, and then create a pull request to add your finished code to `master`.
2. Follow the C# style guidelines outlined below in code contributed to `master` or code being pull requested into `master`.
3. Request code review from another repository maintainer, do not immediately self-merge pull requests unless necessary.
4. When editing the `.teproj`, merge back to master as soon as possible. This file is not compatible with git merge, so must be edited quickly.

## Style Guidelines
The following are the C# style guidelines for code contributed to this repository.
- Methods, properties and field names should be PascalCase for any public API, and *optionally* camelCase for private or internal functions.
- Opening braces go on their own line.
- Single line if, for, while statements should have no braces, and go on an indented line following the statement. eg.
  ```
  if (thing)
     doMethod();
  ```
- Indents are done with tabs.
