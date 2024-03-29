using System.Globalization;
using System.IO;
using CsvHelper;

namespace Pb.DataLogger
{
    /// <summary>
    /// Logger that writes data to a file
    /// </summary>
    public class CsvLogger<T> : IDataLogger<T>
    {
        private readonly StreamWriter _sw;
        private readonly CsvWriter _csv;

        public CsvLogger(string filepath)
        {
            _sw = File.CreateText(filepath);
            _csv = new CsvWriter(_sw, CultureInfo.InvariantCulture);
            _csv.WriteHeader<T>();
            _csv.NextRecord();
        }

        /// <summary>
        /// Write the data object in the file
        /// </summary>
        public void WriteLine(T data)
        {
            _csv.WriteRecord(data);
            _csv.NextRecord();
            _csv.Flush();
        }

        public void Dispose()
        {
            _csv.Dispose();
            _sw.Dispose();
        }
    }
}
