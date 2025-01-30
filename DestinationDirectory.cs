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
        private static HashSet<string>? UnrecognisedFileExtensions;
        private static Dictionary<string, string>? FailedToMoveFiles;
        private Dictionary<string, int> SortingStatistics;
        public Dictionary<string, string> ExtensionCategories { get; set; }
        public List<string> SourceFiles { get; set; }
        public List<string> ExcludedExtensions { get; set; }

        private HashSet<string> SubDirectories
        {
            get
            {
                HashSet<string> result = new HashSet<string>();

                foreach (var subCategory in ExtensionCategories.Values)
                {
                    result.Add($"{subCategory}");
                }
                result.Add("Other");

                return result;
            }
        }

        public DestinationDirectory(string destinationDirectoryPath, List<string> sourceFiles, Dictionary<string, string> extensionCategories, List<string> excludedEtensions)
            : base(destinationDirectoryPath)
        {
            SpecialPrinting.PrintColored(
                $"Set destination directory to {destinationDirectoryPath}... ",
                ConsoleColor.Yellow,
                destinationDirectoryPath
                );

            if (sourceFiles == null)
                throw new ArgumentNullException($"{nameof(sourceFiles)} cannot be null in {nameof(DestinationDirectory)} initialization");

            SourceFiles = sourceFiles;

            if (extensionCategories == null)
                throw new ArgumentNullException($"{nameof(extensionCategories)} cannot be null in {nameof(DestinationDirectory)} initialization");

            ExtensionCategories = extensionCategories;

            if (excludedEtensions == null)
                throw new ArgumentNullException($"{nameof(excludedEtensions)} cannot be null in {nameof(DestinationDirectory)} initialization");

            ExcludedExtensions = excludedEtensions;

            // initialise sorting statistics and add each subdirectory and set file number to zero
            SortingStatistics = new Dictionary<string, int>();
            foreach (var subDirectory in SubDirectories)
            {
                SortingStatistics.Add(subDirectory, 0);
            }

            UnrecognisedFileExtensions = new HashSet<string>();
            FailedToMoveFiles = new Dictionary<string, string>();
        }

        // check the destination directory for the 
        public void CheckDestinationSubDirectories()
        {
            SpecialPrinting.PrintColored(
                "Checking for the subdirectories in destination directory... ",
                ConsoleColor.Yellow
                );

            foreach (var subCategory in SubDirectories)
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

                    if (ExcludedExtensions.Contains(extension))
                    {
                        SpecialPrinting.PrintColored(
                            $"\tSkipped file with excluded extension({extension}): {info.FullName}\n File not moved!\n",
                            ConsoleColor.Yellow,
                            extension, info.FullName
                            );

                        continue;
                    }
                    else if (ExtensionCategories.TryGetValue(extension, out string? value))
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
            }
            else
            {
                SpecialPrinting.PrintColored("\tNo files to sort", ConsoleColor.DarkYellow);
            }

            Console.WriteLine("\n");
        }

        // dialogue for adding new extensions and categories to the existing list
        public Dictionary<string, string> CheckUnrecognisedFileExtensions()
        {
            SpecialPrinting.PrintColored(
                "Checking for unrecongised file extensions... ",
                ConsoleColor.Yellow
                );

            Dictionary<string, string> newCategory = new Dictionary<string, string>();
            string exitChoice = "exit";
            string? readLine = "";
            int userPick;

            if (UnrecognisedFileExtensions!.Count > 0)
            {
                SpecialPrinting.PrintColored(
                    $"\t{UnrecognisedFileExtensions.Count} unrecognised file extensions in the files to be sorted",
                    ConsoleColor.Green,
                    UnrecognisedFileExtensions.Count
                    );

                foreach (var extension in UnrecognisedFileExtensions)
                {
                    SpecialPrinting.PrintColored(
                        $"\tWhich category would you like to add this extension -{extension}- to",
                        ConsoleColor.Magenta, 
                        extension
                        );

                    SpecialPrinting.PrintColored(
                        $"\tType {exitChoice} to instead skip the choice", 
                        ConsoleColor.Magenta, 
                        exitChoice
                        );

                    int i = 0;
                    foreach (var category in SubDirectories)
                    {
                        SpecialPrinting.PrintColored(
                            $"\t  {category.PadLeft(12)} - {i}",
                            ConsoleColor.Magenta,
                            category.PadLeft(12), i
                            );
                        i++;
                    }

                    while (true)
                    {
                        SpecialPrinting.PrintColored("\tChoice - ", ConsoleColor.Blue);

                        readLine = Console.ReadLine();

                        if (string.IsNullOrEmpty(readLine))
                        {
                            SpecialPrinting.PrintColored("\tNull or space values not accepted", ConsoleColor.Magenta);
                        }
                        else if (readLine.Trim().ToLower() == exitChoice.ToLower())
                        {
                            break;
                        }
                        else
                        {
                            if (int.TryParse(readLine, out userPick))
                            {
                                if (userPick >= 0 || userPick < SubDirectories.Count)
                                {
                                    newCategory.Add(extension, SubDirectories.ElementAt(userPick));
                                    SpecialPrinting.PrintColored(
                                        $"\tAdded extension category -> {extension}-{SubDirectories.ElementAt(userPick)}",
                                        ConsoleColor.Green,
                                        extension, SubDirectories.ElementAt(userPick)
                                        );

                                    break;
                                }
                                else
                                {
                                    SpecialPrinting.PrintColored("\tEntered value exceeds allowed range", ConsoleColor.Red);
                                }
                            }
                            else
                            {
                                SpecialPrinting.PrintColored("\tEnter an integer value for your pick", ConsoleColor.Red);
                            }
                        }
                    }
                }

                SortingStatistics.Add("Unrecognised Extensions", UnrecognisedFileExtensions.Count);
            }
            else
            {
                SpecialPrinting.PrintColored("\tNo unrecognised file extensions in the list", ConsoleColor.DarkYellow);
            }

            Console.WriteLine("\n");

            return newCategory;
        }

        // shows statistics on the process
        public void ShowSortingStatistics()
        {
            SpecialPrinting.PrintColored("Showing statistics from the whole operation... ", ConsoleColor.Yellow);
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

        // shows files that failed to move
        public void CheckFailedMoves()
        {
            SpecialPrinting.PrintColored("Checking files that failed to move... ", ConsoleColor.Yellow);

            if (FailedToMoveFiles!.Count > 0)
            {
                foreach (var failedFile in FailedToMoveFiles.Keys)
                {
                    SpecialPrinting.PrintColored(
                        $"\tFile: {failedFile}\n\tReason for failure: {FailedToMoveFiles[failedFile]}",
                        ConsoleColor.DarkYellow,
                        failedFile, FailedToMoveFiles[failedFile]);
                }
                SortingStatistics.Add("Failed Moves", FailedToMoveFiles.Count);
            }
            else
            {
                SpecialPrinting.PrintColored("\tAll files succesfully moved", ConsoleColor.Green);
            }
            Console.WriteLine("\n");
        }
    }
}
