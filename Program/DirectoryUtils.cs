using System.Text.RegularExpressions;

namespace FileHandler
{
    public class DirectoryUtils
    {
        private static int SelectedDrive { get; set; } = 0;

        private static List<DriveInfo>? Drives { get; set; }

        public static DriveInfo? GetDrives()
        {
            Drives = DriveInfo
                .GetDrives()
                .Where(d => d.IsReady && (d.DriveType == DriveType.Fixed || d.DriveType == DriveType.Network))
                .ToList();

            DisplayDrives();
            AskDrive();

            return SelectedDrive >= 0 ? Drives[SelectedDrive] : null;
        }

        private static void DisplayDrives()
        {
            Console.WriteLine("Here are the drives we've found:");

            for (int i = 0; i < Drives?.Count; i++)
            {
                DriveInfo drive = Drives[i];
                Console.WriteLine($"{i + 1}) Name: {drive.Name} Label: {drive.VolumeLabel}");
            }
        }

        private static void AskDrive()
        {

            Console.WriteLine("Please choose the number for the drive you'd like to explore!");

            while (true)
            {
                string? _response = new UserInput().GetUserInput();
                if (_response == null) return;
                var selectedDrive = _response;
                if (int.TryParse(selectedDrive, out int result) && result > 0 && result <= Drives?.Count())
                {
                    SelectedDrive = result - 1;
                    break;
                }
                else
                {
                    Console.WriteLine("That is not a valid selection, please try again");
                }

            }
        }

        public static void ListDirectory(string directory)
        {
            UserInput type = new UserInput(prompt: $"What would you like to see in {directory}?\n1) Files\n2) Directories", directory: directory);

            string? userTypeSelection = type.Response;

            var options = new Dictionary<string, List<string>>();

            try
            {
                var dirs = Directory.EnumerateDirectories(directory).ToList();
                var files = Directory.EnumerateFiles(directory).ToList();
                options.Add("Files", files);
                options.Add("Directories", dirs);
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Access Denied: {ex.Message}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error accessing directory: {ex.Message}");
            }

            UserInput traverseSystem = new UserInput(
                originInput: userTypeSelection,
                directory: directory,
                options: options,
                ListDirectory
            );
        }

        /// <summary>
        /// Simple lists items in the Console
        /// </summary>
        public static void ListItems(List<string> items)
        {
            Console.WriteLine("\nItems Found: ");
            foreach (string item in items)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine("\n");
        }

        public static List<string> GetFiles(string directory)
        {
            string[] fileList = Directory.GetFiles(directory);
            return fileList.Select(f => Path.GetFileName(f))
                .ToList();
        }

        public static bool SearchDirectory(string directory, int searchOption)
        {
            string searchPattern;
            List<string> dirItems = Directory.EnumerateFileSystemEntries(directory).ToList();
            List<string> foundItems = new List<string>();

            // literal search (partial match) && custom pattern search (regex)
            if (searchOption is (int)SearchDirectoryOptions.LiteralSearch or (int)SearchDirectoryOptions.RegexPattern)
            {
                var prompt = searchOption is (int)SearchDirectoryOptions.LiteralSearch ? "What would you like to search for?:" : "Please enter a pattern to perform your search on (more details on patterns can be found at regex101.com)";
                searchPattern = new UserInput(prompt, directory).Response ?? "";
                while (string.IsNullOrEmpty(searchPattern)) searchPattern = new UserInput(prompt, directory).Response ?? "";
            }

            // Pre-defined search list
            else
            {
                searchPattern = @"blu-ray|t\d{2}";
                Console.WriteLine($"Setting your search pattern to the following pattern: /{searchPattern}/gi");
            } // will prolly need to refine this list

            foreach (string item in dirItems)
            {
                var noDirItem = Path.GetFileName(item);
                if (searchOption is (int)SearchDirectoryOptions.Predefined or (int)SearchDirectoryOptions.RegexPattern)
                {
                    if (Regex.IsMatch(noDirItem, searchPattern, RegexOptions.IgnoreCase))
                    {
                        foundItems.Add(noDirItem);
                    }
                }
                else if (searchOption is (int)SearchDirectoryOptions.LiteralSearch)
                {
                    if (noDirItem.Contains(searchPattern, StringComparison.OrdinalIgnoreCase))
                    {
                        foundItems.Add(noDirItem);
                    }
                }
            }
            if (foundItems.Count == 0) Console.WriteLine($"No Items found matching \"{searchPattern}\"\n");
            else ListItems(foundItems);
            return true;
        }

        public enum SearchDirectoryOptions
        {
            Predefined,
            LiteralSearch,
            RegexPattern
        }
    }
}