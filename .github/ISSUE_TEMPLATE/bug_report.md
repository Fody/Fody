---
name: Bug/Issue
about: Report a bug or other issue
---

Where it is clear that the below content has not read, the issue is likely to be closed with "please read the template". Please don't take offense at this. It is simply a time management decision. When someone raises an issue, without reading the template, then often too much time is spent going back and forth to obtain information that is outlined below.


#### You should already be a Patron

To be using Fody you should be a [Patron](https://opencollective.com/fody/order/3059). See [Licensing/Patron FAQ](https://github.com/Fody/Home/blob/master/pages/licensing-patron-faq.md). With that in mind, it is assumed anyone raising an issue is already a Patron. As such your GitHub Id may be verified against the [OpenCollective contributors](https://opencollective.com/fody#contributors). This process will depend on the issue quality, your circumstances, and the impact on the larger user base.


#### Preamble

Questions specific to IL manipulation should be placed on [Stack Overflow](https://stackoverflow.com/) or the [Cecil Forum](https://groups.google.com/forum/#!forum/mono-cecil).

General questions about Fody or weavers should be placed on [Stack Overflow](https://stackoverflow.com/) or the [Fody Gitter room](https://gitter.im/Fody/Fody)

Where relevant, ensure you are using the current stable versions of the following:

 * Fody (note for this you need an explicit NuGet reference to Fody in your csproj)
 * Any Fody weavers being used
 * Visual Studio
 * [.NET Core SDK](https://www.microsoft.com/net/download)

Any code or stack traces must be properly formatted with [GitHub markdown](https://guides.github.com/features/mastering-markdown/).


#### Describe the issue

A description of what the issue is and, if relevant, the expected behavior.


#### Minimal Repro

Ensure you have replicated the bug in a minimal solution with the fewest moving parts. Often this will help point to the true cause of the problem. Upload this repro as part of the issue, preferably a public GitHub repository or a downloadable zip. The repro will allow the maintainers of this project to smoke test the any fix.


#### Submit a PR that fixes the bug

Submit a [Pull Request (PR)](https://help.github.com/articles/about-pull-requests/) that fixes the bug. Include in this PR a test that verifies the fix. If you were not able to fix the bug, a PR that illustrates your partial progress will suffice. If you prefer someone else fix this bug for you, please donate to the [Fody OpenCollective](https://opencollective.com/fody/donate) and include a note to that effect in this issue.
