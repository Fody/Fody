using Mono.Cecil;
#pragma warning disable 618

namespace Fody
{
    public static class BaseModuleWeaverEx
    {
        public static TypeDefinition FindType<T>(this BaseModuleWeaver weaver)
        {
            Guard.AgainstNull(nameof(weaver), weaver);
            Guard.AgainstNull(nameof(weaver.FindType), weaver.FindType);
            return weaver.FindType(typeof(T).FullName);
        }
    }
}