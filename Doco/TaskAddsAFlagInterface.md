# Why is there a new type in my assembly


You may have noticed that once your assembly has been weaved there is a new type added 

`internal interface ProcessedByFody`

The purpose of this type is to flag that the assembly has been processed by Fody. This allows Fody to skip processing when Visual Studio has no changed the assembly.

Now I am the first to admit that this is a hack. Unfortunately it was the only one I could think of to reliably mark and assembly as being processed. The other options were ruled out for various reasons.

##Adding a custom attribute

The idea here it to mark the assembly with an attribute saying it has been processed. To do this, while avoiding adding a dependency, I would have to add the attribute type to the assembly. So this has the same drawbacks as the flag type while meaning more work for me.

##Adding a standard attribute

I could use one of the standard .net attribute to the assembly. However I could not find one that held the right kind of meaning. There is also the possibility that the developer would be doing some kind of reflection that could be broken by me adding an arbitrary attribute to the assembly.

##Time stamps

This solution involved storing the file change time in memory after each weaving. Then using this value to skip the next run if the file time has not changed. This was my initial solution and worked for simple scenarios. Unfortunately it breaks down when you are chaining weavers or restart visual studio.

## It is a Hack

So yes this is a hack. And if anyone can come up with something better please let me know.