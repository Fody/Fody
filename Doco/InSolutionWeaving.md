# How to write weaving code inside a solution.

## Introduction

Often it is necessary to have weaving code specific to a solution. In this case it is desirable to have that code exist in the same solution it is editing.

## How to weave inside a solution 

### Weavers Project 

Add a project named 'Weavers' to your solution. 

### ModuleWeaver Class 

Add a class named 'ModuleWeaver' to the project. See [ModuleWeaver] 

### Project Order 

Change the solution build order so the 'Weavers' project is built before the projects using it. Note: Do not add a project reference. 