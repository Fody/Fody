# Overview

## Introduction 

Manipulating the IL of an assembly as part of a build requires a significant amount of plumbing code. This plumbing code involves knowledge of both the MSBuild and Visual Studio APIs. Fody attempts to elimination that plumbing code through an extensible add-in model. 

## The plumbing tasks Fody handles 

  * Injection of the MSBuild task into the build pipeline
  * Resolving the location of the assembly and pdb
  * Abstracts the complexities of logging to MSBuild
  * Reads the assembly and pdb into the Mono.Cecil object model
  * Re-applying the strong name if necessary
  * Saving the assembly and pdb

## What is required of an add-in 

Add-ins get passed an instance of 'Mono.Cecil.ModuleDefinition' and can manipulate it as required.

[HowToWriteAnAddin]