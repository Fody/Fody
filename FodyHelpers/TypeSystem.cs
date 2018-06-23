using System;
using Mono.Cecil;

namespace Fody
{
    /// <summary>
    /// Replacement for <see cref="ModuleDefinition.TypeSystem"/>.
    /// </summary>
    public class TypeSystem
    {
        public TypeSystem(Func<string, TypeDefinition> resolve, ModuleDefinition module)
        {
            ObjectDefinition = resolve("System.Object");
            ObjectReference = module.ImportReference(ObjectDefinition);
            VoidDefinition = resolve("System.Void");
            VoidReference = module.ImportReference(VoidDefinition);
            BooleanDefinition = resolve("System.Boolean");
            BooleanReference = module.ImportReference(BooleanDefinition);
            CharDefinition = resolve("System.Char");
            CharReference = module.ImportReference(CharDefinition);
            SByteDefinition = resolve("System.SByte");
            SByteReference = module.ImportReference(SByteDefinition);
            ByteDefinition = resolve("System.Byte");
            ByteReference = module.ImportReference(ByteDefinition);
            Int16Definition = resolve("System.Int16");
            Int16Reference = module.ImportReference(Int16Definition);
            UInt16Definition = resolve("System.UInt16");
            UInt16Reference = module.ImportReference(UInt16Definition);
            Int32Definition = resolve("System.Int32");
            Int32Reference = module.ImportReference(Int32Definition);
            UInt32Definition = resolve("System.UInt32");
            UInt32Reference = module.ImportReference(UInt32Definition);
            Int64Definition = resolve("System.Int64");
            Int64Reference = module.ImportReference(Int64Definition);
            UInt64Definition = resolve("System.UInt64");
            UInt64Reference = module.ImportReference(UInt64Definition);
            SingleDefinition = resolve("System.Single");
            SingleReference = module.ImportReference(SingleDefinition);
            DoubleDefinition = resolve("System.Double");
            DoubleReference = module.ImportReference(DoubleDefinition);
            IntPtrDefinition = resolve("System.IntPtr");
            IntPtrReference = module.ImportReference(IntPtrDefinition);
            UIntPtrDefinition = resolve("System.UIntPtr");
            UIntPtrReference = module.ImportReference(UIntPtrDefinition);
            StringDefinition = resolve("System.String");
            StringReference = module.ImportReference(StringDefinition);
        }

        public TypeReference IntPtrReference { get; }
        public TypeReference UIntPtrReference { get; }
        public TypeReference StringReference { get; }
        public TypeReference ObjectReference { get; }
        public TypeReference VoidReference { get; }
        public TypeReference BooleanReference { get; }
        public TypeReference CharReference { get; }
        public TypeReference SByteReference { get; }
        public TypeReference ByteReference { get; }
        public TypeReference Int16Reference { get; }
        public TypeReference UInt16Reference { get; }
        public TypeReference Int32Reference { get; }
        public TypeReference UInt32Reference { get; }
        public TypeReference Int64Reference { get; }
        public TypeReference UInt64Reference { get; }
        public TypeReference SingleReference { get; }
        public TypeReference DoubleReference { get; }
        public TypeDefinition ObjectDefinition { get; }
        public TypeDefinition VoidDefinition { get; }
        public TypeDefinition BooleanDefinition { get; }
        public TypeDefinition CharDefinition { get; }
        public TypeDefinition SByteDefinition { get; }
        public TypeDefinition ByteDefinition { get; }
        public TypeDefinition Int16Definition { get; }
        public TypeDefinition UInt16Definition { get; }
        public TypeDefinition Int32Definition { get; }
        public TypeDefinition UInt32Definition { get; }
        public TypeDefinition Int64Definition { get; }
        public TypeDefinition UInt64Definition { get; }
        public TypeDefinition SingleDefinition { get; }
        public TypeDefinition DoubleDefinition { get; }
        public TypeDefinition IntPtrDefinition { get; }
        public TypeDefinition UIntPtrDefinition { get; }
        public TypeDefinition StringDefinition { get; }
    }
}