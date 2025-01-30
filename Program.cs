// this is a program to sort files stored in one directory in another directory
using FileSortingScript.Directories;
using FileSortingScript.Settings;
using FileSortingScript.Display;

try
{
    Settings settings = Settings.Instance;

    SourceDirectory sourceDirectory = new SourceDirectory
        (sourceDirectoryPath: settings.SourceFolder, excludedExtensions: settings.ExcludedExtensions);

    DestinationDirectory destinationDirectory = new DestinationDirectory
        (destinationDirectoryPath: settings.DestinationFolder, sourceFiles: sourceDirectory.SourceFiles, extensionCategories: settings.ExtensionCategories, excludedEtensions: settings.ExcludedExtensions);

    destinationDirectory.CheckDestinationSubDirectories();
    destinationDirectory.SortFiles();
    destinationDirectory.CheckFailedMoves();
    // add unrecognised extensions and subdirectories
    settings.AddExtensionsCategories(destinationDirectory.CheckUnrecognisedFileExtensions());

    destinationDirectory.ShowSortingStatistics();

    settings.SaveSettings();
}
catch (Exception ex)
{
    SpecialPrinting.PrintColored(
        $"Exception caught\nLocated{ex.StackTrace}\nException Type: {ex.GetType().Name}\nMessage: {ex.Message}",
        ConsoleColor.Red,
        ex.StackTrace, ex.GetType().Name, ex.Message
        );
}

Console.WriteLine("Press <Enter> to continue.... ");
Console.ReadLine();