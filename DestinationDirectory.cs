using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using FileSortingScript.Settings;

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
            Console.WriteLine($"Set destination directory to {destinationDirectoryPath}... ");
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
            Console.WriteLine("Checking for the subdirectories in destination directory... ");
            foreach (var subCategory in SubDirectories)
            {
                Console.WriteLine($"\tChecked: {subCategory}");
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
            Console.WriteLine("Sorting files... ");
            
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
                        Console.WriteLine($"\tSkipped file with excluded extension({extension}): {info.FullName}\n File not moved!\n");
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
                        Console.WriteLine($"\t----Sorted\n{info.FullName} into\n->{subDirectory}");
                        SortingStatistics[subDirectory] += 1;
                    }
                    catch (Exception ex)
                    {
                        FailedToMoveFiles!.Add(info.FullName, ex.Message);
                        Console.WriteLine($"Error moving file: {file} \nin {nameof(SortFiles)} \nto {Path.Combine(DirectoryPath, subDirectory)}");
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            else
            {
                Console.WriteLine("\tNo files to sort");
            }

            Console.WriteLine("\n");
        }

        // dialogue for adding new extensions and categories to the existing list
        public Dictionary<string, string> CheckUnrecognisedFileExtensions()
        {
            Console.WriteLine("Checking for unrecongised file extensions... ");
            Dictionary<string, string> newCategory = new Dictionary<string, string>();
            string exitChoice = "exit";
            string? readLine = "";
            int userPick;

            if(UnrecognisedFileExtensions!.Count > 0)
            {
                Console.WriteLine($"\t{UnrecognisedFileExtensions.Count} unrecognised file extensions in the files to be sorted");

                foreach (var extension in UnrecognisedFileExtensions)
                {
                    Console.WriteLine($"\tWhich category would you like to add this extension -{extension}- to");
                    Console.WriteLine($"\tType {exitChoice} to instead skip the choice");
                    
                    int i = 0;
                    foreach(var category in SubDirectories)
                    {
                        Console.WriteLine($"\t  {category.PadLeft(12)} - {i}");
                        i++;
                    }

                    while (true)
                    {
                        Console.Write("\tChoice - ");

                        readLine = Console.ReadLine();

                        if (string.IsNullOrEmpty(readLine))
                        {
                            Console.WriteLine("\tNull or space values not accepted");
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
                                    Console.WriteLine($"\tAdded extension category -> {extension}-{SubDirectories.ElementAt(userPick)}");
                                    break;
                                }
                                else
                                {
                                    Console.WriteLine("\tEntered value exceeds allowed range");
                                }
                            }
                            else
                            {
                                Console.WriteLine("\tEnter an integer value for your pick");
                            }
                        }
                    }
                }
                
                SortingStatistics.Add("Unrecognised Extensions", UnrecognisedFileExtensions.Count);
            }
            else
            {
                Console.WriteLine("\tNo unrecognised file extensions in the list");
            }

            Console.WriteLine("\n");

            return newCategory ;
        }

        // show 
        public void ShowSortingStatistics()
        {
            Console.WriteLine("Showing statistics from the whole operation... ");
            foreach (var statEntry in SortingStatistics.Keys)
            {
                Console.WriteLine($"\t{statEntry.PadLeft(12)} - {SortingStatistics[statEntry]}");
            }
            Console.WriteLine("\n");
        }

        public void CheckFailedMoves()
        {
            Console.WriteLine("Checking files that failed to move... ");

            if (FailedToMoveFiles!.Count > 0)
            {
                foreach (var failedFile in FailedToMoveFiles.Keys)
                {
                    Console.WriteLine($"\tFile: {failedFile}\n\tReason for failure: {FailedToMoveFiles[failedFile]}");
                }
                SortingStatistics.Add("Failed Moves", FailedToMoveFiles.Count);
            }
            else
            {
                Console.WriteLine("\tAll files succesfully moved");
            }
            Console.WriteLine("\n");
        }
    }
}
