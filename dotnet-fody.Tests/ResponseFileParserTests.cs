using NUnit.Framework;

namespace Dotnet.Fody.Tests
{
    public class ResponseFileParserTests
    {
        private const string ResponseFile =
@"--temp-output:C:\source\MyProject\src\TestApp\obj\Debug\netcoreapp1.0\
--out:C:\source\MyProject\src\TestApp\bin\Debug\netcoreapp1.0\TestApp.dll
--define:MY_DEFINE
--define:DEBUG
--define:TRACE
--define:NETCOREAPP1_0
--suppress-warning:CS1701
--suppress-warning:CS1702
--suppress-warning:CS1705
--optimize:False
--key-file:c:\source\MyProject\src\TestApp\Key.snk
--public-sign:True
--emit-entry-point:True
--output-name:TestApp
--file-version:1.0.0.0
--version:1.0.0.0
--informational-version:1.0.0
--target-framework:.NETCoreApp,Version=v1.0
--reference:C:\Users\bob\.nuget\packages\Microsoft.CSharp\4.0.1\ref\netstandard1.0\Microsoft.CSharp.dll
--reference:C:\Users\bob\.nuget\packages\Microsoft.NETCore.Portable.Compatibility\1.0.1\ref\netstandard1.0\System.ComponentModel.DataAnnotations.dll
C:\source\MyProject\src\TestApp\Program.cs
C:\source\MyProject\src\TestApp\Properties\AssemblyInfo.cs";

        [Test]
        public void IntermediateDirectory()
        {
            Assert.AreEqual(
                @"C:\source\MyProject\src\TestApp\obj\Debug\netcoreapp1.0\",
                ResponseFileParser.Parse(ResponseFile).IntermediateDirectory
            );
        }

        [Test]
        public void AssemblyFilePath()
        {
            Assert.AreEqual(
                @"C:\source\MyProject\src\TestApp\bin\Debug\netcoreapp1.0\TestApp.dll",
                ResponseFileParser.Parse(ResponseFile).AssemblyFilePath
            );
        }


        [Test]
        public void DefineConstants()
        {
            CollectionAssert.AreEquivalent(
                new[] { "MY_DEFINE", "DEBUG", "TRACE", "NETCOREAPP1_0" },
                ResponseFileParser.Parse(ResponseFile).DefineConstants
            );
        }

        [Test]
        public void KeyFilePath()
        {
            Assert.AreEqual(
                @"c:\source\MyProject\src\TestApp\Key.snk",
                ResponseFileParser.Parse(ResponseFile).KeyFilePath
            );
        }

        [Test]
        public void SignAssembly()
        {
            Assert.IsTrue(
                ResponseFileParser.Parse(ResponseFile).SignAssembly
            );
        }

        [Test]
        public void References()
        {
            CollectionAssert.AreEquivalent(
                new[]
                {
                    @"C:\Users\bob\.nuget\packages\Microsoft.CSharp\4.0.1\ref\netstandard1.0\Microsoft.CSharp.dll",
                    @"C:\Users\bob\.nuget\packages\Microsoft.NETCore.Portable.Compatibility\1.0.1\ref\netstandard1.0\System.ComponentModel.DataAnnotations.dll"
                },
                ResponseFileParser.Parse(ResponseFile).References
            );
        }
    }
}
