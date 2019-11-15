This project ensures that the weaver works fine with the various target frameworks.

It just runs the weaver and verifies the assembly.

This simple test proves that the target framework of the target assembly does not really matter, since the IL will be always the same,
as long as the weaved code does not require any special type or method from a specific framework.