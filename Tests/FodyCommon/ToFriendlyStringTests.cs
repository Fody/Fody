public class ToFriendlyStringTests
{
    [Fact(Skip = "todo")]
    public void ToFriendlyName()
    {
        var currentDirectory = Environment.CurrentDirectory.ToLowerInvariant()
            .Replace(@"bin\debug", string.Empty)
            .Replace(@"bin\release", string.Empty);
        try
        {
            ThrowException1();
        }
        catch (Exception exception)
        {
            var friendlyString = exception.ToFriendlyString().ToLowerInvariant();
            friendlyString = friendlyString
                .Replace(currentDirectory, string.Empty);
// ReSharper disable StringLiteralTypo
            var expected = """
                           an unhandled exception occurred:
                           exception:
                           foo
                           type:
                           system.exception
                           stacktrace:
                              at tofriendlystringtests.throwexception2() in tofriendlystringtests.cs:line 60
                              at tofriendlystringtests.throwexception1() in tofriendlystringtests.cs:line 55
                              at tofriendlystringtests.tofriendlyname() in tofriendlystringtests.cs:line 15
                           source:
                           fodycommon.tests
                           targetsite:
                           void throwexception2()

                           """;
// ReSharper restore StringLiteralTypo
            Assert.Equal(expected, friendlyString);
        }
    }

    static void ThrowException1() =>
        ThrowException2();

    static void ThrowException2() =>
        throw new("Foo");
}