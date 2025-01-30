using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FileSortingScript.Display;

namespace FileSortingScript.Directories
{
    internal class SourceDirectory : FSPDirectory
    {
        public List<string> SourceFiles { get; set; }
        public List<string> ExcludedExtensions { get; set; }

        public SourceDirectory(string sourceDirectoryPath, List<string> excludedExtensions)
            : base(sourceDirectoryPath)
        {
            SpecialPrinting.PrintColored(
                $"Set source directory to: {sourceDirectoryPath}... ",
                ConsoleColor.Yellow,
                sourceDirectoryPath
                );

            if (excludedExtensions == null)
                throw new ArgumentNullException($"{nameof(excludedExtensions)} cannot be null in {nameof(SourceDirectory)} initialization");

            ExcludedExtensions = excludedExtensions;
            SourceFiles = SetSourceFiles();

        }

        private List<string> SetSourceFiles()
        {
            SpecialPrinting.PrintColored(
                "Retrieving source files... ",
                ConsoleColor.Yellow
                );

            List<string> sourceFiles = new List<string>();
            IEnumerable<string> files = Directory.EnumerateFiles(DirectoryPath);

            foreach (string file in files)
            {
                var extension = Path.GetExtension(file);

                if (!ExcludedExtensions.Contains(extension))
                    sourceFiles.Add(file);
            }
            SpecialPrinting.PrintColored(
                $"Retrieved {sourceFiles.Count} source files", 
                ConsoleColor.Green, 
                sourceFiles.Count
                );

            return sourceFiles;
        }
    }
}
