using System.Linq;
using System.Xml.Linq;
using Xunit;

namespace Tests.FodyIsolated
{
    public class ValidatePackageReferenceSettingsTests
    {
        [Theory]
        [InlineData(ValidProject)]
        [InlineData(ValidOldStyleProject)]
        [InlineData(InvalidOldStyleProject, InvalidOldStyleProjectErrors)]
        [InlineData(ValidProjectWithValidIncludeAssets)]
        [InlineData(ValidProjectWithPackageConfig)]
        [InlineData(ValidProjectWithTwoItemGroups)]
        [InlineData(ValidProjectWithConditionalGroups)]
        [InlineData(ValidProjectWithItemGroupInComplexProperty)]
        [InlineData(InvalidProjectWithMissingOrWrongPrivateAssets, InvalidProjectWithMissingOrWrongPrivateAssetsErrors)]
        [InlineData(InvalidProjectWithInvalidIncludeAssets, InvalidProjectWithInvalidIncludeAssetsErrors)]
        void Test(string projectFileContent, string expectedErrors = "")
        {
            var weaverNames = new[] { "Weaver1.Fody", "Weaver2.Fody" };
            var projectFile = XDocument.Parse(projectFileContent);

            var errors = InnerWeaver.GetPackageReferenceValidationErrors(projectFile, weaverNames).Distinct();

            Assert.Equal(expectedErrors, string.Join("|", errors));
        }

        const string ValidProject = @"
<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFrameworks>net472;netcoreapp2.2</TargetFrameworks>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include=""Weaver1.Fody"" Version=""1.2.3"" PrivateAssets=""All"" />
    <PackageReference Include=""Weaver2.Fody"" Version=""1.2.3"" >
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
  </ItemGroup>
</Project>";

        const string ValidOldStyleProject = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Project ToolsVersion=""12.0"" DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <Import Project=""$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props"" Condition=""Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')"" />
  <PropertyGroup>
    <TargetFrameworks>net472;netcoreapp2.2</TargetFrameworks>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include=""Weaver1.Fody"" Version=""1.2.3"" PrivateAssets=""All"" />
    <PackageReference Include=""Weaver2.Fody"" Version=""1.2.3"" >
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
  </ItemGroup>
</Project>";

        const string InvalidOldStyleProject = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Project ToolsVersion=""12.0"" DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <Import Project=""$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props"" Condition=""Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')"" />
  <PropertyGroup>
    <TargetFrameworks>net472;netcoreapp2.2</TargetFrameworks>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include=""Weaver1.Fody"" Version=""1.2.3"" />
    <PackageReference Include=""Weaver2.Fody"" Version=""1.2.3"" >
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
  </ItemGroup>
</Project>";
        const string InvalidOldStyleProjectErrors = "The package reference for Weaver1.Fody does not contain PrivateAssets='All'";

        const string ValidProjectWithPackageConfig = @"
<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFrameworks>net472;netcoreapp2.2</TargetFrameworks>
  </PropertyGroup>
  <ItemGroup>
  </ItemGroup>
</Project>";

        const string ValidProjectWithValidIncludeAssets = @"
<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFrameworks>net472;netcoreapp2.2</TargetFrameworks>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include=""Weaver1.Fody"" Version=""1.2.3"" PrivateAssets=""All"" IncludeAssets=""all""/>
    <PackageReference Include=""Weaver2.Fody"" Version=""1.2.3"" >
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; compile; native; contentfiles; analyzers; buildtransitive</IncludeAssets>    </PackageReference>
  </ItemGroup>
</Project>";


        const string ValidProjectWithTwoItemGroups = @"
<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFrameworks>net472;netcoreapp2.2</TargetFrameworks>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include=""Weaver1.Fody"" Version=""1.2.3"" PrivateAssets=""All"" />
  </ItemGroup>
  <ItemGroup>
    <PackageReference Include=""Weaver2.Fody"" Version=""1.2.3"" PrivateAssets=""All"" />
  </ItemGroup>
</Project>";

        const string ValidProjectWithConditionalGroups = @"
<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFrameworks>net472;netcoreapp2.2</TargetFrameworks>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include=""Weaver1.Fody"" Version=""1.2.3"" PrivateAssets=""All"" />
  </ItemGroup>
  <ItemGroup Condition=""true"">
    <PackageReference Include=""Weaver2.Fody"" Version=""1.2.3"" PrivateAssets=""All"" />
  </ItemGroup>
  <ItemGroup Condition=""false"">
    <PackageReference Include=""Weaver2.Fody"" Version=""1.2.3"" PrivateAssets=""All"" />
  </ItemGroup>
</Project>";

        const string ValidProjectWithItemGroupInComplexProperty = @"
<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFrameworks>net472;netcoreapp2.2</TargetFrameworks>
    <AnyProperty>
      <ItemGroup>
        <PackageReference Include=""Weaver1.Fody"" Version=""1.2.3"" />
      </ItemGroup>
    </AnyProperty>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include=""Weaver1.Fody"" Version=""1.2.3"" PrivateAssets=""All"" />
    <PackageReference Include=""Weaver2.Fody"" Version=""1.2.3"" PrivateAssets=""All"" />
  </ItemGroup>
</Project>";

        const string InvalidProjectWithMissingOrWrongPrivateAssets = @"
<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFrameworks>net472;netcoreapp2.2</TargetFrameworks>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include=""Weaver1.Fody"" Version=""1.2.3"" PrivateAssets=""None"" />
    <PackageReference Include=""Weaver2.Fody"" Version=""1.2.3"" />
  </ItemGroup>
</Project>";

        const string InvalidProjectWithMissingOrWrongPrivateAssetsErrors = "The package reference for Weaver1.Fody does not contain PrivateAssets='All'|The package reference for Weaver2.Fody does not contain PrivateAssets='All'";

        const string InvalidProjectWithInvalidIncludeAssets = @"
<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFrameworks>net472;netcoreapp2.2</TargetFrameworks>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include=""Weaver1.Fody"" Version=""1.2.3"" PrivateAssets=""All"" />
    <PackageReference Include=""Weaver2.Fody"" Version=""1.2.3"" >
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>    
    </PackageReference>
  </ItemGroup>
</Project>";

        const string InvalidProjectWithInvalidIncludeAssetsErrors = "The package reference for Weaver2.Fody is missing the 'compile' part in the IncludeAssets setting; it's recommended to completely remove IncludeAssets";


    }
}
