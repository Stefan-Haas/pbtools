using System;

namespace Pb.DataLogger
{
    /// <summary>
    /// Logger that writes data to the console
    /// </summary>
    public class ConsoleLogger<T> : IDataLogger<T>
    {
        private readonly object _lock = new();

        /// <summary>
        /// Write the data object on the console
        /// </summary>
        public void WriteLine(T line)
        {
            lock (_lock)
            {
                Console.WriteLine(line);
            }
        }

        public void Dispose()
        {
        }
    }
}
