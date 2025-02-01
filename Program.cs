// this is a program to sort files stored in one directory in another directory
using FileSortingScript.Settings;
using FileSortingScript.Display;

try
{
    MainInterface mainInterface = new MainInterface();

    MainInterface.SortFiles();
    mainInterface.HomeInterface();
}
catch (Exception ex)
{
    SpecialPrinting.PrintColored(
        $"Exception caught\nLocated{ex.StackTrace}\nException Type: {ex.GetType().Name}\nMessage: {ex.Message}",
        ConsoleColor.Red,
        ex.StackTrace!, ex.GetType().Name, ex.Message
        );
}

SpecialPrinting.PrintColored("Press <Enter> to continue.... ", ConsoleColor.DarkYellow);
Console.ReadLine();