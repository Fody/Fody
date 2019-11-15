[![NuGet Status](https://img.shields.io/nuget/v/BasicFodyAddin.Fody.svg?style=flat&max-age=86400)](https://www.nuget.org/packages/BasicFodyAddin.Fody/)

![Icon](https://raw.githubusercontent.com/Fody/Home/master/BasicFodyAddin/package_icon.png)

This is a simple solution used to illustrate how to [write a Fody addin](/pages/addin-development.md).


## Usage

See also [Fody usage](/pages/usage.md).


### NuGet installation

Install the [BasicFodyAddin.Fody NuGet package](https://www.nuget.org/packages/BasicFodyAddin.Fody/) and update the [Fody NuGet package](https://www.nuget.org/packages/Fody/):

```powershell
PM> Install-Package Fody
PM> Install-Package BasicFodyAddin.Fody
```

The `Install-Package Fody` is required since NuGet always defaults to the oldest, and most buggy, version of any dependency.


### Add to FodyWeavers.xml

Add `<BasicFodyAddin/>` to [FodyWeavers.xml](/pages/configuration.md#fodyweaversxml)

```xml
<Weavers>
  <BasicFodyAddin/>
</Weavers>
```


## The moving parts

See [writing an addin](/pages/addin-development.md)


## Icon

[Lego](https://thenounproject.com/term/lego/16919/) designed by [Timur Zima](https://thenounproject.com/timur.zima/) from [The Noun Project](https://thenounproject.com).