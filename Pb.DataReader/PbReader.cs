using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using Pb.DataModel;

namespace Pb.DataReader
{
    /// <summary>
    /// Class with multiple methods for reading files in PABULIB file format
    /// </summary>
    public static class PbReader
    {
        /// <summary>
        /// Reads all files from a directory in pb file format according to PABULIB and returns a list of of PbInstance, one for each file.
        /// </summary>
        public static List<PbInstance> ReadAllFilesFromDirectory(string directory)
        {
            if (!Directory.Exists(directory))
            {
                throw new DirectoryNotFoundException(directory);
            }

            var dir = new DirectoryInfo(directory);

            var result = new List<PbInstance>();
            foreach (var file in dir.GetFiles())
            {
                try
                {
                    var item = ReadFile(file.FullName);
                    result.Add(item);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error while reading file {file.Name}: {e.Message}");
                }
                
            }

            return result;
        }

        /// <summary>
        /// Reads a file in pb file format according to PABULIB and returns a PbInstance
        /// </summary>
        public static PbInstance ReadFile(string filepath)
        {
            if (!File.Exists(filepath))
            {
                throw new FileNotFoundException(filepath);
            }

            var meta = new PbMeta
            {
                FileName = Path.GetFileName(filepath)
            };
            IList<PbProject> projects = new List<PbProject>();
            IList<PbVoter> votes = new List<PbVoter>();

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";"
            };

            using (var stream = File.Open(filepath, FileMode.Open))
            using (var reader = new StreamReader(stream))
            using (var csvReader = new CsvReader(reader, config))
            {
                csvReader.Context.RegisterClassMap<PbProjectMap>();
                csvReader.Context.RegisterClassMap<PbVoterMap>();

                var section = PbSection.None;
                var isHeader = false;

                while (csvReader.Read())
                {
                    var line = csvReader.Parser.Record?.First();
                    if (line == null)
                    {
                        throw new ParserException(csvReader.Context);
                    }

                    switch (csvReader.Parser.Record?.First())
                    {
                        case "META":
                            section = PbSection.Meta;
                            isHeader = true;
                            continue;
                        case "PROJECTS":
                            section = PbSection.Projects;
                            isHeader = true;
                            continue;
                        case "VOTES":
                            section = PbSection.Votes;
                            isHeader = true;
                            continue;
                    }

                    if (isHeader)
                    {
                        csvReader.ReadHeader();
                        isHeader = false;
                        continue;
                    }


                    switch (section)
                    {
                        case PbSection.Meta:
                        {
                            var key = csvReader.Parser.Record?.First();
                            if (key == null)
                            {
                                throw new ParserException(csvReader.Context);
                            }
                            var value = csvReader.Parser.Record?[1];
                            if (value == null)
                            {
                                throw new ParserException(csvReader.Context);
                            }

                            switch (key)
                            {
                                case "vote_type":
                                    meta.VoteType = VoteTypeMap[value];
                                    break;
                                case "budget":
                                    meta.Budget = double.Parse(value, CultureInfo.InvariantCulture);
                                    break;
                            }

                            break;
                        }
                        case PbSection.Projects:
                        {
                            var project = csvReader.GetRecord<PbProject>();
                            if (project == null)
                            {
                                throw new ParserException(csvReader.Context);
                            }
                            projects.Add(project);
                            break;
                        }
                        case PbSection.Votes:
                        {
                            var voter = csvReader.GetRecord<PbVoter>();
                            if (voter == null)
                            {
                                throw new ParserException(csvReader.Context);
                            }
                            votes.Add(voter);
                            break;
                        }
                    }
                }
            }

            return new PbInstance(meta, projects, votes);
        }

        private enum PbSection
        {
            None,
            Meta,
            Projects,
            Votes
        }

        private static readonly Dictionary<string, VoteType> VoteTypeMap = new()
        {
            { "approval", VoteType.Approval },
            { "cumulative", VoteType.Cumulative },
            { "ordinal", VoteType.Ordinal },
            { "scoring", VoteType.Scoring }
        };
    }
}