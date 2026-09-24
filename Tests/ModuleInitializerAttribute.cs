#if !NET5_0_OR_GREATER
namespace System.Runtime.CompilerServices;

[AttributeUsage(AttributeTargets.Method)]
sealed class ModuleInitializerAttribute :
    Attribute;
#endif
