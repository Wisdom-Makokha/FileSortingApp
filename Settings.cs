using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace FileSortingScript.Settings
{
    public class Settings
    {
        public static readonly Lazy<Settings> instance = new Lazy<Settings>(() => LoadSettings());
        public static Settings Instance => instance.Value;
        private Settings() { }

        private static string SettingsFilePath
        {
            get
            {
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "settings.json");

                if (!File.Exists(filePath))
                    File.Create(filePath);

                return filePath;
            }
        }

        // settings set
        public string? SourceFolder { get; set; }
        public string? DestinationFolder { get; set; }
        public List<string>? ExcludedExtensions { get; set; }
        public Dictionary<string, string>? ExtensionCategories { get; set; }
        public HashSet<string> Subdirectories => new HashSet<string>(
            ExtensionCategories!.Values.Select(subCategory => $"{subCategory}")
            .Concat(new[] { "Other" })
            );

        // add new extensions to exclude
        public void AddExcludedExtensions(List<string> excludedExtensions)
        {
            if (ExcludedExtensions != null)
            {
                foreach (string extension in excludedExtensions)
                {
                    ExcludedExtensions!.Add(extension);
                }
            }
            else
                throw new ArgumentNullException($"{nameof(excludedExtensions)} cannot be null in {nameof(AddExcludedExtensions)}");
        }

        // add new extension and categories to the dictionary
        public void AddExtensionsCategories(Dictionary<string, string> categories)
        {
            if (categories != null)
            {
                foreach (var categoryKey in categories.Keys)
                {
                    ExtensionCategories!.Add(categoryKey, categories[categoryKey]);
                }
            }
            else
                throw new ArgumentNullException($"{nameof(categories)} cannot be null in {nameof(AddExtensionsCategories)}");
        }

        // loads the settings from a file or sets the default settings in case of an issue
        private static Settings LoadSettings()
        {
            var json = File.ReadAllText(SettingsFilePath);
            if (string.IsNullOrEmpty(json))
            {
                return new Settings()
                {
                    SourceFolder = "C:\\Users\\Dev Ark\\Downloads",
                    DestinationFolder = "S:\\Pictures\\Temp",
                    ExcludedExtensions = new List<string> { ".tmp", ".bak", ".log", ".crdownload" },
                    ExtensionCategories = new Dictionary<string, string>
                        {
                            { ".flac", "Audio" },
                            { ".doc", "Documents" },
                            { ".7z", "Compressed" },
                            { ".ps1", "Scripts" },
                            { ".xlsx", "Documents" },
                            { ".jfif", "Pics" },
                            { ".docx", "Documents" },
                            { ".bat", "Scripts" },
                            { ".exe", "Software" },
                            { ".jpeg", "Pics" },
                            { ".png", "Pics" },
                            { ".pptx", "Documents" },
                            { ".gif", "GIFs" },
                            { ".rar", "Compressed" },
                            { ".mp4", "Videos" },
                            { ".zip", "Compressed" },
                            { ".avi", "Videos" },
                            { ".gz", "Compressed" },
                            { ".txt", "Documents" },
                            { ".jpg", "Pics" },
                            { ".wav", "Audio" },
                            { ".pdf", "Documents" },
                            { ".mkv", "Videos" },
                            { ".bmp", "Pics" },
                            { ".mp3", "Audio" },
                            { ".msi", "Software" },
                            { ".jar", "Compressed" }
                        }
                };
            }
            else
            {
                return JsonConvert.DeserializeObject<Settings>(json) ??
                    new Settings()
                    {
                        SourceFolder = "C:\\Users\\Dev Ark\\Downloads",
                        DestinationFolder = "S:\\Pictures\\Temp",
                        ExcludedExtensions = new List<string> { ".tmp", ".bak", ".log", ".crdownload" },
                        ExtensionCategories = new Dictionary<string, string>
                        {
                                { ".flac", "Audio" },
                                { ".doc", "Documents" },
                                { ".7z", "Compressed" },
                                { ".ps1", "Scripts" },
                                { ".xlsx", "Documents" },
                                { ".jfif", "Pics" },
                                { ".docx", "Documents" },
                                { ".bat", "Scripts" },
                                { ".exe", "Software" },
                                { ".jpeg", "Pics" },
                                { ".png", "Pics" },
                                { ".pptx", "Documents" },
                                { ".gif", "GIFs" },
                                { ".rar", "Compressed" },
                                { ".mp4", "Videos" },
                                { ".zip", "Compressed" },
                                { ".avi", "Videos" },
                                { ".gz", "Compressed" },
                                { ".txt", "Documents" },
                                { ".jpg", "Pics" },
                                { ".wav", "Audio" },
                                { ".pdf", "Documents" },
                                { ".mkv", "Videos" },
                                { ".bmp", "Pics" },
                                { ".mp3", "Audio" },
                                { ".msi", "Software" },
                                { ".jar", "Compressed" }
                        }
                    };
            }

        }

        public void SaveSettings()
        {
            try
            {
                var json = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(SettingsFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error {ex.Message}");
            }
        }

        public void ResetSettings()
        {
            SourceFolder = "C:\\Users\\Dev Ark\\Downloads";
            DestinationFolder = "S:\\Pictures\\Temp";
            ExcludedExtensions = new List<string> { ".tmp", ".bak", ".log", ".crdownload" };
            ExtensionCategories = new Dictionary<string, string>
            {
                { ".flac", "Audio" },
                { ".doc", "Documents" },
                { ".7z", "Compressed" },
                { ".ps1", "Scripts" },
                { ".xlsx", "Documents" },
                { ".jfif", "Pics" },
                { ".docx", "Documents" },
                { ".bat", "Scripts" },
                { ".exe", "Software" },
                { ".jpeg", "Pics" },
                { ".png", "Pics" },
                { ".pptx", "Documents" },
                { ".gif", "GIFs" },
                { ".rar", "Compressed" },
                { ".mp4", "Videos" },
                { ".zip", "Compressed" },
                { ".avi", "Videos" },
                { ".gz", "Compressed" },
                { ".txt", "Documents" },
                { ".jpg", "Pics" },
                { ".wav", "Audio" },
                { ".pdf", "Documents" },
                { ".mkv", "Videos" },
                { ".bmp", "Pics" },
                { ".mp3", "Audio" },
                { ".msi", "Software" },
                { ".jar", "Compressed" }
            };
        }
    }
}

