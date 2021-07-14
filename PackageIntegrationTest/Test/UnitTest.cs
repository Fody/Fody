using System;
using System.Collections.Generic;
using SampleApplication;
using Xunit;

namespace Test
{
    public class UnitTest
    {
        [Fact]
        public void Test1()
        {
            var target = new ViewModel();

            var events = new List<string>();

            target.PropertyChanged += (sender, args) =>
            {
                events.Add(args.PropertyName);
            };

            target.Property1 = 3;
            target.Property2 = "Test";

            Assert.Equal(2, events.Count);
            Assert.Equal("Property1", events[0]);
            Assert.Equal("Property2", events[1]);
        }

        [Fact]
        public void Test2()
        {
            var target = new ViewModel();

            Assert.Throws<ArgumentNullException>(() =>
            {
                return target.Property2 = null;
            });
        }
    }
}
