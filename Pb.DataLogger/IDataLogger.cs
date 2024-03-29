using System;

namespace Pb.DataLogger
{
    /// <summary>
    /// Interface for logging of data objects
    /// </summary>
    public interface IDataLogger<in T> : IDisposable
    {
        /// <summary>
        /// Write the data object depending on implementation of interface
        /// </summary>
        void WriteLine(T line);
    }
}
