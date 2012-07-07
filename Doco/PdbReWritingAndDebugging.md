# Implications on debugging and pdb re-writing

At assembly processing time the WeavingTask looks for a pdb file in same directory as the assembly. If one is found it will modify that pdb file to keep in sync with the assembly.

So you should not have any problems with debugging.