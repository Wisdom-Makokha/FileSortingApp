using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using FileSortingScript.Display;

namespace FileSortingScript.Directories
{
    internal class DestinationDirectory : FSPDirectory
    {
        // any unknown file extension will be stored here
        public static HashSet<string>? UnrecognisedFileExtensions = new HashSet<string>();
        public static Dictionary<string, string>? FailedToMoveFiles = new Dictionary<string, string>();
        private Dictionary<string, int> SortingStatistics;
        public List<string> SourceFiles { get; set; }
        Settings.Settings AppSettings { get; set; }

        public DestinationDirectory(string destinationDirectoryPath, List<string> sourceFiles, Settings.Settings settings)
            : base(destinationDirectoryPath)
        {
            SpecialPrinting.PrintColored(
                $"Set destination directory to {destinationDirectoryPath}... ",
                ConsoleColor.Yellow,
                destinationDirectoryPath
                );

            if (sourceFiles == null)
                throw new ArgumentNullException($"{nameof(sourceFiles)} cannot be null in {nameof(DestinationDirectory)} initialization");

            AppSettings = settings;

            SourceFiles = sourceFiles;

            // initialise sorting statistics and add each subdirectory and set file number to zero
            SortingStatistics = new Dictionary<string, int>();
            foreach (var subDirectory in AppSettings.Subdirectories)
            {
                SortingStatistics.Add(subDirectory, 0);
            }
        }

        // check the destination directory for the subdirectories that need to be there
        public void CheckDestinationSubDirectories()
        {
            SpecialPrinting.PrintColored(
                "Checking for the AppSettings.Subdirectories in destination directory... ",
                ConsoleColor.Yellow
                );

            foreach (var subCategory in AppSettings.Subdirectories)
            {
                SpecialPrinting.PrintColored(
                    $"\tChecked: {subCategory}",
                    ConsoleColor.Green,
                    subCategory
                    );

                if (!Directory.Exists(subCategory))
                {
                    try
                    {
                        Directory.CreateDirectory(Path.Combine(DirectoryPath, subCategory));
                    }
                    catch
                    {
                        throw;
                    }
                }
            }

            Console.WriteLine("\n");
        }

        // move each file into each subdirectory
        public void SortFiles()
        {
            SpecialPrinting.PrintColored(
                "Sorting files... ",
                ConsoleColor.Yellow
                );

            string subDirectory;
            string destination;

            if (SourceFiles.Count > 0)
            {
                foreach (var file in SourceFiles)
                {
                    FileInfo info = new FileInfo(file);
                    var extension = Path.GetExtension(file);

                    if (AppSettings.ExcludedExtensions!.Contains(extension))
                    {
                        SpecialPrinting.PrintColored(
                            $"\tSkipped file with excluded extension({extension}): {info.FullName}\n File not moved!\n",
                            ConsoleColor.Yellow,
                            extension, info.FullName
                            );

                        continue;
                    }
                    else if (AppSettings.ExtensionCategories!.TryGetValue(extension, out string? value))
                    {
                        subDirectory = value;
                    }
                    else
                    {
                        UnrecognisedFileExtensions!.Add(extension);
                        subDirectory = "Other";
                    }

                    destination = Path.Combine(Path.Combine(DirectoryPath, subDirectory), Path.GetFileName(file));

                    try
                    {
                        File.Move(file, destination);

                        SpecialPrinting.PrintColored(
                            $"--Sorted\n{info.FullName} into {subDirectory}",
                            ConsoleColor.Green,
                            info.FullName, subDirectory
                            );

                        SortingStatistics[subDirectory] += 1;
                    }
                    catch (Exception ex)
                    {
                        FailedToMoveFiles!.Add(info.FullName, ex.Message);
                        SpecialPrinting.PrintColored(
                            $"Error moving file: {file} \nin {nameof(SortFiles)} \nto {Path.Combine(DirectoryPath, subDirectory)}",
                            ConsoleColor.Red,
                            nameof(SortFiles), Path.Combine(DirectoryPath, subDirectory)
                            );
                        SpecialPrinting.PrintColored(ex.Message, ConsoleColor.Red);
                    }
                }

                if(SortingStatistics.ContainsKey("Unrecognised Extensions"))
                {
                    SortingStatistics["Unrecognised Extensions"] += UnrecognisedFileExtensions!.Count;
                }
                else
                {
                    SortingStatistics.Add("Unrecognised Extensions", UnrecognisedFileExtensions!.Count);
                }

                if (SortingStatistics.ContainsKey("Failed Moves"))
                {
                    SortingStatistics["Failed Moves"] += UnrecognisedFileExtensions!.Count;
                }
                else
                {
                    SortingStatistics.Add("Failed Moves", FailedToMoveFiles!.Count);
                }
            }
            else
            {
                SpecialPrinting.PrintColored("\tNo files to sort", ConsoleColor.DarkYellow);
            }

            Console.WriteLine("\n");
        }

        // shows statistics on the process
        public void ShowSortingStatistics()
        {
            SpecialPrinting.PrintColored("Sorting stats ", ConsoleColor.Yellow);
            foreach (var statEntry in SortingStatistics.Keys)
            {
                SpecialPrinting.PrintColored(
                    $"\t{statEntry.PadLeft(12)} - {SortingStatistics[statEntry]}", 
                    ConsoleColor.Green, 
                    statEntry.PadLeft(12), SortingStatistics[statEntry]
                    );
            }
            Console.WriteLine("\n");
        }

    }
}
