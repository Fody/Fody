---
name: Bug fix
about: Create a bug fix to help us improve
---

Where it is clear the below content has not read, the issue is likely to be closed with "please read the template". Please don't take offense at this. It is simply a time management decision. When someone raises an issue, without reading the template, then often too much time is spent going back and forth to obtain information that is outlined below.


#### Ensure you are a Patron

To raise a feature request you must be a [Patron of Fody](https://opencollective.com/fody/order/3059). The GitHub user will be verified against [Home/backers.md](https://github.com/Fody/Fody/blob/master/backers.md). See [Licensing/Patron FAQ](https://github.com/Fody/Fody#licensingpatron-faq) for more information.

If you are not able, or don't have the time, to implement the feature, and would prefer someone else implement it for you, consider proposing a [once off donation](https://opencollective.com/fody/donate) amount. When considering the amount, note that features can take many hours to implement.


#### Preamble

Questions specific to IL manipulation should be placed on [StackOveflow](https://stackoverflow.com/) or the [Cecil mailing list](https://groups.google.com/forum/#!forum/mono-cecil).

General questions about Fody or weavers should be placed on [StackOveflow](https://stackoverflow.com/) or the [Fody Gitter room](https://gitter.im/Fody/Fody)

Where relevant, ensure you are using the current stable versions of the following:

 * Fody (note for this you need an explicit NuGet reference to Fody in your csproj)
 * Fody weavers
 * Visual Studio
 * [.NET Core SDK](https://www.microsoft.com/net/download)

Any code or stack traces must be properly formatted with [GitHub markdown](https://guides.github.com/features/mastering-markdown/).


#### Describe the issue

A description of what the issue is and, if relevant, the expected behavior.


#### Minimal Repro

Ensure you have replicated the bug in a minimal solution with the fewest moving parts. Often this will help point to the true cause of the problem. Upload this repro as part of the issue, preferably a public GitHub repository or a downloadable zip. The repro will allow the maintainers of this project to smoke test the any fix.


#### Submit a PR that fixes the bug

Submit a [Pull Request (PR)](https://help.github.com/articles/about-pull-requests/) that fixes the bug. Include in this PR a test that verifies the fix. If you were not able to fix the bug, a PR that illustrates your partial progress will suffice. If you prefer someone else fix this bug for you, please donate to the [Fody OpenCollective](https://opencollective.com/fody/donate) and include a note to that effect in this issue.
