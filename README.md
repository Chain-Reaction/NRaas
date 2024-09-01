# NRaas mod collection

This is a collection of NRaas mods for The Sims 3.

## Deciphering and repo map

The Sims3 directory contains (inside subdirectories) the unprotected EA DLL files from the game, where most if not all fields and methods are made public.
The NRaas mods are built against those DLLs, which are not the same as in the game but, for some glitchy reason, it works (and helps modders).

The NRaas and NRaasBootstrap directories are dependencies which most of the mods rely upon (some, like NoCD, do not).

The NRaasPorter directory contains (the source code for) a .package file explorer, which is not a mod but a tool.

The other directories whose names begin with "NRaas" are the mods themselves (or additional modules of mods).

The NRaasChemistry and NRaasEconomizer directories contain, most likely, stub mods that were not finished
and whose code was likely not fully committed to the repo - as the contents of the .csproj files suggest.

The Ani directory contains the Ani mods.

The contents of the other directories are unknown.

## How to build the NRaas mods

Open the "Sims 3.sln" solution file in Visual Studio.
The particular setup you need to do in order to create a new mod project is unnecessary here, as the csproj and sln files already exist.
Building the "Sims 3.sln" solution in itself will fail (because of the very exceptions below), however you will be able to build each NRaas mod individually, except for:
- NRaas and NRaasBootstrap will build, but they are useless by themselves, and don't need to be built for the other mods to work
- NRaasChemistry and NRaasEconomizer will not build, as their code is incorrect and incomplete
- NRaasCupcake is expecting two additional base DLLs alongside the unprotected base ones, related to to objects from the Store, which are missing from the repo at the moment
- NRaasErrorTrap contains a C# compiler error, in VS2022 at least, which I don't fully understand and which is likely either a bug in the compiler itself or a C# version compatibility issue
