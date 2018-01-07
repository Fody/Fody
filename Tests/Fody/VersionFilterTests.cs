using System;
using Fody;
using Xunit;

namespace Tests.Fody
{
    public class VersionFilterTests
    {
        [Fact]
        public void When_major_version_is_specified_it_matchs()
        {
            VersionFilter filter = VersionFilter.Parse("1");

            Assert.True(filter.IsMatch(new Version(1, 0)));
            Assert.True(filter.IsMatch(new Version(1, 1, 2)));
            Assert.True(filter.IsMatch(new Version(1, 1, 2, 3)));
            Assert.False(filter.IsMatch(new Version(2, 0)));
            Assert.False(filter.IsMatch(new Version(0, 0)));
        }

        [Fact]
        public void When_major_and_minor_version_is_specified_it_matchs()
        {
            VersionFilter filter = VersionFilter.Parse("1.1");

            Assert.True(filter.IsMatch(new Version(1, 1)));
            Assert.True(filter.IsMatch(new Version(1, 1, 2)));
            Assert.True(filter.IsMatch(new Version(1, 1, 2, 3)));
            Assert.False(filter.IsMatch(new Version(1, 2)));
            Assert.False(filter.IsMatch(new Version(1, 0)));
        }

        [Fact]
        public void When_major_minor_and_build_version_is_specified_it_matchs()
        {
            VersionFilter filter = VersionFilter.Parse("1.1.1");

            Assert.True(filter.IsMatch(new Version(1, 1, 1)));
            Assert.True(filter.IsMatch(new Version(1, 1, 1, 2)));
            Assert.False(filter.IsMatch(new Version(1, 1, 0)));
            Assert.False(filter.IsMatch(new Version(1, 1, 0)));
        }

        [Fact]
        public void When_major_minor_build_and_revision_version_is_specified_it_matchs()
        {
            VersionFilter filter = VersionFilter.Parse("1.1.1.1");
            
            Assert.True(filter.IsMatch(new Version(1, 1, 1, 1)));
            Assert.False(filter.IsMatch(new Version(1, 1, 1, 0)));
            Assert.False(filter.IsMatch(new Version(1, 1, 1, 2)));
        }

        [Fact]
        public void When_major_and_wildcard_is_specified_it_matchs()
        {
            VersionFilter filter = VersionFilter.Parse("1.*");

            Assert.True(filter.IsMatch(new Version(1, 1)));
            Assert.True(filter.IsMatch(new Version(1, 1, 2)));
            Assert.True(filter.IsMatch(new Version(1, 1, 2, 3)));
            Assert.False(filter.IsMatch(new Version(2, 0)));
            Assert.False(filter.IsMatch(new Version(0, 9)));
        }

        [Fact]
        public void When_major_minor_and_wildcard_version_is_specified_it_matchs()
        {
            VersionFilter filter = VersionFilter.Parse("1.1.*");

            Assert.True(filter.IsMatch(new Version(1, 1)));
            Assert.True(filter.IsMatch(new Version(1, 1, 2)));
            Assert.True(filter.IsMatch(new Version(1, 1, 2, 3)));
            Assert.False(filter.IsMatch(new Version(1, 2)));
            Assert.False(filter.IsMatch(new Version(1, 0)));
        }

        [Fact]
        public void When_major_minor_build_and_wildcard_version_is_specified_it_matchs()
        {
            VersionFilter filter = VersionFilter.Parse("1.1.1.*");

            Assert.True(filter.IsMatch(new Version(1, 1, 1)));
            Assert.True(filter.IsMatch(new Version(1, 1, 1, 2)));
            Assert.False(filter.IsMatch(new Version(1, 1, 0)));
            Assert.False(filter.IsMatch(new Version(1, 1, 0)));
        }

        [Fact]
        public void When_implicit_inclusive_range_version_is_specified_it_matchs()
        {
            VersionFilter filter = VersionFilter.Parse("1.1,2.0.1");

            Assert.True(filter.IsMatch(new Version(1, 1)));
            Assert.True(filter.IsMatch(new Version(2, 0, 1)));
            Assert.True(filter.IsMatch(new Version(1, 9, 9)));
            Assert.False(filter.IsMatch(new Version(1, 0)));
            Assert.False(filter.IsMatch(new Version(2, 0, 2)));
        }

        [Fact]
        public void When_explicit_inclusive_range_version_is_specified_it_matchs()
        {
            VersionFilter filter = VersionFilter.Parse("[1.1, 2.0.1]");

            Assert.True(filter.IsMatch(new Version(1, 1)));
            Assert.True(filter.IsMatch(new Version(2, 0, 1)));
            Assert.True(filter.IsMatch(new Version(1, 9, 9)));
            Assert.False(filter.IsMatch(new Version(1, 0)));
            Assert.False(filter.IsMatch(new Version(2, 0, 2)));
        }

        [Fact]
        public void When_implicit_unbound_max_version_is_specified_it_matchs()
        {
            VersionFilter filter = VersionFilter.Parse("1.1,");

            Assert.True(filter.IsMatch(new Version(1, 1)));
            Assert.True(filter.IsMatch(new Version(2, 0, 1)));
            Assert.True(filter.IsMatch(new Version(1, 9, 9)));
            Assert.False(filter.IsMatch(new Version(1, 0)));
        }

        [Fact]
        public void When_explicit_unbound_max_version_is_specified_it_matchs()
        {
            VersionFilter filter = VersionFilter.Parse("1.1,*");

            Assert.True(filter.IsMatch(new Version(1, 1)));
            Assert.True(filter.IsMatch(new Version(2, 0, 1)));
            Assert.True(filter.IsMatch(new Version(1, 9, 9)));
            Assert.False(filter.IsMatch(new Version(1, 0)));
        }

        [Fact]
        public void When_implicit_unbound_min_version_is_specified_it_matchs()
        {
            VersionFilter filter = VersionFilter.Parse(",2.0");

            Assert.True(filter.IsMatch(new Version(1, 1)));
            Assert.True(filter.IsMatch(new Version(2, 0, 1)));
            Assert.True(filter.IsMatch(new Version(1, 9, 9)));
            Assert.False(filter.IsMatch(new Version(2, 1)));
        }

        [Fact]
        public void When_explicit_unbound_min_version_is_specified_it_matchs()
        {
            VersionFilter filter = VersionFilter.Parse("*,2.0");

            Assert.True(filter.IsMatch(new Version(1, 1)));
            Assert.True(filter.IsMatch(new Version(2, 0, 1)));
            Assert.True(filter.IsMatch(new Version(1, 9, 9)));
            Assert.False(filter.IsMatch(new Version(2, 1)));
        }
    }
}