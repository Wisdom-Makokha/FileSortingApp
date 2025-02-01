using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FileSortingScript.Directories;

namespace FileSortingScript.Display
{
    // this class will handle matters relating to the interface the user will interact with
    public class MainInterface
    {
        // user capabilities
        /*
         * sort files
         * check sorting statistics
         * check files that were not moved for one reason or another
         * set the destination directory
         * set the source directory
         * set the categories and extensions currently present
         * check all the above details
         * exit the interface and program
         */

        public static Settings.Settings AppSettings = Settings.Settings.Instance;

        public MainInterface()
        { }

        public void HomeInterface()
        {
            Dictionary<string, Action> InterfaceOptions = new Dictionary<string, Action>
            {
                {"sort", SortFiles },
                {"settings", CheckSettings },
                {"issues", ViewIssues },
                {"exit", ()=> { } }
            };

            bool KeepGoing = true;
            while (KeepGoing)
            {
                KeepGoing = RunOptions(InterfaceOptions, "exit");
            }
        }

        private static string GetUserInput()
        {
            string? input;
            while (true)
            {
                Console.Write("Enter value: ");
                input = Console.ReadLine();

                if (string.IsNullOrEmpty(input))
                { SpecialPrinting.PrintColored("Null or empty values not accepted.", ConsoleColor.Red); }
                else
                    break;
            }

            return input.Trim().ToLower();
        }

        private static bool RunOptions(Dictionary<string, Action> options, string exitMessage)
        {
            bool result = true;

            Console.Clear();
            SpecialPrinting.PrintColored("Pick one of these options: ", ConsoleColor.Magenta);
            SpecialPrinting.PrintColored("(Enter the text corresponding to your choice)", ConsoleColor.Magenta);

            foreach (var optionKey in options.Keys)
            {
                SpecialPrinting.PrintColored($"- {optionKey}", ConsoleColor.Magenta);
            }

            string userChoice = GetUserInput();

            if (options.ContainsKey(userChoice))
            {
                if (userChoice == exitMessage)
                {
                    return false;
                }
                else
                {
                    options[userChoice]();
                }
            }
            else
            {
                SpecialPrinting.PrintColored("Invalid choice", ConsoleColor.Red);
            }

            SpecialPrinting.PrintColored("Press <Enter> to continue.... ", ConsoleColor.Yellow);
            Console.ReadLine();

            return result;
        }

        public static void SortFiles()
        {
            DestinationDirectory? destinationDirectory = null;

            Dictionary<string, Action> subFunctions = new Dictionary<string, Action>()
            {
                {"sort", () =>
                    {
                        if(destinationDirectory == null)
                        {
                            SourceDirectory sourceDirectory = new SourceDirectory
                                (AppSettings.SourceFolder!, AppSettings);

                            destinationDirectory = new DestinationDirectory
                                (AppSettings.DestinationFolder!, sourceDirectory.SourceFiles, AppSettings);

                            destinationDirectory.CheckDestinationSubDirectories();
                            destinationDirectory.SortFiles();
                        }
                    }
                },
                {"stats", () =>
                    {
                        if(destinationDirectory == null)
                        {
                            SourceDirectory sourceDirectory = new SourceDirectory
                                (AppSettings.SourceFolder!, AppSettings);

                            destinationDirectory = new DestinationDirectory
                                (AppSettings.DestinationFolder!, sourceDirectory.SourceFiles, AppSettings);

                            destinationDirectory.CheckDestinationSubDirectories();
                            destinationDirectory.SortFiles();
                        }

                        destinationDirectory.ShowSortingStatistics();
                    }
                },
                {"back", () =>  { } }
            };

            bool KeepGoing = true;
            while (KeepGoing)
            {
                KeepGoing = RunOptions(subFunctions, "back");
            }
        }

        private static void CheckSettings()
        {
            Dictionary<string, Action> subFunctions = new Dictionary<string, Action>
            {
                {"source", CheckSourceFolder },
                {"destination", CheckDestinationFolder},
                {"excluded", CheckExcludedExtensions},
                {"categories", CheckExtensionCategories },
                {"subdirectories", ShowSubdirectories },
                {"back", ()=> { } }
            };
            bool KeepGoing = true;
            while (KeepGoing)
            {
                KeepGoing = RunOptions(subFunctions, "back");
            }
        }

        private static void CheckSourceFolder()
        {
            Dictionary<string, Action> miniFunctions = new Dictionary<string, Action>
            {
                {"show", ShowSourceFolder },
                {"set", SetSourceFolder },
                {"back", () => { } }
            };

            bool KeepGoing = true;
            while (KeepGoing)
            {
                KeepGoing = RunOptions(miniFunctions, "back");
            }
        }

        private static void ShowSourceFolder()
        {
            SpecialPrinting.PrintColored($"Source folder - {AppSettings.SourceFolder}", ConsoleColor.Green, AppSettings.SourceFolder!);
        }

        private static void SetSourceFolder()
        {
            SpecialPrinting.PrintColored("Enter the new source folder full path: ", ConsoleColor.Magenta);
            string folder = GetUserInput();

            if (Directory.Exists(folder))
            {
                AppSettings.SourceFolder = folder;

                AppSettings.SaveSettings();
                ShowSourceFolder();
            }
            else
            {
                SpecialPrinting.PrintColored(
                    $"entered folder path is invalid or does not exist - {folder}",
                    ConsoleColor.Red,
                    folder
                    );
            }
        }

        private static void CheckDestinationFolder()
        {
            Dictionary<string, Action> miniFunctions = new Dictionary<string, Action>
            {
                {"show", ShowDestinationFolder },
                {"set", SetDestinationFolder },
                {"back", () => { } }
            };

            bool KeepGoing = true;
            while (KeepGoing)
            {
                KeepGoing = RunOptions(miniFunctions, "back");
            }
        }

        private static void ShowDestinationFolder()
        {
            SpecialPrinting.PrintColored($"Destination folder - {AppSettings.DestinationFolder}", ConsoleColor.Green, AppSettings.DestinationFolder!);
        }

        private static void SetDestinationFolder()
        {
            SpecialPrinting.PrintColored("Enter the new destination folder full path: ", ConsoleColor.Magenta);
            string folder = GetUserInput();

            if (Directory.Exists(folder))
            {
                AppSettings.DestinationFolder = folder;

                AppSettings.SaveSettings();
                ShowDestinationFolder();
            }
            else
            {
                SpecialPrinting.PrintColored(
                    $"entered folder path is invalid or does not exist - {folder}",
                    ConsoleColor.Red,
                    folder
                    );
            }
        }

        private static void CheckExcludedExtensions()
        {
            Dictionary<string, Action> miniFunctions = new Dictionary<string, Action>
            {
                {"show", ShowExcludedExtensions },
                {"add", AddExcludedExtension },
                {"remove", RemoveExcludedExtensions },
                {"back", () => { } }
            };

            bool KeepGoing = true;
            while (KeepGoing)
            {
                KeepGoing = RunOptions(miniFunctions, "back");
            }
        }

        private static void ShowExcludedExtensions()
        {
            if (AppSettings.ExcludedExtensions!.Count > 0)
            {
                SpecialPrinting.PrintColored("Excluded extensions: ", ConsoleColor.Green);
                foreach (string extension in AppSettings.ExcludedExtensions!)
                {
                    SpecialPrinting.PrintColored($"  - {extension}", ConsoleColor.Green, extension);
                }
            }
            else
            {
                SpecialPrinting.PrintColored("No excluded extensions to show", ConsoleColor.Yellow);
            }
        }

        private static void AddExcludedExtension()
        {
            SpecialPrinting.PrintColored("Enter an extension to add to the list of excluded extensions", ConsoleColor.Magenta);

            string newExtension = GetUserInput();
            if (!newExtension.StartsWith('.'))
                newExtension = '.' + newExtension;

            if (AppSettings.ExcludedExtensions!.Contains(newExtension))
            {
                SpecialPrinting.PrintColored(
                    $"{newExtension} - already exists in excluded extensions",
                    ConsoleColor.Red,
                    newExtension
                    );
            }
            else
            {
                AppSettings.ExcludedExtensions!.Add(newExtension);

                SpecialPrinting.PrintColored(
                    $"{newExtension} - added to excluded extensions",
                    ConsoleColor.Green,
                    newExtension
                    );

                AppSettings.SaveSettings();
                ShowExcludedExtensions();
            }
        }

        private static void RemoveExcludedExtensions()
        {
            ShowExcludedExtensions();
            SpecialPrinting.PrintColored("Enter an extension to remove from to the list of excluded extensions", ConsoleColor.Magenta);

            string removeExtension = GetUserInput();
            if (!removeExtension.StartsWith('.'))
                removeExtension = '.' + removeExtension;

            if (AppSettings.ExcludedExtensions!.Contains(removeExtension))
            {
                AppSettings.ExcludedExtensions!.Remove(removeExtension);

                SpecialPrinting.PrintColored(
                    $"Removed extension - {removeExtension}",
                    ConsoleColor.Green,
                    removeExtension
                    );

                AppSettings.SaveSettings();
                ShowExcludedExtensions();
            }
            else
            {
                SpecialPrinting.PrintColored($"{removeExtension} - not in list of excluded extensions", ConsoleColor.Red);
            }
        }

        private static void CheckExtensionCategories()
        {
            Dictionary<string, Action> miniFunctions = new Dictionary<string, Action>()
            {
                {"show", ShowExtensionCategories },
                {"add", AddExtensionCategory },
                {"edit", EditExtensionCategory },
                {"remove", RemoveExtensionCategory },
                {"back", () => { } }
            };

            bool KeepGoing = true;
            while (KeepGoing)
            {
                KeepGoing = RunOptions(miniFunctions, "back");
            }
        }

        private static void ShowExtensionCategories()
        {
            var sortedByValues = AppSettings.ExtensionCategories!.OrderBy(kvp => kvp.Value);

            if (sortedByValues.Any())
            {
                SpecialPrinting.PrintColored("Extension categories", ConsoleColor.Green);
                foreach (var extension in sortedByValues)
                {
                    SpecialPrinting.PrintColored(
                        $"{extension.Key,6} - {extension.Value}",
                        ConsoleColor.Green,
                        extension.Key, extension.Value
                        );
                }
            }
            else
            {
                SpecialPrinting.PrintColored("No extension categories to show", ConsoleColor.Yellow);
            }
        }

        private static void AddExtensionCategory()
        {
            SpecialPrinting.PrintColored("Enter a new extension for a category: ", ConsoleColor.Magenta);
            string extension = GetUserInput();

            if (!extension.StartsWith("."))
                extension = '.' + extension;

            if (AppSettings.ExtensionCategories!.ContainsKey(extension))
            {
                SpecialPrinting.PrintColored(
                    $"{extension} already exists in extension categories",
                    ConsoleColor.Red,
                    extension
                    );
            }
            else
            {
                string category = GetUserInput();
                category = char.ToUpper(category[0]) + category.Substring(1);

                AppSettings.ExtensionCategories!.Add(extension, category);
                SpecialPrinting.PrintColored($"Added {extension} - {category} to extension categories", ConsoleColor.Green);

                AppSettings.SaveSettings();
                ShowExtensionCategories();
            }

        }

        private static void EditExtensionCategory()
        {
            SpecialPrinting.PrintColored("Pick an extension to edit the entry: ", ConsoleColor.Magenta);
            ShowExtensionCategories();

            string extension = GetUserInput();
            if (!extension.StartsWith('.'))
                extension = '.' + extension;

            if (AppSettings.ExtensionCategories!.ContainsKey(extension))
            {
                SpecialPrinting.PrintColored("Enter a new extension value: ", ConsoleColor.Magenta);
                string newExtension = GetUserInput();

                if (!newExtension.StartsWith('.'))
                    newExtension = '.' + newExtension;

                if (extension == newExtension)
                {
                    AppSettings.ExtensionCategories.Remove(extension);
                }

                if (AppSettings.ExtensionCategories.ContainsKey(newExtension))
                {
                    SpecialPrinting.PrintColored(
                        $"{newExtension} already exits as another entry in extension categories",
                        ConsoleColor.Red,
                        newExtension
                        );
                }
                else
                {
                    string newCategory = GetUserInput();
                    newCategory = char.ToUpper(newCategory[0]) + newCategory.Substring(1);

                    AppSettings.ExtensionCategories.Add(newExtension, newCategory);
                    SpecialPrinting.PrintColored(
                        $"Edited to {newExtension} - {newCategory}",
                        ConsoleColor.Green,
                        newExtension, newCategory
                        );
                    AppSettings.SaveSettings();
                    ShowExtensionCategories();
                }
            }
            else
            {
                SpecialPrinting.PrintColored(
                    $"{extension} - does not exist in extension categories to edit",
                    ConsoleColor.Red,
                    extension
                    );
            }
        }

        private static void RemoveExtensionCategory()
        {
            SpecialPrinting.PrintColored("Pick an extension to remove its entry: ", ConsoleColor.Magenta);
            ShowExtensionCategories();

            string extension = GetUserInput();
            if (AppSettings.ExtensionCategories!.ContainsKey(extension))
            {
                SpecialPrinting.PrintColored(
                    $"Removed {extension} - {AppSettings.ExtensionCategories[extension]}",
                    ConsoleColor.Green,
                    extension, AppSettings.ExtensionCategories![extension]
                    );

                AppSettings.ExtensionCategories!.Remove(extension);
                AppSettings.SaveSettings();
                ShowExtensionCategories();
            }
            else
            {
                SpecialPrinting.PrintColored($"{extension} not contained in extension categories", ConsoleColor.Red);
            }

            AppSettings.SaveSettings();
        }

        private static void ShowSubdirectories()
        {
            if (AppSettings.Subdirectories.Count > 0)
            {
                SpecialPrinting.PrintColored("Subdirectories: ", ConsoleColor.Green);
                foreach (var subdirectory in AppSettings.Subdirectories)
                {
                    SpecialPrinting.PrintColored(
                        $"  - {subdirectory}",
                        ConsoleColor.Green,
                        subdirectory
                        );
                }
            }
            else
            {
                SpecialPrinting.PrintColored($"No subdirectories to show", ConsoleColor.Yellow);
            }
        }

        private static void ViewIssues()
        {
            Dictionary<string, Action> subFunctions = new Dictionary<string, Action>()
            {
                {"failed", CheckFailedMoves},
                {"unrecognised", CheckUnrecognisedFileExtensions },
                {"back", () => { } }
            };

            bool KeepGoing = true;
            while (KeepGoing)
            {
                KeepGoing = RunOptions(subFunctions, "back");
            }
        }

        // checks files that failed to move
        private static void CheckFailedMoves()
        {
            Dictionary<string, Action> miniFunctions = new Dictionary<string, Action>()
            {
                {"show", ShowFailedMoveFiles},
                {"back", () => { } }
            };

            bool KeepGoing = true;
            while (KeepGoing)
            {
                KeepGoing = RunOptions(miniFunctions, "back");
            }
        }

        private static void ShowFailedMoveFiles()
        {

            if (DestinationDirectory.FailedToMoveFiles!.Count > 0)
            {
                SpecialPrinting.PrintColored("Files that failed to move: ", ConsoleColor.Green);
                foreach (var failedFile in DestinationDirectory.FailedToMoveFiles.Keys)
                {
                    SpecialPrinting.PrintColored(
                        $"\tFile: {failedFile}\n\tReason for failure: {DestinationDirectory.FailedToMoveFiles[failedFile]}",
                        ConsoleColor.Green,
                        failedFile, DestinationDirectory.FailedToMoveFiles[failedFile]);
                }
            }
            else
            {
                SpecialPrinting.PrintColored("All files succesfully moved", ConsoleColor.Yellow);
            }
        }

        // dialogue for adding new extensions and categories to the existing list
        private static void CheckUnrecognisedFileExtensions()
        {
            Dictionary<string, Action> miniFunctions = new Dictionary<string, Action>
            {
                {"show", ShowUnrecognisedFileExtensions },
                {"add", AddUnrecognisedToExtensionCategories },
                {"back", () => { } }
            };

            bool KeepGoing = true;
            while (KeepGoing)
            {
                KeepGoing = RunOptions(miniFunctions, "back");
            }
        }

        private static void ShowUnrecognisedFileExtensions()
        {
            if (DestinationDirectory.UnrecognisedFileExtensions!.Count > 0)
            {
                SpecialPrinting.PrintColored("Unrecognised File extensions: ", ConsoleColor.Green);
                foreach (var extension in DestinationDirectory.UnrecognisedFileExtensions!)
                {
                    SpecialPrinting.PrintColored(
                        $"  - {extension}",
                        ConsoleColor.Green,
                        extension
                        );
                }
            }
            else
            {
                SpecialPrinting.PrintColored("No unrecognised file extensions to show", ConsoleColor.Yellow);
            }
        }

        private static void AddUnrecognisedToExtensionCategories()
        {
            if (DestinationDirectory.UnrecognisedFileExtensions!.Count > 0)
            {
                ShowSubdirectories();

                foreach (var extension in DestinationDirectory.UnrecognisedFileExtensions!)
                {
                    SpecialPrinting.PrintColored(
                        $"Enter a category to enter this extension - {extension}",
                        ConsoleColor.Magenta,
                        extension
                        );
                    SpecialPrinting.PrintColored("(The category can be new or an already existing one)", ConsoleColor.Magenta);

                    string category = GetUserInput();
                    category = char.ToUpper(category[0]) + category.Substring(1);

                    AppSettings.ExtensionCategories!.Add(extension, category);

                    SpecialPrinting.PrintColored(
                        $"Added -{extension}, {category}",
                        ConsoleColor.Green,
                        extension, category
                        );
                }
                ShowExtensionCategories();
            }
            else
            {
                SpecialPrinting.PrintColored("No unrecognised file extensions to add to extension categories", ConsoleColor.Yellow);
            }
        }
    }
}
