using System;
using System.IO;
using System.Reflection;

namespace PdbIssueWithoutFody
{
    public class Program
    {
        public static void Main()
        {
            var assembly = typeof(Program).Assembly;
        }
    }
}
