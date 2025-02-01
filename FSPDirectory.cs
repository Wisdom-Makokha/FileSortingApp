using System;
using System.Collections.Generic;
using System.Text;

namespace FileSortingScript.Directories
{
    // this class will be the parent class for the directories
    internal class FSPDirectory
    {
        public string DirectoryPath { get; private set; }

        public FSPDirectory(string directoryPath)
        {
            if (string.IsNullOrEmpty(directoryPath))
                throw new ArgumentNullException(nameof(directoryPath), "No directory path provided");
            
            if (Directory.Exists(directoryPath))
                DirectoryPath = directoryPath;
            else
                throw new ArgumentException($"Provided directory path: {directoryPath} is invalid");
        }

        public void SetDirectoryPath(string directoryPath)
        {
            if (string.IsNullOrEmpty(directoryPath))
                throw new ArgumentNullException(nameof(directoryPath), "No directory path provided");

            if (Directory.Exists(directoryPath))
                DirectoryPath = directoryPath;
            else
                throw new ArgumentException($"Provided directory path: {directoryPath} is invalid");
        }
    }
}
