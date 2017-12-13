using System;
using JetBrains.Annotations;

namespace FelineSorter.Components
{
    [UsedImplicitly]
    internal class ConsoleWriter : IConsoleWriter
    {
        public void Write(object value)
        {
            Console.WriteLine(value.ToString());
        }
    }
}